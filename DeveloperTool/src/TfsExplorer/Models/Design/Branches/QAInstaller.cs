using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Design
{
    public class QAInstaller : Installer<Folders.QAInstallerFolder>, IQAInstaller
    {
        public QAInstaller(QAInstallerFolder location, Guid installerId, VersionNumber version, IServiceLocator serviceLocator) 
            : base(location, installerId, version, serviceLocator)
        {
        }
        
        public override string GetDescription()
        {
            return $"QA Installer Version {this.Version}";
        }

        
        protected override void WriteHotfixTriggerContent()
        {
            var properties = new StringKeyValueCollection();
            properties.Add(new StringKeyValue(Build.AntPropertyNames.InstallerVersion, this.Version.ToString()));
            properties.Add(new StringKeyValue(Build.AntPropertyNames.TriggerDate, DateTime.Now.ToString()));
            this.Location.Parent.Parent.HotfixTrigger.TriggerIni.SetTextContent(properties.ToString());
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as QAInstaller;

            if (theOther == null)
                return false;

            return this.InstallerId == theOther.InstallerId;

        }

        public override int GetHashCode()
        {
            return this.InstallerId.GetHashCode();
        }

        public override string ToString()
        {
            return this.Version.ToString();
        }
        
    }
}
