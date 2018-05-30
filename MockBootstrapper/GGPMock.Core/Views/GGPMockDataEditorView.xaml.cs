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
    [ViewModel(typeof(ViewModels.GGPMockDataEditorWorkAreaItem))]
    public partial class GGPMockDataEditorView : UserControl, IView
    {
        public GGPMockDataEditorView()
        {
            InitializeComponent();
        }


        ViewModels.GGPMockDataEditorWorkAreaItem _editorViewModel;

        public ViewModels.GGPMockDataEditorWorkAreaItem EditorViewModel
        {
            get { return _editorViewModel; }
            set
            {
                _editorViewModel = value;
                this.DataContext = _editorViewModel;
                _editorViewModel.PropertyChanged += _editorViewModel_PropertyChanged;
            }
        }

        void _editorViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.DataContext = null;
            this.DataContext = _editorViewModel;
        }


        #region IView Members

        object IView.ViewModel
        {
            get
            {
                return this.EditorViewModel;
            }
            set
            {
                this.EditorViewModel = value as ViewModels.GGPMockDataEditorWorkAreaItem;          
            }
        }

      

        #endregion
    }
}
