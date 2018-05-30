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

namespace GGPMockBootstrapper.Views
{
    [ViewModel(typeof(ViewModels.Html5GamesWorkAreaItem))]
    public partial class Html5GamesWorkAreaView : UserControl, IView
    {
        public Html5GamesWorkAreaView()
        {
            InitializeComponent();
        }

        #region IView Members

        public object ViewModel
        {
            get
            {
                return this.DataContext;
            }
            set
            {
                this.DataContext = value;
                holder.Children.Add(new GamesWorkAreaItemView(value as ViewModels.GamesWorkAreaItem) { MainFlashColumnVisibility = System.Windows.Visibility.Collapsed });
            }
        }

        #endregion

        private void txtHttpHandlerAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHttpHandlerAddress.SelectAll();
        }
    }
}
