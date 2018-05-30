using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Models
{
    public class JackpotInfoProvider : Interfaces.IJackpotInfoProvider
    {
        public JackpotInfoProvider(IWcfServiceFactory wcfServiceFactory)
        {
            _wcfServiceFactory = wcfServiceFactory;
        }

        IWcfServiceFactory _wcfServiceFactory;

        public int[] GetAllJackpotIds()
        {
            using (var wcf = _wcfServiceFactory.CreateLayoutToolService())
            {
                return wcf.GetAllJackpotIds().JackpotIds;
            }
        }
    }
}
