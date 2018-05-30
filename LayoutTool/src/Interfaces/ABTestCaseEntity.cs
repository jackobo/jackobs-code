using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LayoutTool.Interfaces
{

    public class ABTestConfiguration
    {
        public ABTestConfiguration()
        {
            
        }

        public ABTestConfiguration(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        
        public string Id { get; set; }

        
        public string Name { get; set; }
        
        
        public string Description { get; set; }


        List<ABTestCaseSet> _testCaseSets = new List<ABTestCaseSet>();
        public List<ABTestCaseSet> TestCaseSets
        {
            get { return _testCaseSets; }
            set { _testCaseSets = value;}
        }



        public ABTestCaseSet[] GetApplicableTestCases(SkinCode skinCode)
        {
            var testCases = TestCaseSets.Where(tc => tc.BrandId == skinCode.BrandId.ToString() && tc.SkinId == skinCode.SkinId.ToString())
                                                         .ToArray();

            if (testCases.Length > 0)
                return testCases;

            testCases = TestCaseSets.Where(tc => tc.BrandId == skinCode.BrandId.ToString() && tc.SkinId == "*")
                                                     .ToArray();

            if (testCases.Length > 0)
                return testCases;


            testCases = TestCaseSets.Where(tc => tc.BrandId == "*" && tc.SkinId == skinCode.SkinId.ToString())
                                                   .ToArray();

            if (testCases.Length > 0)
                return testCases;


            testCases = TestCaseSets.Where(tc => tc.BrandId == "*" && tc.SkinId == "*")
                                                 .ToArray();

            if (testCases.Length > 0)
                return testCases;

            return new ABTestCaseSet[0];
        }
    }


    public enum ABTestMethod
    {
        Override = 1,
        FullClient = 2
    }

    public class ABTestCase
    {
        public ABTestCase(string id, bool isDefault, ABTestMethod method, string name, decimal usePercentage, string description, PathDescriptor clientPath)
        {
            this.Id = id;
            this.IsDefault = isDefault;
            this.Method = method;
            this.Name = name;
            this.UsePercentage = usePercentage;
            this.Description = description;
            this.ClientPath = clientPath;
            this.FilesOverride = new ABTestFileOverrideCollection();
        }

        
        
        public string Id { get; set; }

        
        public bool IsDefault { get; set; }

        
        public ABTestMethod Method { get; set; }

        
        public string Name { get; set; }

        
        public decimal UsePercentage { get; set; }

        
        public string Description { get; set; }

        
        public PathDescriptor ClientPath { get; set; }


        public ABTestFileOverrideCollection FilesOverride { get; set; }

        public override string ToString()
        {
            return $"{Name} [{UsePercentage}%]";
        }

        public bool IsFileAffected(IConfigurationFileDescriptor fileDescriptor, SkinCode skinCode)
        {
            return null != GetOverrideFileOrNull(fileDescriptor, skinCode);
        }

        public PathDescriptor GetOverrideFileOrNull(IConfigurationFileDescriptor fileDescriptor, SkinCode skinCode)
        {

            return GetOverrideFileOrNull(fileDescriptor.GetRelativePath(skinCode), skinCode);

        }


        public PathDescriptor GetOverrideFileOrNull(PathDescriptor fileRelativePath, SkinCode skinCode)
        {

            if (this.Method == ABTestMethod.FullClient)
                return fileRelativePath;

            foreach (var fileOverride in this.FilesOverride)
            {
                if (fileOverride.GetOriginalFile(skinCode).EndsWith(fileRelativePath))
                    return fileOverride.OverrideFile;
            }


            return null;
        }

        public string GetOverrideParameterPathUrlParameter()
        {
            if (this.Method == ABTestMethod.Override)
            {
                return "&ABTestingOverridePath=" + ClientPath.ToHttpUrlFormat();
            }

            return null;
        }
    }

    public class ABTestFileOverride
    {
        public ABTestFileOverride()
        {
        }

        public ABTestFileOverride(string originalFile, string overrideFile)
        {
            _originalFile = new PathDescriptor(originalFile);
            this.OverrideFile = new PathDescriptor(overrideFile);
        }


        PathDescriptor _originalFile;
        public PathDescriptor OverrideFile { get; private set; }

        public PathDescriptor GetOriginalFile(SkinCode skinCode)
        {
            return _originalFile.Replace("%BRAND%", skinCode.BrandId.ToString())
                                 .Replace("%SKIN%", skinCode.SkinId.ToString());
        }


    }

    public class ABTestFileOverrideCollection : List<ABTestFileOverride>
    {

    }


    public class ABTestCaseSet  : List<ABTestCase>
    {
        public ABTestCaseSet() 
        {
        }

        public ABTestCaseSet(string id, string brandId, string skinId, string language)
        {
            Id = id;
            BrandId = brandId;
            SkinId = skinId;
            Language = language;
        }

       

        [XmlAttribute("ID")]
        public string Id { get; set; }

        [XmlAttribute("brand")]
        public string BrandId { get; set; }

        [XmlAttribute("skin")]
        public string SkinId { get; set; }

        [XmlAttribute("lang")]
        public string Language { get; set; }
        

        public bool AppliesTo(SkinCode skinCode, IEnumerable<IConfigurationFileDescriptor> fileDescriptors)
        {
            
            if (this.Any(t => t.Method == ABTestMethod.FullClient))
                return true;

            foreach(var test in this.Where(t => t.Method == ABTestMethod.Override))
            {
                if (fileDescriptors.Any(fd => test.IsFileAffected(fd, skinCode)))
                    return true;
                
            }
            return false;
        }


    }
}
