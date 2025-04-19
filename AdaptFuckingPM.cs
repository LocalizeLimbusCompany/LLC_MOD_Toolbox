using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    /// <summary>
    /// 尝试修复：1.74.0版本的适配问题
    /// </summary>
    public static class AdaptFuckingPM
    {
        public static void CheckAdapt(string LimbusCompanyPath)
        {
            string fontPath = Path.Combine(LimbusCompanyPath, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font");
            string fontFile = Path.Combine(fontPath, "ChineseFont.ttf");
            if (!Path.Exists(fontPath)) return;
            if (!File.Exists(fontFile)) return;
            string contextPath = Path.Combine(fontPath, "Context");
            string titlePath = Path.Combine(fontPath, "Title");
            if (Path.Exists(contextPath) && Path.Exists(titlePath)) return;
            Log.logger.Info("发现旧版本版本文件夹。");
            MessageBoxResult result = MessageBox.Show("警告！\n发现您可能使用了旧版本的汉化补丁结构，该结构会导致汉化失效。\n按“确定”尝试修复，如果您确定这不是一个问题，请按“取消”。", "适配警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel)
            {
                Log.logger.Info("用户取消适配。");
                return;
            }
            else
            {
                Log.logger.Info("开始适配。");
                TryFixAdaptFont(LimbusCompanyPath);
            }
        }
        public static void TryFixAdaptFont(string LimbusCompanyPath)
        {
            string fontPath = Path.Combine(LimbusCompanyPath, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font");
            string fontFile = Path.Combine(fontPath, "ChineseFont.ttf");
            Directory.CreateDirectory(Path.Combine(LimbusCompanyPath, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context"));
            Directory.CreateDirectory(Path.Combine(LimbusCompanyPath, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Title"));
            File.Move(fontFile, Path.Combine(fontPath, "Context", "ChineseFont.ttf"));
            Log.logger.Info("适配成功。");
            MessageBox.Show("适配成功！", "适配完成", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
