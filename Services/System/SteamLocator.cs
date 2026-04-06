using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

public static class SteamLocator
{
    public static string FindLimbusCompanyPath(string? appId, string? executableName)
    {
        var steamRoot = GetSteamRootFromRegistry();
        if (string.IsNullOrWhiteSpace(steamRoot))
            throw new InvalidOperationException("未在注册表找到 Steam 安装路径");

        steamRoot = NormalizePath(steamRoot);

        var libs = new List<string> { steamRoot };
        var vdf = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
        libs.AddRange(ParseLibraryFolders(vdf));

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in libs)
        {
            var lib = NormalizePath(raw);
            if (!seen.Add(lib)) continue;

            var steamapps = Path.Combine(lib, "steamapps");

            if (!string.IsNullOrWhiteSpace(appId))
            {
                var acf = Path.Combine(steamapps, $"appmanifest_{appId}.acf");
                var dir = ParseInstallDir(acf);
                if (!string.IsNullOrWhiteSpace(dir))
                {
                    var full = Path.Combine(steamapps, "common", dir);
                    if (IsValidGameDir(full, executableName))
                        return full;
                }
            }

            var fallback = Path.Combine(steamapps, "common", "Limbus Company");
            if (IsValidGameDir(fallback, executableName))
                return fallback;
        }

        throw new DirectoryNotFoundException("未在任何库中找到 Limbus Company");
    }

    // ===== Helper 方法（和之前一致） =====
    private static string GetSteamRootFromRegistry()
    {
        if (TryRegGetString(Registry.CurrentUser, @"Software\Valve\Steam", "SteamPath", out var root) &&
            !string.IsNullOrWhiteSpace(root))
            return root!;

        if (TryRegGetString(Registry.LocalMachine, @"SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", out root) &&
            !string.IsNullOrWhiteSpace(root))
            return root!;

        return string.Empty;
    }

    private static bool TryRegGetString(RegistryKey hive, string path, string name, out string? value)
    {
        value = null;
        try
        {
            using var key = hive.OpenSubKey(path, false);
            if (key == null) return false;
            value = key.GetValue(name) as string;
            return value != null;
        }
        catch { return false; }
    }

    private static string NormalizePath(string p)
    {
        if (string.IsNullOrWhiteSpace(p)) return p;
        p = p.Replace('/', '\\').Replace(@"\\", @"\");
        try { return Path.GetFullPath(p); }
        catch { return p; }
    }

    private static IEnumerable<string> ParseLibraryFolders(string file)
    {
        var results = new List<string>();
        try
        {
            if (!File.Exists(file)) return results;
            var text = File.ReadAllText(file);
            var re = new Regex("(?i)\"path\"\\s*\"([^\"]+)\"", RegexOptions.Compiled);
            foreach (Match m in re.Matches(text))
            {
                if (m.Groups.Count >= 2)
                    results.Add(NormalizePath(m.Groups[1].Value));
            }
        }
        catch { }
        return results;
    }

    private static string ParseInstallDir(string acf)
    {
        try
        {
            if (!File.Exists(acf)) return string.Empty;
            var b = File.ReadAllText(acf);
            var re = new Regex("(?i)\"installdir\"\\s*\"([^\"]+)\"", RegexOptions.Compiled);
            var m = re.Match(b);
            if (m.Success && m.Groups.Count >= 2)
                return m.Groups[1].Value.Replace(@"\\", @"\");
        }
        catch { }
        return string.Empty;
    }

    private static bool IsValidGameDir(string dir, string? executableName)
    {
        if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir)) return false;
        if (string.IsNullOrWhiteSpace(executableName)) return true;
        return File.Exists(Path.Combine(dir, executableName));
    }
}
