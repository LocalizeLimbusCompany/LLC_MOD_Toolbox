namespace LLC_MOD_Toolbox.Models
{
    public class SkinDefinition
    {
        public string name { get; set; } = "";
        public string displayName { get; set; } = "";
        public string desc { get; set; } = "";
        public string author { get; set; } = "";
        public string version { get; set; } = "1.0.0";
        public Dictionary<string, string> images { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> visibility { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, string> margins { get; set; } = new Dictionary<string, string>();
    }
}
