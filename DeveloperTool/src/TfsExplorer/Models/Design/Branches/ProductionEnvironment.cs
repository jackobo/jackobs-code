using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public class ProductionEnvironment : IProductionEnvironment
    {
        public ProductionEnvironment(Guid id,
                                     string name, 
                                     Folders.ProductionEnvironmentFolder location, 
                                     IRootBranch owner, 
                                     IServiceLocator serviceLocator)
        {
            _location = location;
            _id = id;
            Name = name;
            _owner = owner;
            _serviceLocator = serviceLocator;
        }


        Folders.ProductionEnvironmentFolder _location;
        Guid _id;
        IRootBranch _owner;

        IServiceLocator _serviceLocator;
        public string Name { get; private set; }


        InstallerSynchronizer<IProductionInstaller, NewProductionInstallerEventData> _installerSynchronizer;
        public IEnumerable<IProductionInstaller> GetInstallers()
        {
            if (_installerSynchronizer == null)
            {
                _installerSynchronizer = new InstallerSynchronizer<IProductionInstaller, NewProductionInstallerEventData>(
                                                ReadInstallers,
                                                installer => new NewProductionInstallerEventData(installer, this),
                                                _serviceLocator);
            }

            return _installerSynchronizer.CurrentInstallers;
        }
        
        private IEnumerable<IProductionInstaller> ReadInstallers()
        {
            using (var proxy = DeveloperToolServiceProxyFactory.CreateProxy())
            {
                var response = proxy.GetProductionInstallers(new DeveloperToolService.GetProductionInstallersRequest()
                {
                    EnvironmentId = _id,
                    BranchName = _owner.Version.ToString()
                });

                return response.Installers
                        .Select(e =>
                        {
                            var installerVersion = new VersionNumber(e.Version);
                            return new ProductionInstaller(this._location.Installers.Installer(installerVersion),
                                                           e.Id,
                                                           installerVersion,
                                                           this,
                                                           _serviceLocator);
                        })
                               .ToList();
            }
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as ProductionEnvironment;
            if (theOther == null)
                return false;

            return this._id == theOther._id
                    && this._owner.Equals(theOther._owner);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode()
                    ^ this._owner.GetHashCode();
        }
    }
}
