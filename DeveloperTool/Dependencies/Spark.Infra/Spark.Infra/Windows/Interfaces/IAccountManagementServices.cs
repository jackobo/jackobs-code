using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public interface IAccountManagementServices
    {
        AccountInfo GetAccountInfo(string userName);
    }

    public class AccountInfo
    {
        public AccountInfo(string userName, string displayName, string emailAddress)
        {
            this.UserName = userName;
            this.DisplayName = displayName;
            this.EmailAddress = emailAddress;
            
        }
        public string UserName { get; private set; }
        public string DisplayName { get; private set; }
        public string EmailAddress { get; private set; }
    }
}
