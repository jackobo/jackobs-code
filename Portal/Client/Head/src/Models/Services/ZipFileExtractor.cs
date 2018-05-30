using System;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Services;
using Spark.Infra.Types;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Infra.Windows;

namespace GamesPortal.Client.Models.Services
{
    public class ZipFileExtractor : IZipFileExtractor
    {
        public ZipFileExtractor(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            
        }

        IServiceLocator ServiceLocator { get; set; }

        IFileSystemManager FileSystemManager
        {
            get { return this.ServiceLocator.GetInstance<IFileSystemManager>(); }
        }

        private IApplicationServices ApplicationServices
        {
            get
            {
                return this.ServiceLocator.GetInstance<IApplicationServices>();
            }
        }


        private void ExecuteOnUIThreadIfPossible(Action action)
        {
            var appserv = ApplicationServices;
            if (appserv != null)
                appserv.ExecuteOnUIThread(action);
            else
                action();
        }

        #region IZipFileExtractor Members

        public void Unzip(string zipFileName, string destinationFolder)
        {
            _canceled = false;
            this.ZipFileName = zipFileName;
            this.DestinationFolder = destinationFolder;

            ServiceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(UnzipAsync);
        }


        private void UnzipAsync()
        {
            try
            {
                if (FileSystemManager.FolderExists(this.DestinationFolder))
                    FileSystemManager.DeleteFolder(this.DestinationFolder);

                FileSystemManager.CreateFolder(this.DestinationFolder);

                using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(this.ZipFileName))
                {
                    zipFile.ExtractProgress += zipFile_ExtractProgress;


                    zipFile.ExtractAll(this.DestinationFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                    ExecuteOnUIThreadIfPossible(() => OnUnzipCompleted(null, _canceled));
                }
            }
            catch (Exception ex)
            {
                ExecuteOnUIThreadIfPossible(() => OnUnzipCompleted(ex, false));
            }
        }

     
     
        private string ZipFileName { get; set; }
        private string DestinationFolder { get; set; }
        


        void zipFile_ExtractProgress(object sender, Ionic.Zip.ExtractProgressEventArgs e)
        {

            if (_canceled)
                e.Cancel = true;
            else
            {
                ServiceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(() => ExecuteOnUIThreadIfPossible(() => OnUnzipProgressChanged(e.EntriesTotal, e.EntriesExtracted)));
            }
        }


        bool _canceled = false;
        public void Cancel()
        {
            _canceled = true;
        }

        private void OnUnzipProgressChanged(int totalEntries, int extractedEntries)
        {
            var ev = UnzipProgressChanged;

            if (ev != null && totalEntries > 0)
            {
                ev(this, new ActionProgressChangedEventArgs((int)Math.Round(((decimal)extractedEntries / (decimal)totalEntries) * 100m, 0)));
            }
        }

        public event ActionProgressChangedEventHandler UnzipProgressChanged;

        public event System.ComponentModel.AsyncCompletedEventHandler UnzipCompleted;

        private void OnUnzipCompleted(Exception ex, bool canceled)
        {
            var ev = UnzipCompleted;
            if (ev != null)
            {
                UnzipCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(ex, canceled, null));
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
