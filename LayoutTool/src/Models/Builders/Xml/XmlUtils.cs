using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LayoutTool.Models.Builders.Xml
{
    internal static class XmlUtils
    {
        public static XDocument Parse(string xml)
        {
            return XDocument.Parse(xml.Replace(Convert.ToString((char)0x1F), string.Empty));
            //return XDocument.Parse(xml);
        }
    }
}
