using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLC_MOD_Toolbox.Models
{
    class AppSettings
    {
        public bool CskipLCBPathCheck { get; set; } = false;
        public string CLCBPath { get; set; } = string.Empty;
    }
}
