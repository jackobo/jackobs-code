using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.CCK.FilesDescriptors
{
    public class NavigationPlanJsonFileDescriptor : IJsonConfigurationFileDescriptor
    {
        public NavigationPlanJsonFileDescriptor(PathDescriptor relativePath)
        {
            _relativePath = relativePath;
        }

        PathDescriptor _relativePath;
        public string DefaultFileName
        {
            get
            {
                return "navigation_plan.json";
            }
        }

        public PathDescriptor GetRelativePath(SkinCode skinCode)
        {
            return _relativePath;
        }

        public void ApplyToParser(IJsonSkinDefinitionParser parser, IClientConfigurationFile file)
        {
            parser.NavigationPlanJson = file;
        }

        public void ApplyToConverter(IJsonSkinDefinitionConverter converter, IClientConfigurationFile file)
        {
            converter.NavigationPlan = file;
        }
    }
}
