using System.IO;

namespace LLC_MOD_Toolbox.Services.Font
{
    public interface IFontService
    {
        bool IsValidFontFile(string filePath);
        string GetFontFamilyName(string filePath);
        void ApplyFontReplacement(string fontFilePath, string limbusDir);
        void RestoreOriginalFont(string limbusDir);
    }

    public sealed class FontService : IFontService
    {
        public bool IsValidFontFile(string filePath)
        {
            if (filePath == "输入字体路径")
                return false;
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".ttf" && extension != ".otf" && extension != ".ttc")
                return false;
            if (!File.Exists(filePath))
                return false;
            try
            {
                using var fontCollection = new System.Drawing.Text.PrivateFontCollection();
                fontCollection.AddFontFile(filePath);
                return fontCollection.Families.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public string GetFontFamilyName(string filePath)
        {
            try
            {
                using var fontCollection = new System.Drawing.Text.PrivateFontCollection();
                fontCollection.AddFontFile(filePath);
                if (fontCollection.Families.Length > 0)
                    return fontCollection.Families[0].Name;
            }
            catch { }
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public void ApplyFontReplacement(string fontFilePath, string limbusDir)
        {
            string oldFontPath = Path.Combine(limbusDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.ttf");
            string oldOTFFontPath = Path.Combine(limbusDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.otf");
            string backupFontPath = Path.Combine(limbusDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont", "ChineseFont.ttf.bak");

            if (!File.Exists(oldFontPath) && !File.Exists(backupFontPath))
                throw new InvalidOperationException("请先安装汉化，然后再进行字体替换。");

            if (File.Exists(oldFontPath) && !File.Exists(backupFontPath))
            {
                Log.logger.Info("正在备份原字体文件。");
                Directory.CreateDirectory(Path.Combine(limbusDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont"));
                File.Move(oldFontPath, backupFontPath);
            }
            if (File.Exists(oldFontPath))
            {
                Log.logger.Info("正在删除原TTF字体文件。");
                File.Delete(oldFontPath);
            }
            if (File.Exists(oldOTFFontPath))
            {
                Log.logger.Info("正在删除原OTF字体文件。");
                File.Delete(oldOTFFontPath);
            }

            Log.logger.Info("正在替换字体文件。");
            string extension = new FileInfo(fontFilePath).Extension;
            File.Copy(fontFilePath, Path.Combine(limbusDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", $"ChineseFont{extension}"), true);
            Log.logger.Info("字体替换成功。");
        }

        public void RestoreOriginalFont(string limbusDir)
        {
            string backupFontPath = Path.Combine(limbusDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont", "ChineseFont.ttf.bak");
            if (!File.Exists(backupFontPath))
                throw new FileNotFoundException("没有找到备份字体文件。");

            string oldFontTTFPath = Path.Combine(limbusDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.ttf");
            string oldFontOTFPath = Path.Combine(limbusDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.otf");
            if (File.Exists(oldFontTTFPath))
                File.Delete(oldFontTTFPath);
            if (File.Exists(oldFontOTFPath))
                File.Delete(oldFontOTFPath);
            File.Move(backupFontPath, oldFontTTFPath);
            Log.logger.Info("字体还原成功。");
        }
    }
}
