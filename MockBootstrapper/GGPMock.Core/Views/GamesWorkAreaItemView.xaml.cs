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
    /// <summary>
    /// Interaction logic for GamesWorkAreaItemView.xaml
    /// </summary>
    public partial class GamesWorkAreaItemView : UserControl
    {
        public GamesWorkAreaItemView(ViewModels.GamesWorkAreaItem viewModel)
        {
            InitializeComponent();
            this.ViewModel = viewModel;
            viewModel.PropertyChanged += viewModel_PropertyChanged;
            Navigate();
        }


        


        public Visibility MainFlashColumnVisibility
        {
            get { return colMainFlashFile.Visibility; }
            set
            {

                colMainFlashFile.Visibility = value;
            }
        }

        private ViewModels.GamesWorkAreaItem _viewModel;

        public ViewModels.GamesWorkAreaItem ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                this.DataContext = _viewModel;
            }
        }

        void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.ViewModel.GetPropertyName(t => t.CurrentGameSimulatorUrl))
            {
                Navigate();
            }
        }

        private void Navigate()
        {
            if (!string.IsNullOrEmpty(this.ViewModel.CurrentGameSimulatorUrl))
            {
                webBrowser.Navigate(this.ViewModel.CurrentGameSimulatorUrl);
                tabControl.SelectedIndex = 1;
            }
        }


        
    }
}
