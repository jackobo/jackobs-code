using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using LayoutTool.Interfaces;
using Spark.Infra.Windows;

namespace LayoutTool.Models
{
    public class TextFileReader : ITextFileReader
    {
        public TextFileReader(IFileSystemManager fileSystemManager, IWebClientFactory webClientFactory)
        {
            this.FileSystemManager = fileSystemManager;
            this.WebClientFactory = webClientFactory;
        }

        IFileSystemManager FileSystemManager
        {
            get; set;
        }
        IWebClientFactory WebClientFactory { get; set; }

        public string ReadAllText(PathDescriptor path)
        {
            string location = path.ToFileSystemFormat();
            if (File.Exists(location))
                return FileSystemManager.ReadAllText(location);
            
            using (var webClient = WebClientFactory.CreateWebClient())
            {
                return webClient.DownloadString(path);
            }
        }
    }
}
