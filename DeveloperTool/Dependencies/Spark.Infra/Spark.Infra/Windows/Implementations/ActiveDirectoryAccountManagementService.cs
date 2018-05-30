using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public class ActiveDirectoryAccountManagementService : IAccountManagementServices
    {
        public AccountInfo GetAccountInfo(string fullyQualifiedDomainUserName)
        {
            var windowsIdentityComponents = fullyQualifiedDomainUserName?.Split('/', '\\');

            if (windowsIdentityComponents == null || windowsIdentityComponents.Length != 2)
            {
                throw new ArgumentException("User name must be in the form DOMAIN\\username", nameof(fullyQualifiedDomainUserName));
            }

            var domainName = windowsIdentityComponents[0];
            var userName = windowsIdentityComponents[1];

            using (var ctx = new PrincipalContext(ContextType.Domain, domainName))
            using (var qbeUser = new UserPrincipal(ctx) { SamAccountName = userName })
            using (var srch = new PrincipalSearcher(qbeUser))
            using (var user = srch.FindOne() as UserPrincipal)
            {
                if (user != null && !string.IsNullOrEmpty(user.EmailAddress))
                {
                    return new AccountInfo(userName, user.DisplayName, user.EmailAddress);
                }
                else
                {
                    throw new ApplicationException($"Can't find the user {fullyQualifiedDomainUserName}");
                }
            }

        }
    }
}
