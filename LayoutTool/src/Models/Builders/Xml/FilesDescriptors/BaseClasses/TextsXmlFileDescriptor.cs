using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{
    public abstract class TextsXmlFileDescriptor : XmlClientConfigurationFileDescriptor
    {

        public override PathDescriptor GetRelativePath(SkinCode skinCode)
        {
            return new PathDescriptor($"navigation/language/en/{DefaultFileName}");
        }
    }
}
