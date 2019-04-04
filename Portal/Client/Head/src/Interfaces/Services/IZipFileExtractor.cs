using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Client.Interfaces.Services
{
    public interface IZipFileExtractor : IDisposable
    {
        void Unzip(string zipFileName, string destinationFolder);
        void Cancel();
        event ActionProgressChangedEventHandler UnzipProgressChanged;
        event AsyncCompletedEventHandler UnzipCompleted;

        
    }
}
