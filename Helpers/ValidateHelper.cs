using System.IO;
using System.Security.Cryptography;

namespace LLC_MOD_Toolbox.Helpers;

static class ValidateHelper
{
    public static async Task<bool> CheckHashAsync(Stream archive, string onlineHash)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hash = await sha256.ComputeHashAsync(archive);
        return Convert.ToHexString(hash).Equals(onlineHash, StringComparison.OrdinalIgnoreCase);
    }
}
