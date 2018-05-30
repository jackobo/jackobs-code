using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.Interfaces.UI
{
    public interface IModalWindow
    {
        IDialogBoxCommands Commands { get; set; }
        void ShowModal();
        void Close();
    }
}
