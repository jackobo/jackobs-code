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
    [ViewModel(typeof(ViewModels.GGPLogWorkAreaItem))]
    public partial class GGPLogView : UserControl, IView
    {
        public GGPLogView()
        {
            InitializeComponent();
        }

        ViewModels.GGPLogWorkAreaItem _viewModel;

        #region IView Members

        object IView.ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value as ViewModels.GGPLogWorkAreaItem;
                this.DataContext = _viewModel;

                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged += _viewModel_PropertyChanged;
                }
            }
        }

        void _viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _viewModel.GetPropertyName(t => t.CurrentItem))
            {
                //lvwMessages.ScrollIntoView(_viewModel.CurrentItem);
                gridMessages.ScrollIntoView(_viewModel.CurrentItem);
            }
        }



        #endregion
    }
}
