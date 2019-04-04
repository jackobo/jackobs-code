using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.Publish;

namespace Spark.TfsExplorer.Models.Design
{
    public class ProductionInstaller : Installer<Folders.ProductionInstallerFolder>, IProductionInstaller
    {

        public ProductionInstaller(Folders.ProductionInstallerFolder location, 
                                   Guid installerId, 
                                   VersionNumber version, 
                                   IProductionEnvironment owner, 
                                   IServiceLocator serviceLocator)
            : base(location, installerId, version, serviceLocator)
        {

            this.Owner = owner;
        }

        IProductionEnvironment Owner { get; set; }
        
        public bool IsOwnedBy(IProductionEnvironment productionEnvironment)
        {
            return this.Owner.Equals(productionEnvironment);
        }
        
        public override string GetDescription()
        {
            return $"{this.Owner.Name} installer version {Version}";
        }
        
        

        protected override void WriteHotfixTriggerContent()
        {
            var properties = new StringKeyValueCollection();
            properties.Add(new StringKeyValue(Build.AntPropertyNames.InstallerVersion, this.Version.ToString()));
            properties.Add(new StringKeyValue(Build.AntPropertyNames.TriggerDate, DateTime.Now.ToString()));
            this.Location.Parent.Parent.HotfixTrigger.TriggerIni.SetTextContent(properties.ToString());

            properties = new StringKeyValueCollection();
            properties.Add(new StringKeyValue(Build.AntPropertyNames.ProductionEnvironment, this.Owner.Name));
            properties.Add(new StringKeyValue(Build.AntPropertyNames.TriggerDate, DateTime.Now.ToString()));
            this.Location.Parent.Parent.Parent.HotfixTrigger.TriggerIni.SetTextContent(properties.ToString());
        }


        public override bool Equals(object obj)
        {
            var theOther = obj as ProductionInstaller;

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
