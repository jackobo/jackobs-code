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
using System.Windows.Shapes;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.Views
{
    /// <summary>
    /// Interaction logic for ModalWindow.xaml
    /// </summary>
    public partial class ModalWindow : Window, IModalWindow
    {
        public ModalWindow()
        {
            InitializeComponent();

            this.SourceInitialized += (x, y) =>
            {
                this.HideMinimizeAndMaximizeButtons();
            };
        }


        public IDialogBoxCommands Commands
        {
            get
            {
                return this.DataContext as IDialogBoxCommands;
            }
            set
            {
                this.DataContext = value;
            }
        }


        public void ShowModal()
        {
            this.ShowDialog();
        }

    }
}
