using System.Windows.Input;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.Wpf.Common.UIServices
{
    public class DialogBoxCommands : ViewModelBase, IDialogBoxCommands
    {
        public DialogBoxCommands(object dialogViewModel)
        {
            this.DialogViewModel = dialogViewModel;
        }

        public object DialogViewModel { get; set; }


        public ICommand OkCommand
        {
            get;
            set;
        }


        public ICommand CancelCommand
        {
            get;
            set;
        }
    }
}
