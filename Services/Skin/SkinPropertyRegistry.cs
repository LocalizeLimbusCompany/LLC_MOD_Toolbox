using LLC_MOD_Toolbox.Views.Controls;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LLC_MOD_Toolbox.Services.Skin
{
    internal sealed record PreparedSkinProperty(
        DependencyObject Target,
        DependencyProperty Property,
        object Value,
        string JsonPath);

    internal static class SkinPropertyRegistry
    {
        private static readonly HashSet<string> SupportedProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            "width", "height", "minWidth", "minHeight", "maxWidth", "maxHeight",
            "margin", "padding", "horizontalAlignment", "verticalAlignment", "opacity", "visibility",
            "zIndex", "gridRow", "gridColumn", "gridRowSpan", "gridColumnSpan",
            "fontFamily", "fontSize", "fontWeight", "fontStyle", "foreground", "background",
            "borderBrush", "borderThickness", "source", "stretch", "stretchDirection",
            "fill", "stroke", "strokeThickness", "textAlignment", "content"
        };

        public static bool IsSupported(string propertyName) => SupportedProperties.Contains(propertyName);

        public static bool TryPrepare(
            FrameworkElement target,
            string propertyName,
            JToken token,
            string jsonPath,
            Func<string, string?> resolveAssetPath,
            out PreparedSkinProperty? prepared,
            out string? error)
        {
            prepared = null;
            error = null;

            try
            {
                DependencyProperty? dependencyProperty = null;
                object? value = null;

                switch (propertyName.ToLowerInvariant())
                {
                    case "width":
                        dependencyProperty = FrameworkElement.WidthProperty;
                        value = ReadNonNegativeDouble(token);
                        break;
                    case "height":
                        dependencyProperty = FrameworkElement.HeightProperty;
                        value = ReadNonNegativeDouble(token);
                        break;
                    case "minwidth":
                        dependencyProperty = FrameworkElement.MinWidthProperty;
                        value = ReadNonNegativeDouble(token);
                        break;
                    case "minheight":
                        dependencyProperty = FrameworkElement.MinHeightProperty;
                        value = ReadNonNegativeDouble(token);
                        break;
                    case "maxwidth":
                        dependencyProperty = FrameworkElement.MaxWidthProperty;
                        value = ReadNonNegativeDouble(token);
                        break;
                    case "maxheight":
                        dependencyProperty = FrameworkElement.MaxHeightProperty;
                        value = ReadNonNegativeDouble(token);
                        break;
                    case "margin":
                        dependencyProperty = FrameworkElement.MarginProperty;
                        value = ReadThickness(token);
                        break;
                    case "padding":
                        (dependencyProperty, value) = target switch
                        {
                            Border => (Border.PaddingProperty, ReadThickness(token)),
                            TextBlock => (TextBlock.PaddingProperty, ReadThickness(token)),
                            Control => (Control.PaddingProperty, ReadThickness(token)),
                            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 padding")
                        };
                        break;
                    case "horizontalalignment":
                        dependencyProperty = FrameworkElement.HorizontalAlignmentProperty;
                        value = ReadEnum<HorizontalAlignment>(token);
                        break;
                    case "verticalalignment":
                        dependencyProperty = FrameworkElement.VerticalAlignmentProperty;
                        value = ReadEnum<VerticalAlignment>(token);
                        break;
                    case "opacity":
                        dependencyProperty = UIElement.OpacityProperty;
                        value = ReadRangeDouble(token, 0, 1);
                        break;
                    case "visibility":
                        dependencyProperty = UIElement.VisibilityProperty;
                        value = ReadVisibility(token);
                        break;
                    case "zindex":
                        dependencyProperty = Panel.ZIndexProperty;
                        value = ReadInt(token);
                        break;
                    case "gridrow":
                        dependencyProperty = Grid.RowProperty;
                        value = ReadNonNegativeInt(token);
                        break;
                    case "gridcolumn":
                        dependencyProperty = Grid.ColumnProperty;
                        value = ReadNonNegativeInt(token);
                        break;
                    case "gridrowspan":
                        dependencyProperty = Grid.RowSpanProperty;
                        value = ReadPositiveInt(token);
                        break;
                    case "gridcolumnspan":
                        dependencyProperty = Grid.ColumnSpanProperty;
                        value = ReadPositiveInt(token);
                        break;
                    case "fontfamily":
                        dependencyProperty = GetFontFamilyProperty(target);
                        value = new FontFamily(ReadString(token));
                        break;
                    case "fontsize":
                        dependencyProperty = GetFontSizeProperty(target);
                        value = ReadPositiveDouble(token);
                        break;
                    case "fontweight":
                        dependencyProperty = GetFontWeightProperty(target);
                        value = new FontWeightConverter().ConvertFromString(null, CultureInfo.InvariantCulture, ReadString(token))
                            ?? throw new FormatException("无法解析 fontWeight");
                        break;
                    case "fontstyle":
                        dependencyProperty = GetFontStyleProperty(target);
                        value = new FontStyleConverter().ConvertFromString(null, CultureInfo.InvariantCulture, ReadString(token))
                            ?? throw new FormatException("无法解析 fontStyle");
                        break;
                    case "foreground":
                        dependencyProperty = GetForegroundProperty(target);
                        value = ReadBrush(token);
                        break;
                    case "background":
                        dependencyProperty = GetBackgroundProperty(target);
                        value = ReadBrush(token);
                        break;
                    case "borderbrush":
                        dependencyProperty = target switch
                        {
                            Border => Border.BorderBrushProperty,
                            Control => Control.BorderBrushProperty,
                            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 borderBrush")
                        };
                        value = ReadBrush(token);
                        break;
                    case "borderthickness":
                        dependencyProperty = target switch
                        {
                            Border => Border.BorderThicknessProperty,
                            Control => Control.BorderThicknessProperty,
                            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 borderThickness")
                        };
                        value = ReadThickness(token);
                        break;
                    case "source":
                        if (target is not Image)
                            throw new InvalidOperationException($"{target.GetType().Name} 不支持 source");
                        dependencyProperty = Image.SourceProperty;
                        string source = ReadString(token);
                        string? fullPath = resolveAssetPath(source);
                        if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
                            throw new FileNotFoundException($"素材不存在：{source}", fullPath);
                        value = LoadBitmap(fullPath);
                        break;
                    case "stretch":
                        if (target is not Image)
                            throw new InvalidOperationException($"{target.GetType().Name} 不支持 stretch");
                        dependencyProperty = Image.StretchProperty;
                        value = ReadEnum<Stretch>(token);
                        break;
                    case "stretchdirection":
                        if (target is not Image)
                            throw new InvalidOperationException($"{target.GetType().Name} 不支持 stretchDirection");
                        dependencyProperty = Image.StretchDirectionProperty;
                        value = ReadEnum<StretchDirection>(token);
                        break;
                    case "fill":
                        EnsureOutlinedText(target, propertyName);
                        dependencyProperty = OutlinedTextControl.FillProperty;
                        value = ReadBrush(token);
                        break;
                    case "stroke":
                        EnsureOutlinedText(target, propertyName);
                        dependencyProperty = OutlinedTextControl.StrokeProperty;
                        value = ReadBrush(token);
                        break;
                    case "strokethickness":
                        EnsureOutlinedText(target, propertyName);
                        dependencyProperty = OutlinedTextControl.StrokeThicknessProperty;
                        value = ReadNonNegativeDouble(token);
                        break;
                    case "textalignment":
                        dependencyProperty = target switch
                        {
                            OutlinedTextControl => OutlinedTextControl.TextAlignmentProperty,
                            TextBlock => TextBlock.TextAlignmentProperty,
                            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 textAlignment")
                        };
                        value = ReadEnum<TextAlignment>(token);
                        break;
                    case "content":
                        if (target is not ContentControl)
                            throw new InvalidOperationException($"{target.GetType().Name} 不支持 content");
                        dependencyProperty = ContentControl.ContentProperty;
                        value = ReadRestrictedContent(token);
                        break;
                    default:
                        error = $"未知属性：{propertyName}";
                        return false;
                }

                prepared = new PreparedSkinProperty(target, dependencyProperty, value!, jsonPath);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static void RestoreOriginalValue(DependencyObject target, DependencyProperty property, object localValue, BindingBase? binding)
        {
            BindingOperations.ClearBinding(target, property);
            if (binding != null)
                BindingOperations.SetBinding(target, property, binding);
            else if (localValue == DependencyProperty.UnsetValue)
                target.ClearValue(property);
            else
                target.SetValue(property, localValue);
        }

        private static DependencyProperty GetFontFamilyProperty(FrameworkElement target) => target switch
        {
            OutlinedTextControl => OutlinedTextControl.FontFamilyProperty,
            TextBlock => TextBlock.FontFamilyProperty,
            Control => Control.FontFamilyProperty,
            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 fontFamily")
        };

        private static DependencyProperty GetFontSizeProperty(FrameworkElement target) => target switch
        {
            OutlinedTextControl => OutlinedTextControl.FontSizeProperty,
            TextBlock => TextBlock.FontSizeProperty,
            Control => Control.FontSizeProperty,
            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 fontSize")
        };

        private static DependencyProperty GetFontWeightProperty(FrameworkElement target) => target switch
        {
            OutlinedTextControl => OutlinedTextControl.FontWeightProperty,
            TextBlock => TextBlock.FontWeightProperty,
            Control => Control.FontWeightProperty,
            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 fontWeight")
        };

        private static DependencyProperty GetFontStyleProperty(FrameworkElement target) => target switch
        {
            OutlinedTextControl => OutlinedTextControl.FontStyleProperty,
            TextBlock => TextBlock.FontStyleProperty,
            Control => Control.FontStyleProperty,
            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 fontStyle")
        };

        private static DependencyProperty GetForegroundProperty(FrameworkElement target) => target switch
        {
            TextBlock => TextBlock.ForegroundProperty,
            Control => Control.ForegroundProperty,
            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 foreground")
        };

        private static DependencyProperty GetBackgroundProperty(FrameworkElement target) => target switch
        {
            Border => Border.BackgroundProperty,
            Panel => Panel.BackgroundProperty,
            TextBlock => TextBlock.BackgroundProperty,
            Control => Control.BackgroundProperty,
            _ => throw new InvalidOperationException($"{target.GetType().Name} 不支持 background")
        };

        private static void EnsureOutlinedText(FrameworkElement target, string propertyName)
        {
            if (target is not OutlinedTextControl)
                throw new InvalidOperationException($"{target.GetType().Name} 不支持 {propertyName}");
        }

        private static string ReadString(JToken token)
        {
            if (token.Type != JTokenType.String)
                throw new FormatException("必须是字符串");
            return token.Value<string>()!;
        }

        private static object? ReadRestrictedContent(JToken token)
        {
            return token.Type switch
            {
                JTokenType.Null => null,
                JTokenType.String => token.Value<string>() ?? string.Empty,
                _ => throw new FormatException("content 只能是字符串或 null")
            };
        }

        private static int ReadInt(JToken token)
        {
            if (token.Type != JTokenType.Integer)
                throw new FormatException("必须是整数");
            return token.Value<int>();
        }

        private static int ReadNonNegativeInt(JToken token)
        {
            int value = ReadInt(token);
            if (value < 0)
                throw new FormatException("不能小于 0");
            return value;
        }

        private static int ReadPositiveInt(JToken token)
        {
            int value = ReadInt(token);
            if (value <= 0)
                throw new FormatException("必须大于 0");
            return value;
        }

        private static double ReadDouble(JToken token)
        {
            if (token.Type is not (JTokenType.Integer or JTokenType.Float))
                throw new FormatException("必须是数字");
            double value = token.Value<double>();
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new FormatException("必须是有限数字");
            return value;
        }

        private static double ReadNonNegativeDouble(JToken token)
        {
            double value = ReadDouble(token);
            if (value < 0)
                throw new FormatException("不能小于 0");
            return value;
        }

        private static double ReadPositiveDouble(JToken token)
        {
            double value = ReadDouble(token);
            if (value <= 0)
                throw new FormatException("必须大于 0");
            return value;
        }

        private static double ReadRangeDouble(JToken token, double min, double max)
        {
            double value = ReadDouble(token);
            if (value < min || value > max)
                throw new FormatException($"必须在 {min} 到 {max} 之间");
            return value;
        }

        private static T ReadEnum<T>(JToken token) where T : struct, Enum
        {
            string value = ReadString(token);
            if (!Enum.TryParse(value, true, out T parsed))
                throw new FormatException($"无法解析 {typeof(T).Name}：{value}");
            return parsed;
        }

        private static Visibility ReadVisibility(JToken token)
        {
            if (token.Type == JTokenType.Boolean)
                return token.Value<bool>() ? Visibility.Visible : Visibility.Collapsed;

            string value = ReadString(token);
            if (bool.TryParse(value, out bool visible))
                return visible ? Visibility.Visible : Visibility.Collapsed;
            if (Enum.TryParse(value, true, out Visibility parsed))
                return parsed;
            throw new FormatException($"无法解析 Visibility：{value}");
        }

        private static Brush ReadBrush(JToken token)
        {
            string value = ReadString(token);
            var brush = new BrushConverter().ConvertFromString(null, CultureInfo.InvariantCulture, value) as Brush;
            if (brush == null)
                throw new FormatException($"无法解析颜色：{value}");
            if (brush.CanFreeze)
                brush.Freeze();
            return brush;
        }

        private static Thickness ReadThickness(JToken token)
        {
            double[] values;
            if (token is JArray array)
            {
                values = array.Select(ReadDouble).ToArray();
            }
            else if (token.Type == JTokenType.String)
            {
                values = ReadString(token)
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(value => double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture))
                    .ToArray();
            }
            else
            {
                throw new FormatException("Thickness 必须是字符串或 JSON 数组");
            }

            return values.Length switch
            {
                1 => new Thickness(values[0]),
                2 => new Thickness(values[0], values[1], values[0], values[1]),
                4 => new Thickness(values[0], values[1], values[2], values[3]),
                _ => throw new FormatException("Thickness 只能包含 1、2 或 4 个数字")
            };
        }

        private static BitmapImage LoadBitmap(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }
}
