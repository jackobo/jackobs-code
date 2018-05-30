using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : GGPGameServer.ApprovalSystem.Common.WPFSingleInstanceApplication
    {
        public App()
            : this(null)
        {
        }
        public App(string applicationVersion)
        {
            InitializeComponent();
            this.ApplicationVersion = applicationVersion;
        }

        string ApplicationVersion { get; set; }


        public override void StartWithSplash<TApp>(string[] args, Func<Window> mainWindowCreator)
        {
            if (!IISDetection.IsInstalled)
            {
                if (MessageBoxResult.Yes != MessageBox.Show("Atention! IIS is not installed no this machine. Some of the GGP Mock components like GGP Simulator User Interface, ThinClient, HistoryHandler, Sample Flash & Html5 Games will not be installed"
                                + Environment.NewLine + Environment.NewLine
                                + "Do you want to continue the installation ?"
                                , "IIS is missing", 
                                MessageBoxButton.YesNo, 
                                MessageBoxImage.Question))
                {
                    return;
                }
            }
            
            base.StartWithSplash<TApp>(args, mainWindowCreator);
        }

        protected override string ApplicationID
        {
            get
            {
                return "GGPMockBootstrapper";
            }
        }

        protected override string GetApplicationTitle()
        {
            return "GGP Mock";
        }

        protected override string CreateApplicationTitle()
        {
            if (string.IsNullOrEmpty(ApplicationVersion))
                return base.CreateApplicationTitle();

            return string.Format("GGP Mock [{0}]", this.ApplicationVersion);
        }

        protected override string GetConfigurationName()
        {
            return null;
        }
        
        
    }
}
