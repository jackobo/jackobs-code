using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models.GGP
{
    public class HistoryHandlerInstallAction : IISAppInstallActionBase<GGPProduct>
    {
        public HistoryHandlerInstallAction(GGPProduct ggpProduct)
            : base(ggpProduct, GGPHistoryHandlerAppName,
                    Microsoft.Web.Administration.ManagedPipelineMode.Integrated)
        {
            
        }


        public static readonly string GGPHistoryHandlerAppName = "GGPHistoryHandler";

        protected override string GetPackageName()
        {
            return "HistoryHandler.zip";
        }


        protected override void ConfigureApplicationCore(IInstalationContext context, Microsoft.Web.Administration.ServerManager iisManager)
        {
            base.ConfigureApplicationCore(context, iisManager);

            context.EnvironmentServices.FileSystem.CopyFile(Product.GetEmbededResourceFullPath("HistoryHandler.web.config"), 
                                                            Path.Combine(GetApplicationPhysicalPath(iisManager), "Web.config"));
            
        }
    }
}
