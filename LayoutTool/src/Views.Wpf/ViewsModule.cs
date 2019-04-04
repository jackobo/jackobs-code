using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using LayoutTool.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.Views.Wpf
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
        public void Initialize()
        {
            var regionManager = this.Container.Resolve<IRegionManager>();

            RegisterViewsWithViewModels();
            LoadDialogsResources();
            
            regionManager.NavigateToMainContent(Container.Resolve<SkinToDesignSelectorViewModel>());
            
        }

     

        private void RegisterViewsWithViewModels()
        {
            var viewBaseUserControlClassName = nameof(ViewBaseUserControl<IViewModel>);
            var userControlTypes = this.GetType().Assembly.GetTypes().Where(t => t.BaseType.Name.StartsWith(viewBaseUserControlClassName))
                                                                          .ToArray();

            foreach(var controlType in userControlTypes)
            {
                this.Container.RegisterViewWithViewModel(controlType, controlType.BaseType.GetGenericArguments()[0]);
            }



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
