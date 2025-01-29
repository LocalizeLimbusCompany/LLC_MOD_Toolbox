using System.IO;
using System.Security.Cryptography;

namespace LLC_MOD_Toolbox.Helpers;

internal static class ValidateHelper
{
    public static async Task<bool> CheckHashAsync(Stream archive, string onlineHash)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hash = await sha256.ComputeHashAsync(archive);
        return Convert.ToHexString(hash).Equals(onlineHash, StringComparison.OrdinalIgnoreCase);
    }

    public static bool CheckMelonloader(string path)
    {
        string melonloaderPath = Path.Combine(path, "version.dll");
        return File.Exists(melonloaderPath);
    }
}
