namespace LLC_MOD_Toolbox.Models
{
    [Obsolete("正在向Settings.settings文件迁移")]
    internal class AppSettings
    {
        public bool CskipLCBPathCheck { get; set; } = false;
        public string CLCBPath { get; set; } = string.Empty;
    }
}
