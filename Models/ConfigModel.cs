namespace LLC_MOD_Toolbox.Models
{
    public class GeneralConfig
    {
        public bool skipLCBPathCheck { get; set; } = false;
        public string? LCBPath { get; set; } = null;
        public bool internationalMode { get; set; } = false;
    }
    public class InstallConfig
    {
        public bool installWhenLaunch { get; set; } = false;
        public bool afterInstallClose { get; set; } = false;
    }
    public class AnnouncementConfig
    {
        public bool getAnno { get; set; } = true;
        public int annoVersion { get; set; } = 0;
    }
    public class MirrorChyanConfig
    {
        public bool enable { get; set; } = false;
        public bool notice { get; set; } = false;
    }
    public class NodeConfig
    {
        public string defaultNode { get; set; } = "";
        public string defaultApiNode { get; set; } = "";
    }
    public class SkinConfig
    {
        public string currentSkin { get; set; } = "default";
    }
    public class AppSettings
    {
        public GeneralConfig general { get; set; } = new GeneralConfig();
        public InstallConfig install { get; set; } = new InstallConfig();
        public AnnouncementConfig announcement { get; set; } = new AnnouncementConfig();
        public MirrorChyanConfig mirrorChyan { get; set; } = new MirrorChyanConfig();
        public NodeConfig nodeSelect { get; set; } = new NodeConfig();
        public SkinConfig skin { get; set; } = new SkinConfig();
    }

}
