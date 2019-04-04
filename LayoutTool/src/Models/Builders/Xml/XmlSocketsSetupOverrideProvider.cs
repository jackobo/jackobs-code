using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml
{
    public class XmlSocketsSetupOverrideProvider : SocketsSetupOverrideProviderBase
    {
        public XmlSocketsSetupOverrideProvider(PathDescriptor filePath, int mainProxyPort)
            : base(filePath, mainProxyPort)
        {
            _filePath = filePath;
        }

        PathDescriptor _filePath;
        
      
        protected override string HijackSocketSetup(string socketSetupContent, int mainProxyPort)
        {
            try
            {
                var xmlDoc = XmlUtils.Parse(socketSetupContent);

                var communicationElement = xmlDoc.Root.Element("communication");
                if (communicationElement == null)
                    return socketSetupContent;



                communicationElement.AddOrUpdateAttributeValue("currentProxy", "socketProxy");


                var socketProxyElement = communicationElement.Element("socketProxy");

                if (socketProxyElement == null)
                {
                    socketProxyElement = new XElement("socketProxy");
                }

                socketProxyElement.AddOrUpdateAttributeValue("config", "MOCK");

                var mockConnectionElement = socketProxyElement.Elements("connection")
                                                        .FirstOrDefault(element => element.GetAttributeValue("name") == "MOCK");

                if (mockConnectionElement == null)
                {
                    mockConnectionElement = new XElement("connection");
                    mockConnectionElement.AddOrUpdateAttributeValue("name", "MOCK");
                    socketProxyElement.Add(mockConnectionElement);

                }


                mockConnectionElement.AddOrUpdateAttributeValue("IP", "127.0.0.1");
                mockConnectionElement.AddOrUpdateAttributeValue("practiceIP", "127.0.0.1");
                mockConnectionElement.AddOrUpdateAttributeValue("realPort", mainProxyPort);
                mockConnectionElement.AddOrUpdateAttributeValue("practicePort", mainProxyPort);

                return xmlDoc.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return socketSetupContent;
            }
        }
    }
}
