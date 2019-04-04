using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models.FlashPolicyServer
{
    public class FlashPolicyServerProduct : Product
    {
        public FlashPolicyServerProduct(IInstalationContext installationContext)
            : base("FlashPolicyServer", installationContext)
        {

        }
        public override IInstallAction[] GetInstallActions()
        {
            return new IInstallAction[]
            {
                new FlashPolicyServerInstallAction(this)
            };
        }

        protected override string GetContentSectionName()
        {
            return "FlashPolicyServer";
        }
    }
}
