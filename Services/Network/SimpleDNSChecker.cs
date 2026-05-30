using System.Net;
using System.Net.Sockets;

namespace LLC_MOD_Toolbox.Services.Network
{
    public static class SimpleDnsChecker
    {
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
