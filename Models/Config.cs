using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLC_MOD_Toolbox.Helpers;

namespace LLC_MOD_Toolbox.Models
{
    public class Config(PrimaryNodeList primaryNodeList)
    {
        public string GamePath { get; set; } =
            PathHelper.DetectedLimbusCompanyPath ?? PathHelper.SelectPath();
        public string? Token { get; set; }
        public NodeInformation ApiNode { get; set; } =
            primaryNodeList.ApiNode.First(n => n.IsDefault);

        public NodeInformation DownloadNode { get; set; } =
            primaryNodeList.DownloadNode.First(n => n.IsDefault);
    }
}
