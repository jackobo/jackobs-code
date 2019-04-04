using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class ArenaCollectionViewModel : ObservableCollectionExtended<ArenaViewModel>
    {
        public ArenaCollectionViewModel(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            this.GoToArenaCommand = new Command<ArenaViewModel>(GoToArena);
        }

        public string Title
        {
            get { return "Games"; }
        }

        public ICommand GoToArenaCommand { get; private set; }

        

        private void GoToArena(ArenaViewModel arena)
        {
            this.ServiceLocator.GetInstance<ISkinDesigner>().NavigateToWorkspace(arena);
        }


        IServiceLocator ServiceLocator { get; set; }

        
        

     
    }
}
