using System;
using System.Collections.Generic;
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
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.Views
{
    [ViewModel(typeof(ViewModels.MainViewModel))]
    public partial class MainView : UserControl, IView
    {
        

        public MainView()
        {
            InitializeComponent();
            
        }

        #region IView Members

        private ViewModels.MainViewModel MainViewModel
        {
            get
            {
                return this.DataContext as ViewModels.MainViewModel;
            }
            set
            {
                this.DataContext = value;

                value.PropertyChanged += ViewModelPropertyChanged;

                LoadWorkAreaItem();
            }
        }

        
     

        object IView.ViewModel
        {
            get
            {
                return this.MainViewModel;
            }
            set
            {
                this.MainViewModel = value as ViewModels.MainViewModel;
            }
        }


        void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.MainViewModel.GetPropertyName(t => t.SelectedItem))
            {
                LoadWorkAreaItem();
            }
        }

        private void LoadWorkAreaItem()
        {
            workArea.Children.Clear();

            if (this.MainViewModel.SelectedItem == null)
            {
                return;
            }

            var workAreaItemViewModel = this.MainViewModel.SelectedItem.WorkAreaItem;

            if (workAreaItemViewModel != null)
            {
                workArea.Children.Add(ViewFactory.CreateView(workAreaItemViewModel));
            }

        }

        #endregion
    }
}
