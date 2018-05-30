using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GGPGameServer.ApprovalSystem.Common.Logger;
using GGPMockBootstrapper.Models;

namespace GGPMockBootstrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            
            var uiServices = new GGPGameServer.ApprovalSystem.Common.WPFUserInterfaceServices();
            uiServices.RegisterCustomUIService<ViewModels.IDialogShowService>(new Views.WpfDialogShowService());
            uiServices.RegisterCustomUIService<Models.IApplicationServices>(new WpfApplicationServices());
            ViewModels.MainViewModel.Init(uiServices);
            this.ViewModel = ViewModels.MainViewModel.Singletone;
        }

        ISysTrayManager SysTrayManager { get; set; }


        private ViewModels.MainViewModel ViewModel
        {
            get { return this.DataContext as ViewModels.MainViewModel; }
            set
            {
                this.DataContext = value;
            }
        }


        private void InitSysTrayManager()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
                SysTrayManager = new WpfSysTrayManager(this);
            else
                SysTrayManager = new VoidSysTrayManager();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var installationProgressViewModel = this.ViewModel.CreateInstallationProgressViewModel();

            var installationProgressView =  Views.ViewFactory.CreateView(installationProgressViewModel);

            contentContainer.Children.Add(installationProgressView);

            installationProgressViewModel.Run();


            installationProgressViewModel.Finished += (sndr, args) =>
                {
                    this.Dispatcher.Invoke(new Action(() => 
                        {
                            try
                            {
                                contentContainer.Children.Remove(installationProgressView);
                                contentContainer.Children.Add(Views.ViewFactory.CreateView(this.ViewModel));
                                mainMenu.Visibility = System.Windows.Visibility.Visible;
                                InitSysTrayManager();
                                ViewModel.GamesInformationProvider.Refresh();
                            }
                            catch(Exception ex)
                            {
                                LogException(ex);
                            }
                        }));
                };


            
        }

        private void LogException(Exception ex)
        {
            this.Logger.Exception(ex);
        }

        ILogNotifier Logger
        {
            get
            {
                return LoggerFactoryManager.CreateLogNotifier(this.GetType());
            }
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.ViewModel.AppExit();

            base.OnClosing(e);
        }
        
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            SysTrayManager.Enabled = false;
            this.Close();
        }

      

       
    }
}
