using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLC_MOD_Toolbox.Services
{
    public interface ILoadingTextService
    {
        public Task<string> GetLoadingTextAsync();
    }
}
