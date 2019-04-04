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

namespace Spark.TfsExplorer.Views.Workspace
{
    /// <summary>
    /// Interaction logic for ComponentsExplorerBarItemView.xaml
    /// </summary>
    public partial class ComponentsExplorerBarItemView : UserControl
    {
        public ComponentsExplorerBarItemView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ComponentImageSourceProperty = DependencyProperty.RegisterAttached("ComponentImageSource", typeof(ImageSource), typeof(ComponentsExplorerBarItemView));

        public ImageSource ComponentImageSource
        {
            get { return (ImageSource)GetValue(ComponentImageSourceProperty); }
            set { SetValue(ComponentImageSourceProperty, value); }
        }
    }
}
