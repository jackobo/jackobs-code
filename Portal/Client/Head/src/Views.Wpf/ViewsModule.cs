using System;
using System.Windows;
using GamesPortal.Client.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace GamesPortal.Client
{
    [Module(ModuleName = WellKnownModules.Views)]
    [ModuleDependency(WellKnownModules.ViewModels)]
    public class ViewsModule : IModule
    {
        public ViewsModule(IUnityContainer container)
        {
            
            this.Container = container;

        }

        IUnityContainer Container { get; set; }

        IRegionManager RegionManager { get; set; }

        public void Initialize()
        {

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ExplorerBar", typeof(GamesPortal.Client.Views.Wpf.ExplorerBarView));
            regionManager.RegisterViewWithRegion("Workspace", typeof(GamesPortal.Client.Views.Wpf.WorkspaceView));
            regionManager.RegisterViewWithRegion("NotificationArea", typeof(GamesPortal.Client.Views.Wpf.NotificationAreaView));
            regionManager.RegisterViewWithRegion("MainMenu", typeof(GamesPortal.Client.Views.Wpf.MainMenuView));

            LoadDialogsResources();

        }

        private void LoadDialogsResources()
        {
            var src = new Uri(string.Format("pack://application:,,,/{0};component/Dialogs/DialogsDictionary.xaml", this.GetType().Assembly.GetName().Name), UriKind.RelativeOrAbsolute);
            var dic = new ResourceDictionary();
            dic.Source = src;

            Application.Current.Resources.MergedDictionaries.Add(dic);
        }
    }
}
