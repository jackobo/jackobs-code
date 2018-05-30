using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class GGPSimulatorWorkAreaItem : WorkAreaItemBase
    {
        public GGPSimulatorWorkAreaItem(IWorkArea workArea)
            : base(workArea)
        {
            this.OpenGGPSimulatorAction = new ActionViewModel("Open GGP simulator with default browser", new Command(OpenGGPSimulator));
        }

        private void OpenGGPSimulator()
        {

            Models.InstalledBrowsersProvider.OpenWithChromeIfPossible(this.GGPSimulatorProduct.GetSimulatorURL());
 
        }

        Models.GGPSimulator.GGPSimulatorProduct GGPSimulatorProduct
        {
            get { return WorkArea.GetProduct<Models.GGPSimulator.GGPSimulatorProduct>(); }
        }

        public IActionViewModel OpenGGPSimulatorAction { get; private set; }



        public string SimulatorUrl
        {
            get { return this.GGPSimulatorProduct.GetSimulatorURL(); }
        }
    }
}
