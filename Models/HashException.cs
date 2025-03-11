namespace LLC_MOD_Toolbox.Models;

internal class HashException : Exception
{
    public HashException()
        : base("Hash 验证失败") { }
}
