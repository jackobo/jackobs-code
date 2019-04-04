using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service.Artifactory
{
    //http://artifactorydev.888holdings.corp:8081/artifactory/ui/propertysets
    //http://artifactorydev.888holdings.corp:8081/artifactory/ui/propertysets/DL




    public class PropertySet : ArtifactoryResponse
    {
        public PropertySet()
        {
            this.properties = new List<PropertyDefinition>();
        }

        public PropertySet(string name, params PropertyDefinition[] properties)
        {
            this.name = name;
            this.properties = (properties ?? new PropertyDefinition[0]).ToList();
        }

        public string name { get; set; }
        public List<PropertyDefinition> properties { get; set; }


        public override string ToString()
        {
            return this.name + string.Format(" [properties count: {0}]", this.properties.Count);
        }
    }



   

    public class PropertyDefinition
    {
        public PropertyDefinition()
        {
            this.predefinedValues = new List<PropertyPredefinedValue>();
        }

        public PropertyDefinition(string name, PropertyType propertyType, params PropertyPredefinedValue[] predefinedValues)
        {
            this.name = name;
            this.propertyType = propertyType;
            this.predefinedValues = (predefinedValues ?? new PropertyPredefinedValue[0]).ToList();
        }

        public string name { get; set; }
        public PropertyType propertyType { get; set; }
        public List<PropertyPredefinedValue> predefinedValues { get; set; }

    }

    public class PropertyPredefinedValue
    {
        public PropertyPredefinedValue()
        {

        }

        public PropertyPredefinedValue(string value, bool defaultValue)
        {
            this.value = value;
            this.defaultValue = defaultValue;
        }

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
