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
    /// Interaction logic for ArenaContentEditorView.xaml
    /// </summary>
    public partial class ArenaLayoutView : UserControl
    {
        public ArenaLayoutView()
        {
            InitializeComponent();
            this.DataContextChanged += ArenaLayoutView_DataContextChanged;
        }

        private void ArenaLayoutView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldViewModel = e.OldValue as ArenaLayoutViewModel;

            if(oldViewModel != null)
                oldViewModel.PropertyChanged -= ViewModel_PropertyChanged;

            var newViewModel = e.NewValue as ArenaLayoutViewModel;

            if(newViewModel != null)
                newViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
            if (e.PropertyName == nameof(ArenaLayoutViewModel.SelectedGame))
            {
                if(ViewModel != null && ViewModel.SelectedGame != null)
                    gamesGrid.ScrollIntoView(ViewModel.SelectedGame);
            }
        }

        private ArenaLayoutViewModel ViewModel
        {
            get { return this.DataContext as ArenaLayoutViewModel; }
        }

        private void gamesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            var dataGrid = sender as DataGrid;

            if(dataGrid != null && dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
            */
        }
    }

   
}
