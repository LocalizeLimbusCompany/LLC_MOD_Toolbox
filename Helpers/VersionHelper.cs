using System.Reflection;

namespace LLC_MOD_Toolbox.Helpers;

public static class VersionHelper
{
    /// <summary>
    /// 通过反射获取的本地版本号
    /// </summary>
    public static Version LocalVersion =>
        Assembly.GetExecutingAssembly().GetName().Version ?? throw new NullReferenceException();

    public static bool CheckForUpdate(string version) => new Version(version) > LocalVersion;
}
