using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service
{
    public interface ICallContextServices
    {
        string GetCallingUserName();
    }

    public class WcfOperationCallContextServices : ICallContextServices
    {
        public string GetCallingUserName()
        {
            if (OperationContext.Current == null)
                throw new InvalidOperationException("Operation is not in the WCF context");

            return OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
        }
    }
}
