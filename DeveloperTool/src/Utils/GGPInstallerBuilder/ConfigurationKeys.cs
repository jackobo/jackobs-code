using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGPInstallerBuilder
{
    public static class ConfigurationKeys
    {
        public static readonly string installerDefinitionServerPath = "installerDefinitionServerPath";
        public static readonly string ggpApprovalSystemSourceCodeServerPath = "ggpApprovalSystemSourceCodeServerPath";
        public static readonly string installerDistributionPath = "installerDistributionPath";
        public static readonly string oldDistributionPath = "oldDistributionPath"; //for backward compatibility with the installers up to 1.5.x
    }
}
