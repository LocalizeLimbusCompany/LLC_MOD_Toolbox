using System.Runtime.InteropServices;

namespace LLC_MOD_Toolbox.Services.Compatibility
{
    /// <summary>
    /// 检测应用是否运行在 Wine / Proton / CrossOver 等 Windows 兼容层中。
    /// 主要用于在兼容层中禁用 WPF 硬件渲染（其 D3D/OpenGL 翻译路径不可用），
    /// 让 WPF 回退到软件渲染，从而在 CrossOver(macOS) 和 Wine(Linux) 下正常显示界面。
    /// </summary>
    internal static class WineDetector
    {
        private const string CrossOverRootVariable = "CX_ROOT";

        private static readonly Lazy<ProbeResult> Result = new(Probe);

        public static bool IsWine => Result.Value.IsWine;

        public static bool IsCrossOver => Result.Value.IsCrossOver;

        public static bool IsCrossOverOnMac => Result.Value.IsCrossOverOnMac;

        public static string Description => Result.Value.Description;

        private static ProbeResult Probe()
        {
            WineProbe wine = ProbeWine();
            string? cxRoot = Environment.GetEnvironmentVariable(CrossOverRootVariable);

            bool isCrossOver = !string.IsNullOrWhiteSpace(cxRoot);
            bool isCrossOverOnMac = isCrossOver &&
                                    cxRoot is not null &&
                                    cxRoot.Contains("/Applications/CrossOver.app", StringComparison.OrdinalIgnoreCase);

            // wine_get_version 是主判据；CX_ROOT 是 CrossOver 的兜底判据。
            bool isWine = wine.IsWine || isCrossOver;

            string description = $"Wine={isWine}, CrossOver={isCrossOver}, CrossOverMac={isCrossOverOnMac}, " +
                                 $"WineVersion={wine.Version ?? "?"}, CX_ROOT={cxRoot ?? "?"}";

            return new ProbeResult(isWine, isCrossOver, isCrossOverOnMac, description);
        }

        private static WineProbe ProbeWine()
        {
            IntPtr ntdll = LoadLibraryW("ntdll.dll");
            if (ntdll == IntPtr.Zero)
                return default;

            try
            {
                IntPtr getVersion = GetProcAddress(ntdll, "wine_get_version");
                if (getVersion == IntPtr.Zero)
                    return default;

                string? version = null;
                try
                {
                    IntPtr versionPointer = Marshal.GetDelegateForFunctionPointer<WineGetVersionDelegate>(getVersion)();
                    version = Marshal.PtrToStringAnsi(versionPointer);
                }
                catch
                {
                    // 获取版本字符串失败不影响 Wine 判定，只影响诊断日志。
                }

                return new WineProbe(true, version);
            }
            finally
            {
                FreeLibrary(ntdll);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr WineGetVersionDelegate();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibraryW(string lpLibFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        private readonly record struct WineProbe(bool IsWine, string? Version);

        private readonly record struct ProbeResult(
            bool IsWine,
            bool IsCrossOver,
            bool IsCrossOverOnMac,
            string Description);
    }
}
