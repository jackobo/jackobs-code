using System.Collections.ObjectModel;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.MainMenu
{
    public class MainMenuViewModel : ServicedViewModelBase
    {
        public MainMenuViewModel(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Items = new ObservableCollection<MenuItem>();
            this.Items.Add(new FileMenuItem(serviceLocator));
            this.Items.Add(new ArtifactoryMenuItem(serviceLocator));
        }

        public ObservableCollection<MenuItem> Items { get; private set; }
    }

    
    public class MenuItem : ServicedViewModelBase
    {
        public MenuItem(string caption, ICommand command, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Caption = caption;
            this.Command = command;
            this.Items = new ObservableCollection<MenuItem>();
        }

        public string Caption { get; set; }
        public ICommand Command { get; set; }
        public ObservableCollection<MenuItem> Items { get; private set; }

        public bool IsSeparator { get; protected set; }
    }


    public class MenuItemSeparator : MenuItem
    {
        public MenuItemSeparator(IServiceLocator serviceLocator)
            : base("", null, serviceLocator)
        {
            this.IsSeparator = true;
        }
    }


    public class FileMenuItem : MenuItem
    {
        public FileMenuItem(IServiceLocator serviceLocator)
            : base("File", null, serviceLocator)
        {
            this.Items.Add(new MenuItem("Exit", new Command(ExitApplication), serviceLocator));
        }


        private void ExitApplication()
        {
            if (MessageBoxResponse.Yes == this.ServiceLocator.GetInstance<IMessageBox>().ShowYesNoMessage("Are you sure you want to exit the application ?"))
            {
                this.ServiceLocator.GetInstance<IApplicationServices>().Exit();
            }   
        }
    }


    public class ArtifactoryMenuItem : MenuItem
    {
        public ArtifactoryMenuItem(IServiceLocator serviceLocator)
            : base("Artifactory", null, serviceLocator)
        {
            this.Items.Add(new MenuItem("Force synchronization", new Command(ForceSynchronization), serviceLocator));
        }

        private void ForceSynchronization()
        {
            var gamesRepository = this.ServiceLocator.GetInstance<IGamesRepositorySynchronizer>();
            gamesRepository.ForceSynchronization();

            this.ServiceLocator.GetInstance<IMessageBox>().ShowMessage("Synchronization was initiated on the server! When synchronization is finished you will be notified in the Information area at the bottom of the screen");
        }
    }
}
