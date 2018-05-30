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

namespace LayoutTool.Views.Wpf
{
    /// <summary>
    /// Interaction logic for AddDynamicLayoutHandlerView.xaml
    /// </summary>
    public partial class AddDynamicLayoutHandlerView : UserControl
    {
        public AddDynamicLayoutHandlerView()
        {
            InitializeComponent();
        }

        public static DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(AddDynamicLayoutHandlerView), new PropertyMetadata("Add Dynamic Layout"));

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
    }
}
