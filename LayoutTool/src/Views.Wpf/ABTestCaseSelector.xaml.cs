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
using LayoutTool.ViewModels;

namespace LayoutTool.Views.Wpf
{
    /// <summary>
    /// Interaction logic for ABTestCaseSelector.xaml
    /// </summary>
    public partial class ABTestCaseSelector : UserControl
    {
        public ABTestCaseSelector()
        {
            InitializeComponent();
        }


        public static DependencyProperty ABTestCasesProperty = DependencyProperty.Register("ABTestCases", typeof(ABTestCaseSetViewModel[]), typeof(ABTestCaseSelector));
        public static DependencyProperty SelectedAbTestCaseProperty = DependencyProperty.Register("SelectedAbTestCase", typeof(ABTestCaseSetViewModel), typeof(ABTestCaseSelector));


        public ABTestCaseSetViewModel[] ABTestCases
        {
            get { return (ABTestCaseSetViewModel[])GetValue(ABTestCasesProperty); }
            set { SetValue(ABTestCasesProperty, value); }

        }

        public ABTestCaseSetViewModel SelectedAbTestCase
        {
            get { return (ABTestCaseSetViewModel)GetValue(SelectedAbTestCaseProperty); }
            set { SetValue(SelectedAbTestCaseProperty, value); }
        }
    }
}
