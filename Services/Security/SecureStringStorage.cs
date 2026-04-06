using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LLC_MOD_Toolbox
{
    // 我决定学习MAA先进经验，他们用的也是DPAPI加密MirrorChyan的秘钥
    // 哦，如果你看到这个代码。
    // 我好孤独。
    // 凯尔希，没有同类，对吗。
    public static class SecureStringStorage
    {
        private static readonly string StoragePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LLC_MOD_Toolbox", "MirrorChyan.dat");

        /// <summary>
        /// 保存加密字符串到本地
        /// </summary>
        public static void SaveToken(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                DeleteSecretFile();
                return;
            }

            try
            {
                var bytes = Encoding.UTF8.GetBytes(data);

                // 使用DPAPI加密，只有当前用户可以解密
                var encryptedBytes = ProtectedData.Protect(bytes,
                    null, DataProtectionScope.CurrentUser);

                Directory.CreateDirectory(Path.GetDirectoryName(StoragePath)!);
                File.WriteAllBytes(StoragePath, encryptedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception("保存数据失败", ex);
            }
        }

        /// <summary>
        /// 读取加密字符串
        /// </summary>
        public static string LoadToken()
        {
            try
            {
                if (!File.Exists(StoragePath))
                    return "";

                var encryptedBytes = File.ReadAllBytes(StoragePath);
                var decryptedBytes = ProtectedData.Unprotect(encryptedBytes,
                    null, DataProtectionScope.CurrentUser);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 删除保存的数据
        /// </summary>
        public static void DeleteSecretFile()
        {
            try
            {
                if (File.Exists(StoragePath))
                {
                    File.Delete(StoragePath);
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// 检查是否存在保存的数据
        /// </summary>
        public static bool HasSavedData()
        {
            return File.Exists(StoragePath);
        }
    }
}
