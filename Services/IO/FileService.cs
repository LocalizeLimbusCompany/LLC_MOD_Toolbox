using SevenZip;
using System.IO;
using System.Security.Cryptography;

namespace LLC_MOD_Toolbox.Services.IO
{
    public sealed class FileService : IFileService
    {
        public void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                Log.logger.Info("删除文件： " + path);
                File.Delete(path);
            }
            else
            {
                Log.logger.Info("文件不存在： " + path);
            }
        }

        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Log.logger.Info("删除目录： " + path);
                Directory.Delete(path, true);
            }
            else
            {
                Log.logger.Info("目录不存在： " + path);
            }
        }

        public void ExtractArchive(string archivePath, string outputDir)
        {
            using SevenZipExtractor extractor = new(archivePath);
            extractor.ExtractArchive(outputDir);
        }

        public string CalculateSHA256(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var fileStream = File.OpenRead(filePath);
            byte[] hashBytes = sha256.ComputeHash(fileStream);
            string result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            Log.logger.Info($"计算位置为 {filePath} 的文件的Hash结果为：{result}");
            return result;
        }
    }
}
