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
    [ViewModel(typeof(ViewModels.AddNewUserDialog))]
    public partial class AddNewUserView : IView
    {
        public AddNewUserView()
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
            }
        }

        #endregion
    }
}
