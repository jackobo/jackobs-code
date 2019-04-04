using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Newtonsoft.Json.Linq;

namespace LayoutTool.Models.Builders.CCK
{
    public static class JsonUtils
    {
        public static string ExtractPlayerStatus(dynamic playerStatusArray)
        {
            if (playerStatusArray == null)
                return string.Empty;

            var array = playerStatusArray as JArray;

            if (array == null)
                return string.Empty;

            var firstItem = array.FirstOrDefault() as JValue;

            if (firstItem == null)
                return string.Empty;


            return (firstItem.Value?.ToString() ?? string.Empty);
        }

        public static List<AttributeValue> ExtractAllAttributes(JObject jObject)
        {
            
            var attributes = new List<AttributeValue>();
            foreach (var property in jObject.Properties())
            {
                if (property.Value.GetType() == typeof(JValue))
                {
                    attributes.Add(new AttributeValue(property.Name, (string)property.Value));
                }
            }

            return attributes;
        }

        public static bool IsPlayerStatusEmpty(dynamic playerStatusArray)
        {
            return string.IsNullOrEmpty(ExtractPlayerStatus(playerStatusArray));
        }

        public static JArray BuildPlayerStatusArray(string playerStatus)
        {
            if(string.IsNullOrEmpty(playerStatus ))
            {
                return new JArray();
            }

            return new JArray(new JValue(playerStatus));
        }

        public static bool HasProperty(dynamic obj, string propertyName)
        {
            var jobject = obj as JObject;
            if (jobject == null)
            {
                if(obj is JProperty)
                {
                    jobject = ((JProperty)obj).Value as JObject;
                }
            }

            if (jobject == null)
                return false;

            return jobject.Property(propertyName) != null;
        }
    }
}
