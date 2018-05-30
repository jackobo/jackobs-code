using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Types;

namespace GamesPortal.Service
{
    public interface ICallContextServices
    {
        string GetCallingUserName();
        Optional<UserDetails> GetCallingUserDetails();
    }

    public class UserDetails
    {
        public UserDetails(string userName, string displayName, string emailAddress)
        {
            this.UserName = userName;
            this.DisplayName = displayName;
            this.EmailAddress = emailAddress;
        }

        public string UserName { get; private set; }
        public string DisplayName { get; private set; }
        public string EmailAddress { get; private set; }
    }

    public class WcfOperationCallContextServices : ICallContextServices
    {
        public WcfOperationCallContextServices(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(this.GetType());
        }

        ILogger Logger { get; set; }

        public string GetCallingUserName()
        {
            if (OperationContext.Current == null)
                throw new InvalidOperationException("Operation is not in the WCF context");

            return OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
        }

        public Optional<UserDetails> GetCallingUserDetails()
        {
            if (OperationContext.Current == null)
                throw new InvalidOperationException("Operation is not in the WCF context");


            var windowsIdentityName = OperationContext.Current?.ServiceSecurityContext?.WindowsIdentity?.Name;
            var windowsIdentityComponents = windowsIdentityName?.Split('/', '\\');

            if (windowsIdentityComponents == null || windowsIdentityComponents.Length != 2)
            {
                Logger.Warning("GetCallingUserDetails operation failed! Can't find user name and domain name from windows identity");
                return Optional<UserDetails>.None();
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
                    return Optional<UserDetails>.Some(new UserDetails(windowsIdentityName, user.DisplayName,  user.EmailAddress));
                }
                else
                {
                    return Optional<UserDetails>.None();
                }
            }
        }
    }
}
