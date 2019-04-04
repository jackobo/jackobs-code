using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models
{
    public class MimeTypeDefinition
    {
        public MimeTypeDefinition(string extension, string mimeType)
        {
            this.Extension = extension;
            this.MimeType = mimeType;
        }

        public string Extension { get; set; }
        public string MimeType { get; set; }
    }
}
