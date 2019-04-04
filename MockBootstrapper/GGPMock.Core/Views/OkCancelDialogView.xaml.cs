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
using System.Windows.Shapes;
using GGPMockBootstrapper.ViewModels;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.Views
{
    /// <summary>
    /// Interaction logic for OkCancelDialogView.xaml
    /// </summary>
    public partial class OkCancelDialogView : Window
    {
        public OkCancelDialogView(IOkCancelDialogViewModel viewModel)
        {
            InitializeComponent();
            viewModel.Close = () => this.Close();
            contentContainer.Children.Add(ViewFactory.CreateView(viewModel));
            

            this.DataContext = viewModel;
            

            this.SourceInitialized += (x, y) =>
            {
                this.HideMinimizeAndMaximizeButtons();
            };
        }
    }
}
