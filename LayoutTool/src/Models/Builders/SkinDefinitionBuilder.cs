using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Models.Builders
{
    public class SkinDefinitionBuilder : ISkinDefinitionBuilder
    {
        public SkinDefinitionBuilder(string clientVersion, 
                                     ISkinDefinitionParser parser,
                                     ISkinDefinitionConverter converter)
        {
            this.ClientVersion = clientVersion;
            this.Parser = parser;
            this.Converter = converter;
        }
        
        public string ClientVersion { get; private set; }

        private ISkinDefinitionParser Parser { get; set; }

        private ISkinDefinitionConverter Converter { get; set; }

        public IClientConfigurationFile[] Files
        {
            get;
            private set;
        }

        public SkinDefinitionContext Build()
        {
            return Parser.Parse();
        }

        public SkinConversionResult Convert(SkinDefinition skinDefinition)
        {
             return this.Converter.Convert(skinDefinition);
            
            
        }

    }
}
