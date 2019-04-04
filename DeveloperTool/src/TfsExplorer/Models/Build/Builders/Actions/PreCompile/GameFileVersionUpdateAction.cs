using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.TFS;

namespace Spark.TfsExplorer.Models.Build
{
    public class GameFileVersionUpdateAction : IBuildAction
    {
        private ILocalPath _file;
        private VersionNumber _version;

        public GameFileVersionUpdateAction(ILocalPath file, VersionNumber version)
        {
            _file = file;
            _version = version;
        }

        private static readonly string VERSION_ELEMENT = "Version";

        public void Execute(IBuildContext buildContext)
        {
            try
            {
                var fileSystemAdapter = buildContext.FileSystemAdapter;
                var logger = buildContext.Logger;

                logger.Info($"Checkout for edit: {_file}");
                buildContext.SourceControlAdapter.CheckoutForEdit(_file);

                logger.Info($"Parsing xml file : {_file}");
                var xmlDoc = XDocument.Parse(fileSystemAdapter.ReadAllText(_file));

                logger.Info($"Update version to {_version} for xml file : {_file}");
                var versionElement = xmlDoc.Root.Element(VERSION_ELEMENT);
                if (versionElement == null)
                {
                    versionElement = new XElement(VERSION_ELEMENT);
                    xmlDoc.Root.AddFirst(versionElement);
                }
                versionElement.Value = _version.ToString();

                logger.Info($"Writting file : {_file}");
                xmlDoc.Save(_file.AsString());
            }
            catch(Exception ex)
            {
                throw new ApplicationException($"Failed to update version for file {_file}", ex);
            }

        }
    }
}
