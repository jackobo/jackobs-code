using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models
{
    public static class CDNMappings
    {
        private static Dictionary<string, string> _cdnMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"http://ndl.888.com", "http://ndl-cdn.888.com/casino/" },
            {"http://es.safe-iplay.com", "http://ndl-cdn-es.safe-iplay.com/casino/" },
            {"http://ndl-cdn-it.safe-iplay.com", "http://ndl-cdn-it.safe-iplay.com/casino/" },
            {"http://ndl-cdn-dk.safe-iplay.com", "http://ndl-cdn-dk.safe-iplay.com/casino/" },

            {"http://casino-de.igaming-services.com", "http://casino-de.igaming-services.com/casino/" }, //!
            {"http://casino-nj.secured-igaming-services.com", "http://casino-nj.secured-igaming-services.com/casino/" }, //!
            
            {"http://safe-iplay.com", "http://ndl-cdn.safe-iplay.com/casino/" },
            {"http://ndl-ro.safe-iplay.com", "http://ndl-ro.safe-iplay.com/casino/"}
            //special
        };

        public static PathDescriptor GetCdnUrl(string domainUrl)
        {
            if (_cdnMappings.ContainsKey(domainUrl))
                return new PathDescriptor(_cdnMappings[domainUrl]);
            else
                return null;
        }
    }
}
