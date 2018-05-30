using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{
    public class IconResourcesXmlFileDescriptor : XmlClientConfigurationFileDescriptor
    {
        public IconResourcesXmlFileDescriptor()
        {

        }

        public override string DefaultFileName
        {
            get
            {
                return "icon_resources.xmm";
            }
        }

        public override void ApplyToParser(IXmlSkinDefinitionParser parser, string xml)
        {
            
        }

       

        public override void ApplyToConverter(IXmlSkinDefinitionConverter converter, IClientConfigurationFile file)
        {
            converter.IconResources = file;
        }


        public override PathDescriptor GetRelativePath(SkinCode skinCode)
        {
            return new PathDescriptor($"navigation/plan/{DefaultFileName}");
        }
    }
}
