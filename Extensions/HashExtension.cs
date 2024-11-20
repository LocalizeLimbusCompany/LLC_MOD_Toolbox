using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLC_MOD_Toolbox.Extensions
{
    public static class HashExtension
    {
        public static string ToSha256(this byte[] hash)
        {
            var sb = new StringBuilder();

            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
