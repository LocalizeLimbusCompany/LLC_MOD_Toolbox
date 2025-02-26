namespace LLC_MOD_Toolbox.Models;

class HashException : Exception
{
    public HashException()
        : base("Hash 验证失败") { }
}
