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
using Spark.Wpf.Common.Views;

namespace Spark.TfsExplorer.Views.Workspace
{
    /// <summary>
    /// Interaction logic for ComponentsMergesView.xaml
    /// </summary>
    public partial class MergeBuilderView : StandardViewUserControl<ViewModels.Workspace.MergeBuilderViewModel>
    {
        public MergeBuilderView()
        {
            InitializeComponent();
        }
    }
}
