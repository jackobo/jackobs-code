using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Services;

namespace GamesPortal.Client.Models.Services
{
    public class DownloadManager : IDownloadManager
    {
        System.Net.WebClient _webClient;
        public DownloadManager()
        {
            _webClient = new System.Net.WebClient();
            _webClient.DownloadFileCompleted += _webClient_DownloadFileCompleted;
            _webClient.DownloadProgressChanged += _webClient_DownloadProgressChanged;
        }

        void _webClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            OnDownloadProgressChanged(e.ProgressPercentage);
        }

        void _webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            OnDownloadCompleted(e);
        }

        private void OnDownloadProgressChanged(int percentage)
        {
            DownloadProgressChanged?.Invoke(this, new ActionProgressChangedEventArgs(percentage));
        }

        private void OnDownloadCompleted(System.ComponentModel.AsyncCompletedEventArgs args)
        {
            DownloadCompleted?.Invoke(this, args);

        }


        public void DownloadFile(Uri sourceUri, string localFile)
        {
            _webClient.DownloadFileAsync(sourceUri, localFile);
        }

        public event GamesPortal.Client.Interfaces.Services.ActionProgressChangedEventHandler DownloadProgressChanged;

        public event System.ComponentModel.AsyncCompletedEventHandler DownloadCompleted;

        public void Cancel()
        {
            _webClient.CancelAsync();
        }

      

        public void Dispose()
        {
            _webClient.Dispose();
        }

      
    }
}
