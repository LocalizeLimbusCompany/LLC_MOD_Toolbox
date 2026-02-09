using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LLC_MOD_Toolbox
{
    public class SkinManager
    {
        private static SkinManager _instance;
        private static readonly object _lock = new object();

        private readonly string _skinsDirectory;
        private readonly string _defaultSkinName = "default";
        private Dictionary<string, string> _currentSkinImages = new Dictionary<string, string>();
        private Dictionary<string, string> _defaultSkinImages = new Dictionary<string, string>();
        private Dictionary<string, bool> _currentSkinVisibility = new Dictionary<string, bool>();
        private Dictionary<string, bool> _defaultSkinVisibility = new Dictionary<string, bool>();
        private Dictionary<string, string> _currentSkinMargins = new Dictionary<string, string>();
        private Dictionary<string, string> _defaultSkinMargins = new Dictionary<string, string>();
        private SkinDefinition _currentSkinDefinition;
        private SkinDefinition _defaultSkinDefinition;

        public static SkinManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SkinManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private SkinManager()
        {
            _skinsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skins");
            EnsureSkinsDirectoryExists();
            LoadDefaultSkin();
        }

        private void EnsureSkinsDirectoryExists()
        {
            if (!Directory.Exists(_skinsDirectory))
            {
                Directory.CreateDirectory(_skinsDirectory);
            }

            string defaultSkinPath = Path.Combine(_skinsDirectory, _defaultSkinName);
            if (!Directory.Exists(defaultSkinPath))
            {
                Directory.CreateDirectory(defaultSkinPath);
                CreateDefaultSkinConfig(defaultSkinPath);
            }
        }

        private void CreateDefaultSkinConfig(string skinPath)
        {
            var defaultSkin = new SkinDefinition
            {
                name = "default",
                displayName = "默认皮肤",
                desc = "工具箱的默认皮肤。",
                author = "LLC_MOD_Toolbox",
                version = "1.0.0",
                images = new Dictionary<string, string>()
            };

            string configPath = Path.Combine(skinPath, "skin.json");
            File.WriteAllText(configPath, JsonConvert.SerializeObject(defaultSkin, Formatting.Indented));
        }

        private void LoadDefaultSkin()
        {
            try
            {
                string defaultSkinPath = Path.Combine(_skinsDirectory, _defaultSkinName);
                string configPath = Path.Combine(defaultSkinPath, "skin.json");

                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    _defaultSkinDefinition = JsonConvert.DeserializeObject<SkinDefinition>(json);
                    _defaultSkinImages = _defaultSkinDefinition?.images ?? new Dictionary<string, string>();
                    _defaultSkinVisibility = _defaultSkinDefinition?.visibility ?? new Dictionary<string, bool>();
                    _defaultSkinMargins = _defaultSkinDefinition?.margins ?? new Dictionary<string, string>();
                }
                else
                {
                    _defaultSkinImages = new Dictionary<string, string>();
                    _defaultSkinVisibility = new Dictionary<string, bool>();
                    _defaultSkinMargins = new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error($"加载默认皮肤失败: {ex.Message}");
                _defaultSkinImages = new Dictionary<string, string>();
                _defaultSkinVisibility = new Dictionary<string, bool>();
                _defaultSkinMargins = new Dictionary<string, string>();
            }
        }

        public bool LoadSkin(string skinName)
        {
            try
            {
                string skinPath = Path.Combine(_skinsDirectory, skinName);
                string configPath = Path.Combine(skinPath, "skin.json");

                if (!Directory.Exists(skinPath))
                {
                    Log.logger.Warn($"皮肤目录不存在: {skinName}，回退到默认皮肤");
                    return LoadSkin(_defaultSkinName);
                }

                if (!File.Exists(configPath))
                {
                    Log.logger.Warn($"皮肤配置文件不存在: {skinName}，回退到默认皮肤");
                    return LoadSkin(_defaultSkinName);
                }

                string json = File.ReadAllText(configPath);
                _currentSkinDefinition = JsonConvert.DeserializeObject<SkinDefinition>(json);

                if (_currentSkinDefinition == null || _currentSkinDefinition.images == null)
                {
                    Log.logger.Warn($"皮肤配置无效: {skinName}，回退到默认皮肤");
                    return LoadSkin(_defaultSkinName);
                }

                _currentSkinImages = _currentSkinDefinition.images;
                _currentSkinVisibility = _currentSkinDefinition.visibility ?? new Dictionary<string, bool>();
                _currentSkinMargins = _currentSkinDefinition.margins ?? new Dictionary<string, string>();
                Log.logger.Info($"成功加载皮肤: {_currentSkinDefinition.displayName} v{_currentSkinDefinition.version} by {_currentSkinDefinition.author}");
                return true;
            }
            catch (Exception ex)
            {
                Log.logger.Error($"加载皮肤失败: {skinName}, 错误: {ex.Message}");
                if (skinName != _defaultSkinName)
                {
                    Log.logger.Info("尝试回退到默认皮肤");
                    return LoadSkin(_defaultSkinName);
                }
                return false;
            }
        }

        public void ApplySkinToWindow(Window window)
        {
            if (window == null)
            {
                Log.logger.Warn("窗口对象为空，无法应用皮肤");
                return;
            }

            try
            {
                var images = FindAllImages(window);
                int successCount = 0;
                int failCount = 0;

                foreach (var image in images)
                {
                    if (string.IsNullOrEmpty(image.Name))
                        continue;

                    if (ApplySkinToImage(image))
                        successCount++;
                    else
                        failCount++;
                }

                Log.logger.Info($"皮肤应用完成: 成功 {successCount} 个，失败 {failCount} 个");

                // 应用可见性设置
                ApplyVisibilityToWindow(window);

                // 应用边距设置
                ApplyMarginsToWindow(window);
            }
            catch (Exception ex)
            {
                Log.logger.Error($"应用皮肤到窗口时出错: {ex.Message}");
            }
        }

        private bool ApplySkinToImage(Image image)
        {
            if (image == null || string.IsNullOrEmpty(image.Name))
            {
                return false;
            }

            try
            {
                string imageName = image.Name;
                string imagePath = null;
                bool hasConfig = false;

                // 尝试从当前皮肤获取图片路径
                if (_currentSkinImages.ContainsKey(imageName))
                {
                    hasConfig = true;
                    imagePath = GetFullImagePath(_currentSkinDefinition.name, _currentSkinImages[imageName]);
                }

                // 如果当前皮肤没有或文件不存在，尝试从默认皮肤获取
                if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                {
                    if (_defaultSkinImages.ContainsKey(imageName))
                    {
                        hasConfig = true;
                        imagePath = GetFullImagePath(_defaultSkinName, _defaultSkinImages[imageName]);
                    }
                }

                // 如果皮肤配置中没有这个图片，跳过（使用 XAML 中的默认资源）
                if (!hasConfig)
                {
                    return false;
                }

                // 如果找到了有效的图片路径，应用它
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    bitmap.Freeze();

                    image.Source = bitmap;
                    return true;
                }
                else
                {
                    Log.logger.Warn($"皮肤配置了图片 '{imageName}' 但文件不存在: {imagePath}");
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error($"应用皮肤到图片 {image.Name} 时出错: {ex.Message}");
            }
            return false;
        }

        private string GetFullImagePath(string skinName, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            // 处理以 / 开头的路径（相对于应用程序基目录）
            if (relativePath.StartsWith("/"))
            {
                string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(appBasePath, relativePath.TrimStart('/').Replace('/', '\\'));
            }

            // 如果是绝对路径（如 C:\...），直接返回
            if (Path.IsPathRooted(relativePath))
                return relativePath;

            // 否则，构建相对于皮肤目录的路径
            string skinPath = Path.Combine(_skinsDirectory, skinName);
            return Path.Combine(skinPath, relativePath);
        }

        private List<Image> FindAllImages(DependencyObject parent)
        {
            var images = new List<Image>();

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Image image)
                {
                    images.Add(image);
                }

                images.AddRange(FindAllImages(child));
            }

            return images;
        }

        private void ApplyVisibilityToWindow(Window window)
        {
            try
            {
                window.Dispatcher.BeginInvoke(() =>
                {
                    var elements = FindAllNamedElements(window);
                    int visibleCount = 0;
                    int hiddenCount = 0;

                    foreach (var element in elements)
                    {
                        if (string.IsNullOrEmpty(element.Name))
                            continue;

                        bool? visibility = GetElementVisibility(element.Name);
                        if (visibility.HasValue)
                        {
                            element.Visibility = visibility.Value ? Visibility.Visible : Visibility.Collapsed;
                            if (visibility.Value)
                                visibleCount++;
                            else
                                hiddenCount++;
                        }
                    }

                    if (visibleCount > 0 || hiddenCount > 0)
                    {
                        Log.logger.Info($"可见性设置应用完成: 显示 {visibleCount} 个，隐藏 {hiddenCount} 个");
                    }
                }).Wait();
            }
            catch (Exception ex)
            {
                Log.logger.Error($"应用可见性设置时出错: {ex.Message}");
            }
        }

        private bool? GetElementVisibility(string elementName)
        {
            // 优先使用当前皮肤的可见性设置
            if (_currentSkinVisibility.ContainsKey(elementName))
            {
                return _currentSkinVisibility[elementName];
            }

            // 如果当前皮肤没有设置，尝试使用默认皮肤的设置
            if (_defaultSkinVisibility.ContainsKey(elementName))
            {
                return _defaultSkinVisibility[elementName];
            }

            // 如果都没有设置，返回null表示不改变
            return null;
        }

        private List<FrameworkElement> FindAllNamedElements(DependencyObject parent)
        {
            var elements = new List<FrameworkElement>();

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is FrameworkElement element && !string.IsNullOrEmpty(element.Name))
                {
                    elements.Add(element);
                }

                elements.AddRange(FindAllNamedElements(child));
            }

            return elements;
        }

        private void ApplyMarginsToWindow(Window window)
        {
            try
            {
                window.Dispatcher.BeginInvoke(() =>
                {
                    var elements = FindAllNamedElements(window);
                    int appliedCount = 0;

                    foreach (var element in elements)
                    {
                        if (string.IsNullOrEmpty(element.Name))
                            continue;

                        string marginValue = GetElementMargin(element.Name);
                        if (!string.IsNullOrEmpty(marginValue))
                        {
                            if (TryParseMargin(marginValue, out Thickness margin))
                            {
                                element.Margin = margin;
                                appliedCount++;
                            }
                            else
                            {
                                Log.logger.Warn($"无法解析边距值: {element.Name} = {marginValue}");
                            }
                        }
                    }

                    if (appliedCount > 0)
                    {
                        Log.logger.Info($"边距设置应用完成: 应用 {appliedCount} 个");
                    }
                }).Wait();
            }
            catch (Exception ex)
            {
                Log.logger.Error($"应用边距设置时出错: {ex.Message}");
            }
        }

        private string GetElementMargin(string elementName)
        {
            // 优先使用当前皮肤的边距设置
            if (_currentSkinMargins.ContainsKey(elementName))
            {
                return _currentSkinMargins[elementName];
            }

            // 如果当前皮肤没有设置，尝试使用默认皮肤的设置
            if (_defaultSkinMargins.ContainsKey(elementName))
            {
                return _defaultSkinMargins[elementName];
            }

            // 如果都没有设置，返回null表示不改变
            return null;
        }

        private bool TryParseMargin(string marginValue, out Thickness margin)
        {
            margin = new Thickness();

            if (string.IsNullOrWhiteSpace(marginValue))
                return false;

            try
            {
                var parts = marginValue.Split(',');

                if (parts.Length == 1)
                {
                    // 单个值：所有边相同
                    double uniform = double.Parse(parts[0].Trim());
                    margin = new Thickness(uniform);
                    return true;
                }
                else if (parts.Length == 2)
                {
                    // 两个值：左右, 上下
                    double horizontal = double.Parse(parts[0].Trim());
                    double vertical = double.Parse(parts[1].Trim());
                    margin = new Thickness(horizontal, vertical, horizontal, vertical);
                    return true;
                }
                else if (parts.Length == 4)
                {
                    // 四个值：左, 上, 右, 下
                    double left = double.Parse(parts[0].Trim());
                    double top = double.Parse(parts[1].Trim());
                    double right = double.Parse(parts[2].Trim());
                    double bottom = double.Parse(parts[3].Trim());
                    margin = new Thickness(left, top, right, bottom);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error($"解析边距值失败: {marginValue}, 错误: {ex.Message}");
            }

            return false;
        }

        public List<string> GetAvailableSkins()
        {
            try
            {
                if (!Directory.Exists(_skinsDirectory))
                    return new List<string>();

                return Directory.GetDirectories(_skinsDirectory)
                    .Select(Path.GetFileName)
                    .Where(name => File.Exists(Path.Combine(_skinsDirectory, name, "skin.json")))
                    .ToList();
            }
            catch (Exception ex)
            {
                Log.logger.Error($"获取可用皮肤列表失败: {ex.Message}");
                return new List<string>();
            }
        }

        public SkinDefinition GetSkinInfo(string skinName)
        {
            try
            {
                string configPath = Path.Combine(_skinsDirectory, skinName, "skin.json");
                if (!File.Exists(configPath))
                    return null;

                string json = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<SkinDefinition>(json);
            }
            catch (Exception ex)
            {
                Log.logger.Error($"获取皮肤信息失败: {skinName}, 错误: {ex.Message}");
                return null;
            }
        }

        public string CurrentSkinName => _currentSkinDefinition?.name ?? _defaultSkinName;
        public SkinDefinition CurrentSkinInfo => _currentSkinDefinition;
    }
}
