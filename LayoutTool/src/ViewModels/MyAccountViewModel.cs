using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class MyAccountViewModel : ViewModelBase
    {
       
        public MyAccountViewModel()
        {
        }


   
        private MyAccountItemCollectionViewModel _lobby = new MyAccountItemCollectionViewModel();

        public MyAccountItemCollectionViewModel Lobby
        {
            get
            {
                return _lobby;
            }
            private set
            {
                SetProperty(ref _lobby, value);
            }
        }

        private MyAccountItemCollectionViewModel _history = new MyAccountItemCollectionViewModel();
        public MyAccountItemCollectionViewModel History
        {
            get
            {
                return _history;
            }

            private set
            {
                SetProperty(ref _history,  value);
            }
        }


        ObservableCollection<MyAccountItemViewModel> _allMyAccountItems = new ObservableCollection<MyAccountItemViewModel>();
        public ObservableCollection<MyAccountItemViewModel> AllMyAccountItems
        {
            get { return _allMyAccountItems; }
            private set
            {
                SetProperty(ref _allMyAccountItems, value);
            }
        }
        
    }
}
