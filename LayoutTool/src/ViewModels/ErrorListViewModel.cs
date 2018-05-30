using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class ErrorListViewModel : ObservableCollection<ErrorMessageViewModel>
    {
        public ErrorListViewModel()
        {

        }

        public ErrorListViewModel(IEnumerable<ErrorMessageViewModel> errors)
            : base(errors)
        {

        }
    }


    public class ErrorMessageViewModel
    {
        public ErrorMessageViewModel(string sourceName, string message, ErrorServerity severity, Action navigateAction)
        {
            this.SourceName = sourceName;
            this.Message = message;
            this.Severity = severity;

            if(navigateAction != null)
                this.NavigateCommand = new Command(navigateAction);
        }

        public string SourceName { get; private set; }
        
        public string Message { get; private set; }
        
        public ErrorServerity Severity { get; private set; }

        public ICommand NavigateCommand { get; private set; }
        
    }
}
