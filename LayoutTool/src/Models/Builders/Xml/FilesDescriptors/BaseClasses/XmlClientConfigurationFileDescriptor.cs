using LayoutTool.Interfaces;


namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{

  
    public abstract class XmlClientConfigurationFileDescriptor : IXmlConfigurationFileDescriptor
    {
        
        public XmlClientConfigurationFileDescriptor()
        {
        }

        public abstract string DefaultFileName { get; }
        public abstract void ApplyToParser(IXmlSkinDefinitionParser parser, string xml);


        public abstract PathDescriptor GetRelativePath(SkinCode skinCode);
                
        
        public abstract void ApplyToConverter(IXmlSkinDefinitionConverter converter, IClientConfigurationFile file);

        /*
        public virtual PathDescriptor BuildFullPath(PathDescriptor rootFolder, SkinCode skinCode, ABTestCase testCase)
        {
            if(testCase != null)
            {
                var overrideFilePath = GetOverrideFileRelativePathOrNull(testCase, skinCode);
                if (overrideFilePath != null)
                    return rootFolder + overrideFilePath;
            }

            return rootFolder + GetRelativePath(skinCode);

        }

        public bool IsAffectedByTheTestCase(ABTestCase testCase, SkinCode skinCode)
        {
            return null != GetOverrideFileRelativePathOrNull(testCase, skinCode);
        }

    */
       
        
    }
   
}
