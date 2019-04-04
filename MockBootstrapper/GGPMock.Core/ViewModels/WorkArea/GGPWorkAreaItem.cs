using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class GGPWorkAreaItem : WorkAreaItemBase
    {
        public GGPWorkAreaItem(IWorkArea workArea)
            : base(workArea)
        {

        }

        public string GGPVersion
        {
            get
            {
                return GGPProduct.GetGGPInfo().GGPVersion.ToString();
            }
        }


        public string InstallerVersion
        {
            get
            {
                return GGPProduct.GetGGPInfo().InstallerVersion.ToString();
            }
        }

        


        private GGPMockBootstrapper.Models.GGP.GGPProduct GGPProduct
        {
            get
            {
                return this.WorkArea.GetProduct<GGPMockBootstrapper.Models.GGP.GGPProduct>();
            }
        }
    }
}
