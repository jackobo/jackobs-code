using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.CCK.FilesDescriptors
{
    public class LanguageJsonFileDescriptor : IJsonConfigurationFileDescriptor
    {
        public LanguageJsonFileDescriptor(PathDescriptor relativePath)
        {
            _relativePath = relativePath;
        }
        public string DefaultFileName
        {
            get
            {
                return "language.json";
            }
        }

        PathDescriptor _relativePath;
        public PathDescriptor GetRelativePath(SkinCode skinCode)
        {
            return _relativePath;
        }

        public void ApplyToParser(IJsonSkinDefinitionParser parser, IClientConfigurationFile file)
        {
            parser.LanguageJson = file;
        }

        public void ApplyToConverter(IJsonSkinDefinitionConverter converter, IClientConfigurationFile file)
        {
            converter.Language = file;
        }
    }
}
