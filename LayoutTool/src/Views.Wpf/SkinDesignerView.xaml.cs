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
using Prism.Regions;
using LayoutTool.ViewModels;

namespace LayoutTool.Views.Wpf
{
    /// <summary>
    /// Interaction logic for LayoutEditorView.xaml
    /// </summary>
    public partial class SkinDesignerView : ViewBaseUserControl<SkinDesignerViewModel>, INavigationAware
    {
        public SkinDesignerView()
        {
            InitializeComponent();
        }
     
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            var skinEditor = navigationContext.ExtractViewModel<SkinDesignerViewModel>();
            skinEditor.ExplorerBar.Items[0].IsSelected = true;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
                scrollviewer.LineUp();
            else
                scrollviewer.LineDown();
            e.Handled = true;
        }
    }
}
