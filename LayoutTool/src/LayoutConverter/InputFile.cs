using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutConverter
{
    public class InputFile : IClientConfigurationFile
    {
        public InputFile(string fileName, string content, PathDescriptor location)
        {
            this.FileName = fileName;
            this.Content = content;
            this.Location = location;
        }

        public string FileName { get; private set; }
        public PathDescriptor Location { get; private set; }
        public string Content { get; private set; }

        
    }
}
