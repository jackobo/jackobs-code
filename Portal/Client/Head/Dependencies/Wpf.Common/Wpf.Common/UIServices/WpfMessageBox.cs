using System;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.UIServices
{
    public class WpfMessageBox : IMessageBox
    {
        public WpfMessageBox(IApplicationServices applicationServices)
        {
            this.ApplicationServices = applicationServices;
        }

        
        IApplicationServices ApplicationServices { get; set; }

        #region IMessageBox Members

        public void ShowMessage(string message)
        {
            if (ApplicationServices == null)
                MessageBox.Show(message);
            else
                this.ApplicationServices.ExecuteOnUIThread(() => MessageBox.Show(message));
        }

        #endregion

        #region IMessageBox Members


        public MessageBoxResponse ShowYesNoMessage(string message)
        {
            System.Windows.MessageBoxResult msgResult = MessageBoxResult.None;
            if (ApplicationServices == null)
                msgResult = MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo);
            else
                ApplicationServices.ExecuteOnUIThread(() => msgResult = MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo));

            return ConvertMessageBoxResult(msgResult);
        }

        public MessageBoxResponse ShowYesNoCancelMessage(string message)
        {
            System.Windows.MessageBoxResult msgResult = MessageBoxResult.None;
            if (ApplicationServices == null)
                msgResult = MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNoCancel);
            else
                ApplicationServices.ExecuteOnUIThread(() => msgResult = MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNoCancel));

            return ConvertMessageBoxResult(msgResult);
        }

        private MessageBoxResponse ConvertMessageBoxResult(MessageBoxResult msgResponse)
        {
            switch (msgResponse)
            {
                case MessageBoxResult.None:
                    return MessageBoxResponse.None;
                case MessageBoxResult.Cancel:
                    return MessageBoxResponse.Cancel;
                case MessageBoxResult.No:
                    return MessageBoxResponse.No;
                case MessageBoxResult.OK:
                    return MessageBoxResponse.OK;
                case MessageBoxResult.Yes:
                    return MessageBoxResponse.Yes;
                default:
                    throw new ArgumentException(string.Format("Unknown MessageBox response {0}", msgResponse));
            }
        }

        #endregion
    }
}
