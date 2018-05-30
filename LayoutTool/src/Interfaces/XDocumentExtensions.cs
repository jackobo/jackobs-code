using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Interfaces
{
    public static class XDocumentExtensions
    {
        public static string GetAttributeValue(this XElement element, string attributeName)
        {
            if (element == null)
                return null;

            var attribute = element.Attribute(attributeName);

            if (attribute == null)
                return null;

            return attribute.Value;
        }



        public static T GetAttributeValue<T>(this XElement element, string attributeName)
        {
            return element.GetAttributeValue<T>(attributeName, default(T));
        }


        public static List<AttributeValue> ExtractAllAttributes(this XElement element)
        {
            var result = new List<AttributeValue>();

            if (element == null)
                return result;

            foreach(var attr in element.Attributes())
            {
                result.Add(new AttributeValue(attr.Name.LocalName, attr.Value));
            }

            return result;
        }

        public static T GetAttributeValue<T>(this XElement element, string attributeName, T defaultIfMissing)
        {
            if (element == null)
                return defaultIfMissing;

            var attribute = element.Attribute(attributeName);

            if (attribute == null)
                return defaultIfMissing;

            var converter = TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromInvariantString(attribute.Value);
        }


        public static void CopyAttributesTo(this XElement sourceElement, XElement destinationElement)
        {
            foreach (var attribute in sourceElement.Attributes())
            {
                destinationElement.Add(new XAttribute(attribute.Name, attribute.Value));
            }
        }


        public static void CopyAllElementsExcept(this XElement sourceElement, XElement destinationElement, params XName[] childElementsToExclude)
        {
            HashSet<XName> excluded = new HashSet<XName>(childElementsToExclude);
            foreach (var element in sourceElement.Elements())
            {
                if (excluded.Contains(element.Name))
                    continue;

                destinationElement.Add(element);
            }
        }

        public static void AddOrUpdateAttributeValue(this XElement element, XName attributeName, object value)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                attribute = new XAttribute(attributeName, value);
                element.Add(attribute);
            }
            else
            {
                attribute.SetValue(value);
            }

        }

    }
}
