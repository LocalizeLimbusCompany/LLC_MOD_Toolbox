using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace LLC_MOD_Toolbox.Services.Skin
{
    public sealed class SkinManager
    {
        private sealed record SkinPropertyValue(JToken Value, string JsonPath, bool IsLegacy);

        private sealed class SkinPackage
        {
            public required string DirectoryName { get; init; }
            public required string ConfigPath { get; init; }
            public required SkinDefinition Definition { get; init; }
            public Dictionary<string, Dictionary<string, SkinPropertyValue>> Elements { get; } = new(StringComparer.Ordinal);
            public List<SkinDynamicImageDefinition> DynamicImages { get; } = [];
            public HashSet<string> AssetPaths { get; } = new(StringComparer.OrdinalIgnoreCase);
            public List<string> Warnings { get; } = [];
        }

        private sealed class SkinTreeIndex
        {
            public Dictionary<string, List<FrameworkElement>> NamedElements { get; } = new(StringComparer.Ordinal);
            public Dictionary<string, Panel> HostPanels { get; } = new(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, Dictionary<string, List<FrameworkElement>>> HostElements { get; } = new(StringComparer.OrdinalIgnoreCase);
        }

        private sealed record OriginalPropertyValue(object LocalValue, BindingBase? Binding);

        private sealed class PreparedApplication
        {
            public List<PreparedSkinProperty> Properties { get; } = [];
            public List<(Panel Host, Image Image)> DynamicImages { get; } = [];
            public HashSet<string> DynamicImageNames { get; } = new(StringComparer.Ordinal);
            public List<string> Warnings { get; } = [];
        }

        private static SkinManager? _instance;
        private static readonly object InstanceLock = new();
        private static readonly Regex ValidElementName = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

        private readonly string _skinsDirectory;
        private readonly string _defaultSkinName = "default";
        private readonly Dictionary<(DependencyObject Target, DependencyProperty Property), OriginalPropertyValue> _originalValues = [];
        private readonly List<(Panel Host, Image Image)> _activeDynamicImages = [];
        private SkinPackage _defaultPackage;
        private SkinPackage _currentPackage;
        private SkinPackage? _pendingPackage;
        private Window? _lastWindow;

        public static SkinManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (InstanceLock)
                {
                    _instance ??= new SkinManager();
                    return _instance;
                }
            }
        }

        private SkinManager()
        {
            _skinsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skins");
            EnsureSkinsDirectoryExists();

            SkinApplyResult defaultResult = TryReadPackage(_defaultSkinName, out SkinPackage? defaultPackage);
            if (!defaultResult.Success || defaultPackage == null)
            {
                Log.logger.Error($"加载默认皮肤失败: {defaultResult.ErrorPath}, {defaultResult.ErrorMessage}");
                defaultPackage = CreateEmptyDefaultPackage();
            }

            _defaultPackage = defaultPackage;
            _currentPackage = defaultPackage;
        }

        public SkinApplyResult LoadSkin(string skinName)
        {
            SkinApplyResult result = TryReadPackage(skinName, out SkinPackage? package);
            if (result.Success && package != null)
            {
                _pendingPackage = package;
                Log.logger.Info($"皮肤配置已通过预检: {package.Definition.displayName} v{package.Definition.version} by {package.Definition.author}");
            }
            else
            {
                _pendingPackage = null;
                Log.logger.Error($"加载皮肤失败: {skinName}, {result.ErrorPath}, {result.ErrorMessage}");
            }

            return result;
        }

        public SkinApplyResult ApplySkinToWindow(Window window)
        {
            if (window == null)
                return SkinApplyResult.Failed("$", "窗口对象为空");

            _lastWindow = window;
            SkinPackage packageToApply = _pendingPackage ?? _currentPackage;
            var index = BuildTreeIndex(window);

            SkinApplyResult prepareResult = TryPrepareApplication(index, packageToApply, out PreparedApplication? prepared);
            if (!prepareResult.Success || prepared == null)
            {
                _pendingPackage = null;
                return prepareResult;
            }

            try
            {
                RestoreOriginalState();
                ApplyPreparedApplication(prepared);
                if (packageToApply.DirectoryName.Equals(_defaultSkinName, StringComparison.OrdinalIgnoreCase))
                    _defaultPackage = packageToApply;
                _currentPackage = packageToApply;
                foreach (string warning in prepared.Warnings.Distinct())
                    Log.logger.Warn(warning);
                _pendingPackage = null;
                Log.logger.Info($"皮肤应用完成: {_currentPackage.Definition.displayName}");
                return SkinApplyResult.Succeeded(prepared.Warnings);
            }
            catch (Exception ex)
            {
                Log.logger.Error("应用皮肤失败，正在恢复最后一次成功配置。", ex);
                _pendingPackage = null;
                try
                {
                    RestoreOriginalState();
                    var recoveryIndex = BuildTreeIndex(window);
                    SkinApplyResult recoveryResult = TryPrepareApplication(recoveryIndex, _currentPackage, out PreparedApplication? recovery);
                    if (recoveryResult.Success && recovery != null)
                        ApplyPreparedApplication(recovery);
                }
                catch (Exception recoveryException)
                {
                    Log.logger.Error("恢复最后一次成功皮肤失败。", recoveryException);
                }

                return SkinApplyResult.Failed("$", ex.Message, prepared.Warnings);
            }
        }

        public SkinApplyResult ReloadCurrentSkin()
        {
            if (_lastWindow == null)
                return SkinApplyResult.Failed("$", "窗口尚未初始化");

            SkinApplyResult loadResult = TryReadPackage(_currentPackage.DirectoryName, out SkinPackage? reloadedPackage);
            if (!loadResult.Success || reloadedPackage == null)
                return loadResult;

            _pendingPackage = reloadedPackage;
            return ApplySkinToWindow(_lastWindow);
        }

        public List<string> GetAvailableSkins()
        {
            try
            {
                if (!Directory.Exists(_skinsDirectory))
                    return [];

                return Directory.GetDirectories(_skinsDirectory)
                    .Select(Path.GetFileName)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => name!)
                    .Where(name => File.Exists(Path.Combine(_skinsDirectory, name, "skin.json")))
                    .ToList();
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取可用皮肤列表失败。", ex);
                return [];
            }
        }

        public SkinDefinition? GetSkinInfo(string skinName)
        {
            SkinApplyResult result = TryReadPackage(skinName, out SkinPackage? package);
            if (!result.Success)
            {
                Log.logger.Warn($"获取皮肤信息失败: {skinName}, {result.ErrorPath}, {result.ErrorMessage}");
                return null;
            }

            return package?.Definition;
        }

        public string? GetCurrentSkinMusicPath()
        {
            string? musicPath = _currentPackage.Definition.music?.musicPath;
            if (string.IsNullOrWhiteSpace(musicPath))
                return null;

            string? fullPath = ResolveAssetPath(_currentPackage.DirectoryName, musicPath);
            return File.Exists(fullPath) ? fullPath : null;
        }

        public bool SaveCurrentSkinMusicEnabled(bool enabled)
        {
            SkinMusicConfig? music = _currentPackage.Definition.music;
            if (music == null)
                return false;

            try
            {
                string configPath = _currentPackage.ConfigPath;
                var root = JObject.Parse(File.ReadAllText(configPath));
                if (root["music"] is not JObject musicObject)
                    return false;

                musicObject["enableMusic"] = enabled;
                File.WriteAllText(configPath, root.ToString(Formatting.Indented));
                music.enableMusic = enabled;
                Log.logger.Info($"皮肤音乐状态已保存: {CurrentSkinName}, enableMusic={enabled}");
                return true;
            }
            catch (Exception ex)
            {
                Log.logger.Error($"保存皮肤音乐状态失败: {CurrentSkinName}", ex);
                return false;
            }
        }

        public string CurrentSkinName => _currentPackage.Definition.name;
        public SkinDefinition CurrentSkinInfo => _currentPackage.Definition;
        public string CurrentConfigPath => _currentPackage.ConfigPath;
        public IReadOnlyCollection<string> CurrentAssetPaths => _defaultPackage.AssetPaths
            .Concat(_currentPackage.AssetPaths)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        private SkinApplyResult TryReadPackage(string skinName, out SkinPackage? package)
        {
            package = null;
            if (string.IsNullOrWhiteSpace(skinName))
                return SkinApplyResult.Failed("$.name", "皮肤名称不能为空");

            string skinDirectory = Path.Combine(_skinsDirectory, skinName);
            string configPath = Path.Combine(skinDirectory, "skin.json");
            if (!Directory.Exists(skinDirectory))
                return SkinApplyResult.Failed("$", $"皮肤目录不存在：{skinDirectory}");
            if (!File.Exists(configPath))
                return SkinApplyResult.Failed("$", $"皮肤配置不存在：{configPath}");

            string json;
            JObject root;
            try
            {
                json = File.ReadAllText(configPath);
                using (JsonDocument.Parse(json, new JsonDocumentOptions
                {
                    AllowTrailingCommas = false,
                    CommentHandling = JsonCommentHandling.Disallow
                }))
                {
                }
                root = JObject.Parse(json);
            }
            catch (System.Text.Json.JsonException ex)
            {
                string path = string.IsNullOrWhiteSpace(ex.Path) ? "$" : ex.Path!;
                long line = (ex.LineNumber ?? 0) + 1;
                long position = (ex.BytePositionInLine ?? 0) + 1;
                return SkinApplyResult.Failed(path, $"JSON 语法错误（第 {line} 行，第 {position} 列）：{ex.Message}");
            }
            catch (JsonReaderException ex)
            {
                string path = string.IsNullOrWhiteSpace(ex.Path) ? "$" : $"$.{ex.Path}";
                return SkinApplyResult.Failed(path, $"JSON 语法错误（第 {ex.LineNumber} 行，第 {ex.LinePosition} 列）：{ex.Message}");
            }
            catch (Exception ex)
            {
                return SkinApplyResult.Failed("$", ex.Message);
            }

            SkinApplyResult? shapeError = ValidateConfigurationShape(root);
            if (shapeError != null)
                return shapeError;

            SkinDefinition definition;
            try
            {
                definition = root.ToObject<SkinDefinition>() ?? throw new JsonSerializationException("配置内容为空");
            }
            catch (JsonSerializationException ex)
            {
                string path = string.IsNullOrWhiteSpace(ex.Path) ? "$" : $"$.{ex.Path}";
                return SkinApplyResult.Failed(path, ex.Message);
            }

            if (definition.schemaVersion is > 2)
                return SkinApplyResult.Failed("$.schemaVersion", $"不支持的 schemaVersion：{definition.schemaVersion}");
            if (string.IsNullOrWhiteSpace(definition.name))
                return SkinApplyResult.Failed("$.name", "name 不能为空");

            var resultPackage = new SkinPackage
            {
                DirectoryName = skinName,
                ConfigPath = configPath,
                Definition = definition
            };

            AddLegacyProperties(resultPackage, definition);

            if (root["elements"] is JObject elementsObject)
            {
                foreach (JProperty elementProperty in elementsObject.Properties())
                {
                    if (elementProperty.Value is not JObject propertyObject)
                        return SkinApplyResult.Failed($"$.elements['{elementProperty.Name}']", "元素属性必须是 JSON 对象");

                    Dictionary<string, SkinPropertyValue> properties = GetOrCreateElement(resultPackage, elementProperty.Name);
                    foreach (JProperty property in propertyObject.Properties())
                    {
                        string path = $"$.elements['{elementProperty.Name}'].{property.Name}";
                        if (!SkinPropertyRegistry.IsSupported(property.Name))
                            return SkinApplyResult.Failed(path, $"未知属性：{property.Name}");
                        properties[property.Name] = new SkinPropertyValue(property.Value.DeepClone(), path, false);
                    }
                }
            }

            var dynamicNames = new HashSet<string>(StringComparer.Ordinal);
            if (root["dynamicImages"] is JArray dynamicArray)
            {
                for (int i = 0; i < dynamicArray.Count; i++)
                {
                    if (dynamicArray[i] is not JObject dynamicObject)
                        return SkinApplyResult.Failed($"$.dynamicImages[{i}]", "动态图片定义必须是 JSON 对象");

                    SkinDynamicImageDefinition? dynamicImage;
                    try
                    {
                        dynamicImage = dynamicObject.ToObject<SkinDynamicImageDefinition>();
                    }
                    catch (JsonSerializationException ex)
                    {
                        return SkinApplyResult.Failed($"$.dynamicImages[{i}]", ex.Message);
                    }

                    if (dynamicImage == null)
                        return SkinApplyResult.Failed($"$.dynamicImages[{i}]", "动态图片定义为空");
                    if (string.IsNullOrWhiteSpace(dynamicImage.name) || !ValidElementName.IsMatch(dynamicImage.name))
                        return SkinApplyResult.Failed($"$.dynamicImages[{i}].name", "name 必须是有效且非空的 WPF 元素名");
                    if (!dynamicNames.Add(dynamicImage.name))
                        return SkinApplyResult.Failed($"$.dynamicImages[{i}].name", $"动态图片名称重复：{dynamicImage.name}");
                    if (string.IsNullOrWhiteSpace(dynamicImage.host))
                        return SkinApplyResult.Failed($"$.dynamicImages[{i}].host", "host 不能为空");
                    if (string.IsNullOrWhiteSpace(dynamicImage.source))
                        return SkinApplyResult.Failed($"$.dynamicImages[{i}].source", "source 不能为空");
                    if (dynamicImage.properties.Property("source", StringComparison.OrdinalIgnoreCase) != null)
                        return SkinApplyResult.Failed($"$.dynamicImages[{i}].properties.source", "动态图片 source 必须写在外层 source 字段");

                    foreach (JProperty property in dynamicImage.properties.Properties())
                    {
                        if (!SkinPropertyRegistry.IsSupported(property.Name))
                            return SkinApplyResult.Failed($"$.dynamicImages[{i}].properties.{property.Name}", $"未知属性：{property.Name}");
                    }

                    string? sourcePath = ResolveAssetPath(skinName, dynamicImage.source);
                    if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                        return SkinApplyResult.Failed($"$.dynamicImages[{i}].source", $"素材不存在：{dynamicImage.source}");
                    resultPackage.AssetPaths.Add(sourcePath);
                    resultPackage.DynamicImages.Add(dynamicImage);
                }
            }

            foreach ((string elementName, Dictionary<string, SkinPropertyValue> properties) in resultPackage.Elements)
            {
                if (!properties.TryGetValue("source", out SkinPropertyValue? sourceProperty))
                    sourceProperty = properties.FirstOrDefault(item => item.Key.Equals("source", StringComparison.OrdinalIgnoreCase)).Value;
                if (sourceProperty == null)
                    continue;

                if (sourceProperty.Value.Type != JTokenType.String)
                    return SkinApplyResult.Failed(sourceProperty.JsonPath, "source 必须是字符串");
                string source = sourceProperty.Value.Value<string>()!;
                string? sourcePath = ResolveAssetPath(skinName, source);
                if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                    return SkinApplyResult.Failed(sourceProperty.JsonPath, $"素材不存在：{source}");
                resultPackage.AssetPaths.Add(sourcePath);
            }

            string? musicPath = definition.music?.musicPath;
            if (!string.IsNullOrWhiteSpace(musicPath))
            {
                string? resolvedMusicPath = ResolveAssetPath(skinName, musicPath);
                if (string.IsNullOrWhiteSpace(resolvedMusicPath) || !File.Exists(resolvedMusicPath))
                    return SkinApplyResult.Failed("$.music.musicPath", $"素材不存在：{musicPath}");
                resultPackage.AssetPaths.Add(resolvedMusicPath);
            }

            package = resultPackage;
            return SkinApplyResult.Succeeded(resultPackage.Warnings);
        }

        private static SkinApplyResult? ValidateConfigurationShape(JObject root)
        {
            SkinApplyResult? imagesError = ValidateObjectValues(root, "images", JTokenType.String);
            if (imagesError != null)
                return imagesError;
            SkinApplyResult? visibilityError = ValidateObjectValues(root, "visibility", JTokenType.Boolean);
            if (visibilityError != null)
                return visibilityError;
            SkinApplyResult? marginsError = ValidateObjectValues(root, "margins", JTokenType.String);
            if (marginsError != null)
                return marginsError;

            if (root.TryGetValue("elements", out JToken? elements) && elements.Type is not (JTokenType.Object or JTokenType.Null))
                return SkinApplyResult.Failed("$.elements", "elements 必须是 JSON 对象");
            if (root.TryGetValue("dynamicImages", out JToken? dynamicImages) && dynamicImages.Type is not (JTokenType.Array or JTokenType.Null))
                return SkinApplyResult.Failed("$.dynamicImages", "dynamicImages 必须是 JSON 数组");
            if (root.TryGetValue("music", out JToken? music) && music.Type is not (JTokenType.Object or JTokenType.Null))
                return SkinApplyResult.Failed("$.music", "music 必须是 JSON 对象");

            if (music is JObject musicObject)
            {
                if (musicObject.TryGetValue("enableMusic", out JToken? enabled) && enabled.Type != JTokenType.Boolean)
                    return SkinApplyResult.Failed("$.music.enableMusic", "enableMusic 必须是布尔值");
                if (musicObject.TryGetValue("musicPath", out JToken? path) && path.Type != JTokenType.String)
                    return SkinApplyResult.Failed("$.music.musicPath", "musicPath 必须是字符串");
            }

            return null;
        }

        private static SkinApplyResult? ValidateObjectValues(JObject root, string propertyName, JTokenType expectedType)
        {
            if (!root.TryGetValue(propertyName, out JToken? token) || token.Type == JTokenType.Null)
                return null;
            if (token is not JObject valueObject)
                return SkinApplyResult.Failed($"$.{propertyName}", $"{propertyName} 必须是 JSON 对象");

            foreach (JProperty property in valueObject.Properties())
            {
                if (property.Value.Type != expectedType)
                {
                    string typeName = expectedType switch
                    {
                        JTokenType.String => "字符串",
                        JTokenType.Boolean => "布尔值",
                        _ => expectedType.ToString()
                    };
                    return SkinApplyResult.Failed($"$.{propertyName}['{property.Name}']", $"值必须是{typeName}");
                }
            }

            return null;
        }

        private static void AddLegacyProperties(SkinPackage package, SkinDefinition definition)
        {
            foreach ((string name, string source) in definition.images ?? [])
            {
                GetOrCreateElement(package, name)["source"] = new SkinPropertyValue(
                    JValue.CreateString(source),
                    $"$.images['{name}']",
                    true);
            }

            foreach ((string name, bool visibility) in definition.visibility ?? [])
            {
                GetOrCreateElement(package, name)["visibility"] = new SkinPropertyValue(
                    new JValue(visibility),
                    $"$.visibility['{name}']",
                    true);
            }

            foreach ((string name, string margin) in definition.margins ?? [])
            {
                GetOrCreateElement(package, name)["margin"] = new SkinPropertyValue(
                    JValue.CreateString(margin),
                    $"$.margins['{name}']",
                    true);
            }
        }

        private static Dictionary<string, SkinPropertyValue> GetOrCreateElement(SkinPackage package, string elementName)
        {
            if (!package.Elements.TryGetValue(elementName, out Dictionary<string, SkinPropertyValue>? properties))
            {
                properties = new Dictionary<string, SkinPropertyValue>(StringComparer.OrdinalIgnoreCase);
                package.Elements[elementName] = properties;
            }

            return properties;
        }

        private SkinApplyResult TryPrepareApplication(SkinTreeIndex index, SkinPackage currentPackage, out PreparedApplication? prepared)
        {
            prepared = new PreparedApplication();
            SkinPackage basePackage = currentPackage.DirectoryName.Equals(_defaultSkinName, StringComparison.OrdinalIgnoreCase)
                ? currentPackage
                : _defaultPackage;
            SkinApplyResult defaultResult = TryPreparePackage(index, basePackage, prepared, includeDynamicImages: true);
            if (!defaultResult.Success)
            {
                prepared = null;
                return defaultResult;
            }

            if (!ReferenceEquals(currentPackage, basePackage))
            {
                SkinApplyResult currentResult = TryPreparePackage(index, currentPackage, prepared, includeDynamicImages: true);
                if (!currentResult.Success)
                {
                    prepared = null;
                    return currentResult;
                }
            }
            prepared.Warnings.AddRange(basePackage.Warnings);
            if (!ReferenceEquals(currentPackage, basePackage))
                prepared.Warnings.AddRange(currentPackage.Warnings);
            return SkinApplyResult.Succeeded(prepared.Warnings);
        }

        private SkinApplyResult TryPreparePackage(SkinTreeIndex index, SkinPackage package, PreparedApplication prepared, bool includeDynamicImages)
        {
            foreach ((string targetName, Dictionary<string, SkinPropertyValue> properties) in package.Elements)
            {
                bool isLegacyOnly = properties.Values.All(property => property.IsLegacy);
                SkinApplyResult targetResult = ResolveTarget(index, targetName, isLegacyOnly, out FrameworkElement? target, out string? warning);
                if (!targetResult.Success)
                    return targetResult;
                if (warning != null)
                {
                    prepared.Warnings.Add(warning);
                    continue;
                }

                foreach ((string propertyName, SkinPropertyValue property) in properties)
                {
                    if (!SkinPropertyRegistry.TryPrepare(
                            target!,
                            propertyName,
                            property.Value,
                            property.JsonPath,
                            source => ResolveAssetPath(package.DirectoryName, source),
                            out PreparedSkinProperty? preparedProperty,
                            out string? error))
                    {
                        return SkinApplyResult.Failed(property.JsonPath, error ?? "属性转换失败", prepared.Warnings);
                    }

                    prepared.Properties.Add(preparedProperty!);
                }
            }

            return includeDynamicImages
                ? TryPrepareDynamicImages(index, package, prepared)
                : SkinApplyResult.Succeeded(prepared.Warnings);
        }

        private SkinApplyResult TryPrepareDynamicImages(SkinTreeIndex index, SkinPackage package, PreparedApplication prepared)
        {
            for (int i = 0; i < package.DynamicImages.Count; i++)
            {
                SkinDynamicImageDefinition definition = package.DynamicImages[i];
                if (!index.HostPanels.TryGetValue(definition.host, out Panel? host))
                    return SkinApplyResult.Failed($"$.dynamicImages[{i}].host", $"未知宿主：{definition.host}", prepared.Warnings);
                if (index.NamedElements.ContainsKey(definition.name) || !prepared.DynamicImageNames.Add(definition.name))
                    return SkinApplyResult.Failed($"$.dynamicImages[{i}].name", $"元素名称已存在：{definition.name}", prepared.Warnings);

                var image = new Image
                {
                    Name = definition.name,
                    IsHitTestVisible = false,
                    Focusable = false
                };

                var sourceToken = JValue.CreateString(definition.source);
                if (!SkinPropertyRegistry.TryPrepare(
                        image,
                        "source",
                        sourceToken,
                        $"$.dynamicImages[{i}].source",
                        source => ResolveAssetPath(package.DirectoryName, source),
                        out PreparedSkinProperty? sourceProperty,
                        out string? sourceError))
                {
                    return SkinApplyResult.Failed($"$.dynamicImages[{i}].source", sourceError ?? "图片加载失败", prepared.Warnings);
                }
                image.SetValue(sourceProperty!.Property, sourceProperty.Value);

                foreach (JProperty property in definition.properties.Properties())
                {
                    string path = $"$.dynamicImages[{i}].properties.{property.Name}";
                    if (!SkinPropertyRegistry.TryPrepare(
                            image,
                            property.Name,
                            property.Value,
                            path,
                            source => ResolveAssetPath(package.DirectoryName, source),
                            out PreparedSkinProperty? imageProperty,
                            out string? error))
                    {
                        return SkinApplyResult.Failed(path, error ?? "属性转换失败", prepared.Warnings);
                    }
                    image.SetValue(imageProperty!.Property, imageProperty.Value);
                }

                prepared.DynamicImages.Add((host, image));
            }

            return SkinApplyResult.Succeeded(prepared.Warnings);
        }

        private static SkinApplyResult ResolveTarget(
            SkinTreeIndex index,
            string targetName,
            bool isLegacy,
            out FrameworkElement? target,
            out string? warning)
        {
            target = null;
            warning = null;
            int separatorIndex = targetName.IndexOf('/');

            List<FrameworkElement>? matches;
            if (separatorIndex >= 0)
            {
                string hostName = targetName[..separatorIndex];
                string elementName = targetName[(separatorIndex + 1)..];
                if (!index.HostElements.TryGetValue(hostName, out Dictionary<string, List<FrameworkElement>>? hostElements))
                    return SkinApplyResult.Failed($"$.elements['{targetName}']", $"未知宿主：{hostName}");
                hostElements.TryGetValue(elementName, out matches);
            }
            else
            {
                index.NamedElements.TryGetValue(targetName, out matches);
            }

            if (matches == null || matches.Count == 0)
            {
                if (isLegacy)
                {
                    warning = $"旧版皮肤元素不存在，已跳过：{targetName}";
                    return SkinApplyResult.Succeeded([warning]);
                }
                return SkinApplyResult.Failed($"$.elements['{targetName}']", $"元素不存在：{targetName}");
            }

            if (matches.Count > 1)
            {
                if (isLegacy)
                {
                    warning = $"旧版皮肤元素名称不唯一，已跳过：{targetName}";
                    return SkinApplyResult.Succeeded([warning]);
                }
                return SkinApplyResult.Failed($"$.elements['{targetName}']", $"元素名称不唯一，请使用 宿主/元素名：{targetName}");
            }

            target = matches[0];
            return SkinApplyResult.Succeeded();
        }

        private void ApplyPreparedApplication(PreparedApplication prepared)
        {
            foreach (PreparedSkinProperty property in prepared.Properties)
            {
                var key = (property.Target, property.Property);
                if (!_originalValues.ContainsKey(key))
                {
                    _originalValues[key] = new OriginalPropertyValue(
                        property.Target.ReadLocalValue(property.Property),
                        BindingOperations.GetBindingBase(property.Target, property.Property));
                }

                property.Target.SetValue(property.Property, property.Value);
            }

            foreach ((Panel host, Image image) in prepared.DynamicImages)
            {
                host.Children.Add(image);
                _activeDynamicImages.Add((host, image));
            }
        }

        private void RestoreOriginalState()
        {
            foreach ((Panel host, Image image) in _activeDynamicImages.ToArray())
                host.Children.Remove(image);
            _activeDynamicImages.Clear();

            foreach (((DependencyObject target, DependencyProperty property), OriginalPropertyValue original) in _originalValues)
                SkinPropertyRegistry.RestoreOriginalValue(target, property, original.LocalValue, original.Binding);
        }

        private SkinTreeIndex BuildTreeIndex(Window window)
        {
            var index = new SkinTreeIndex();
            var excludedElements = _activeDynamicImages
                .Select(item => (DependencyObject)item.Image)
                .ToHashSet();
            CollectNamedElements(window, index.NamedElements, excludedElements);
            CollectHosts(window, index, excludedElements);
            return index;
        }

        private static void CollectNamedElements(
            DependencyObject parent,
            Dictionary<string, List<FrameworkElement>> elements,
            HashSet<DependencyObject> excludedElements)
        {
            if (!excludedElements.Contains(parent) && parent is FrameworkElement element && !string.IsNullOrWhiteSpace(element.Name))
            {
                if (!elements.TryGetValue(element.Name, out List<FrameworkElement>? matches))
                {
                    matches = [];
                    elements[element.Name] = matches;
                }
                matches.Add(element);
            }

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
                CollectNamedElements(VisualTreeHelper.GetChild(parent, i), elements, excludedElements);
        }

        private static void CollectHosts(DependencyObject parent, SkinTreeIndex index, HashSet<DependencyObject> excludedElements)
        {
            if (parent is Panel panel)
            {
                string hostName = SkinHost.GetName(panel);
                if (!string.IsNullOrWhiteSpace(hostName))
                {
                    index.HostPanels[hostName] = panel;
                    var hostElements = new Dictionary<string, List<FrameworkElement>>(StringComparer.Ordinal);
                    CollectNamedElements(panel, hostElements, excludedElements);
                    index.HostElements[hostName] = hostElements;
                }
            }

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
                CollectHosts(VisualTreeHelper.GetChild(parent, i), index, excludedElements);
        }

        private string? ResolveAssetPath(string skinName, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            string path;
            if (relativePath.StartsWith('/'))
            {
                path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            }
            else if (Path.IsPathRooted(relativePath))
            {
                path = relativePath;
            }
            else
            {
                path = Path.Combine(_skinsDirectory, skinName, relativePath.Replace('/', Path.DirectorySeparatorChar));
            }

            return Path.GetFullPath(path);
        }

        private void EnsureSkinsDirectoryExists()
        {
            Directory.CreateDirectory(_skinsDirectory);
            string defaultSkinPath = Path.Combine(_skinsDirectory, _defaultSkinName);
            Directory.CreateDirectory(defaultSkinPath);

            string configPath = Path.Combine(defaultSkinPath, "skin.json");
            if (File.Exists(configPath))
                return;

            var defaultSkin = new SkinDefinition
            {
                name = _defaultSkinName,
                displayName = "默认皮肤",
                desc = "工具箱的默认皮肤。",
                author = "LLC_MOD_Toolbox",
                version = "1.0.0"
            };
            File.WriteAllText(configPath, JsonConvert.SerializeObject(defaultSkin, Formatting.Indented));
        }

        private SkinPackage CreateEmptyDefaultPackage()
        {
            return new SkinPackage
            {
                DirectoryName = _defaultSkinName,
                ConfigPath = Path.Combine(_skinsDirectory, _defaultSkinName, "skin.json"),
                Definition = new SkinDefinition
                {
                    name = _defaultSkinName,
                    displayName = "默认皮肤",
                    author = "LLC_MOD_Toolbox"
                }
            };
        }
    }
}
