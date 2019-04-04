using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Spark.TfsExplorer.ViewModels.ExplorerBar;
using Spark.Wpf.Common.ViewModels;
using Spark.Wpf.Common.Views;

namespace Spark.TfsExplorer.Views.ExplorerBar
{
    /// <summary>
    /// Interaction logic for ExplorerBarView.xaml
    /// </summary>
    public partial class ExplorerBarView : UserControl
    {
        public ExplorerBarView(IExplorerBar explorerBarViewModel)
        {
            InitializeComponent();
            this.DataContext = explorerBarViewModel;
        }
    }
}
