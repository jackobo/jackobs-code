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
    /// Interaction logic for LobbiesView.xaml
    /// </summary>
    public partial class LobbyCollectionView : ViewBaseUserControl<LobbyCollectionViewModel>
    {
        public LobbyCollectionView()
        {
            InitializeComponent();
        }

     
    }
}
