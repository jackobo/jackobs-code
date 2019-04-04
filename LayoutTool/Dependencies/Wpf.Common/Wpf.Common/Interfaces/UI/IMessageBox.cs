using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.Interfaces.UI
{
    public interface IMessageBox
    {
        void ShowMessage(string message);
        MessageBoxResponse ShowYesNoMessage(string message);
        MessageBoxResponse ShowYesNoCancelMessage(string message);

    }

    public enum MessageBoxResponse
    {
        // Summary:
        //     The message box returns no result.
        None = 0,
        //
        // Summary:
        //     The result value of the message box is OK.
        OK = 1,
        //
        // Summary:
        //     The result value of the message box is Cancel.
        Cancel = 2,
        //
        // Summary:
        //     The result value of the message box is Yes.
        Yes = 6,
        //
        // Summary:
        //     The result value of the message box is No.
        No = 7,
    }
}
