using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Artifactory
{
    //http://artifactorydev.888holdings.corp:8081/artifactory/ui/propertysets
    //http://artifactorydev.888holdings.corp:8081/artifactory/ui/propertysets/DL

    public class PropertySet
    {
        public string name { get; set; }
        public List<PropertyDefinition> properties { get; set; }
    }


    public class PropertyDefinition
    {
        public string name { get; set; }
        public PropertyType propertyType { get; set; }
        public List<PropertyPredefinedValue> predefinedValues { get; set; }

    }

    public class PropertyPredefinedValue
    {
        public string value { get; set; }
        public bool defaultValue { get; set; }
    }


    public enum PropertyType
    {
        ANY_VALUE,
        SINGLE_SELECT,
        MULTI_SELECT
    }
}
