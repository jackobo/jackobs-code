using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GGPMockBootstrapper
{
    public static class UtilExtensions
    {
        public static TObject DeserializeObjectFromFile<TObject>(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TObject));

            using (var fileStream = File.OpenRead(fileName))
            {
                return (TObject)serializer.Deserialize(fileStream);
            }
        }

        public static void SerializeObjectToFile<TObject>(this TObject obj, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TObject));

            //var productFolders = InstallerFolders.GetProductFolders(deploymentLog.ProductId);

            if (File.Exists(fileName))
                File.Delete(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));


            using (TextWriter WriteFileStream = new StreamWriter(fileName))
            {
                serializer.Serialize(WriteFileStream, obj);
            }
        }
    }
}
