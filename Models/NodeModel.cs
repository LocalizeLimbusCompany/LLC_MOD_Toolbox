namespace LLC_MOD_Toolbox.Models
{

    public class RootModel
    {
        public List<Node> DownloadNode { get; set; } = [];
        public List<Node> ApiNode { get; set; } = [];
    }
    public class Node
    {
        public string Name { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
