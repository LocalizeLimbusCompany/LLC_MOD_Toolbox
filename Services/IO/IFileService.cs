using SevenZip;
using System.IO;
using System.Security.Cryptography;

namespace LLC_MOD_Toolbox.Services.IO
{
    public interface IFileService
    {
        void DeleteFile(string path);
        void DeleteDirectory(string path);
        void ExtractArchive(string archivePath, string outputDir);
        string CalculateSHA256(string filePath);
    }
}
