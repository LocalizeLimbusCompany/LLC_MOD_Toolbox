using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    public static class SimpleDnsChecker
    {
        public static async Task CheckDNS()
        {
            bool result = await CheckForSuspiciousIpAsync("www.zeroasso.top");
            if (result)
            {
                MessageBox.Show("警告！\n检测到您的DNS解析结果存在问题。\n您大概率无法使用工具箱。\n建议您更换DNS服务器后再使用工具箱。\n如果您不知道使用什么DNS服务器，请使用阿里云DNS。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public static async Task<bool> CheckForSuspiciousIpAsync(string domain)
        {
            try
            {
                IPAddress[] addresses = await Dns.GetHostAddressesAsync(domain);

                foreach (var ip in addresses)
                {
                    if (IsLocalhost(ip) || IsPrivateNetwork(ip))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsLocalhost(IPAddress ip)
        {
            return IPAddress.IsLoopback(ip);
        }

        private static bool IsPrivateNetwork(IPAddress ip)
        {
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return ip.IsIPv6SiteLocal || ip.IsIPv6LinkLocal;
            }

            byte[] bytes = ip.GetAddressBytes();

            if (bytes[0] == 10)
                return true;

            if (bytes[0] == 172 && (bytes[1] >= 16 && bytes[1] <= 31))
                return true;

            if (bytes[0] == 192 && bytes[1] == 168)
                return true;

            return false;
        }
    }
}
