using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models.MainProxy
{
    public class MainProxyProduct : Product
    {
        public MainProxyProduct(IInstalationContext installationContext)
            : base("MainProxy", installationContext)
        {

        }
        protected override string GetContentSectionName()
        {
            return "MainProxy";
        }

        public override IInstallAction[] GetInstallActions()
        {
            return new IInstallAction[]
            {
                new MainProxyInstallAction(this)
            };
        }
    }
}
