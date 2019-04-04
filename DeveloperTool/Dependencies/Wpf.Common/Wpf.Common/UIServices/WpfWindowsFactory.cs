using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.Views;

namespace Spark.Wpf.Common.UIServices
{
    public class WpfWindowsFactory : IWindowsFactory
    {
        #region IWindowsFactory Members

        public IModalWindow CreateModalWindow()
        {
            return new ModalWindow(){Owner = System.Windows.Application.Current.MainWindow};
        }

        #endregion
    }
}
