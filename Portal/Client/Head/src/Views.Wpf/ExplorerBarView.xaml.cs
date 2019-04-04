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

namespace GamesPortal.Client.Views.Wpf
{
    /// <summary>
    /// Interaction logic for ExplorerBarView.xaml
    /// </summary>
    public partial class ExplorerBarView : UserControl
    {
        public ExplorerBarView(ViewModels.ExplorerBar.ExplorerBarViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            
        }
     
    }
}
