using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class GameGroupLayoutCollectionViewModel : ObservableCollectionExtended<GameGroupLayoutViewModel>
    {
        public GameGroupLayoutCollectionViewModel(string title, IPlayerStatusFriendlyNameProvider playerStatusFriendlyNameProvider)
        {
            this.Title = title;
            AddDynamicLayoutHandler = new AddDynamicLayoutHandler<GameGroupLayoutViewModel>(this,
                                                                                             ps => new GameGroupLayoutViewModel(ps),
                                                                                             playerStatusFriendlyNameProvider);
        }

        
        public string Title { get; private set; }

        public AddDynamicLayoutHandler<GameGroupLayoutViewModel> AddDynamicLayoutHandler { get; private set; }

       
    }
}
