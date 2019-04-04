using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Client.Interfaces.Services
{
    public delegate void ActionProgressChangedEventHandler(object sender, ActionProgressChangedEventArgs e);

    public class ActionProgressChangedEventArgs : EventArgs
    {
        public ActionProgressChangedEventArgs(int percentage)
        {
            this.Percentage = percentage;
        }

        public int Percentage { get; private set; }

    }

    public interface IDownloadManager : IDisposable
    {
        void DownloadFile(Uri sourceUri, string localFile);
        event ActionProgressChangedEventHandler DownloadProgressChanged;
        event AsyncCompletedEventHandler DownloadCompleted;
        void Cancel();
    }
}
