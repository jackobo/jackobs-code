using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Client.Interfaces.Entities
{
    public class DownloadInfo
    {
        public DownloadInfo(string uri, string fileName, long fileSize, string md5)
        {
            this.Uri = uri;
            this.FileName = fileName;
            this.FileSize = fileSize;
            this.MD5 = md5;
        }



        public string Uri { get; set; }

        public string FileName { get; set; }

        public long FileSize { get; set; }

        public string MD5 { get; set; }


    }
}
