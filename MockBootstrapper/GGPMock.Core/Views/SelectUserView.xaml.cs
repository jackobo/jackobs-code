namespace GGPMockBootstrapper.Views
{
    [ViewModel(typeof(ViewModels.SelectUserDialog))]
    public partial class SelectUserView :IView
    {
        public SelectUserView()
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
