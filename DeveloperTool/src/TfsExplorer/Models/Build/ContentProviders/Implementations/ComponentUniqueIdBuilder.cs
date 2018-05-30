using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class ComponentUniqueIdBuilder : IComponentUniqueIdBuilder
    {
        public ComponentUniqueIdBuilder(Folders.IFileHolder componentUniqueIdTxtFile)
        {
            _componentUniqueIdTxtFile = componentUniqueIdTxtFile;
            _componentUniqueId = new FileBasedComponentUniqueID(() => _componentUniqueIdTxtFile);
        }

        FileBasedComponentUniqueID _componentUniqueId;
        
        public IComponentUniqueId Id
        {
            get
            {
                return _componentUniqueId;
            }
        }

        Folders.IFileHolder _componentUniqueIdTxtFile;

        public Optional<IBuildAction> GetBuildAction()
        {
            if (_componentUniqueIdTxtFile.Exists())
                return Optional<IBuildAction>.None();

            return Optional<IBuildAction>.Some(new CreateComponentUniqueIdTxtAction(_componentUniqueIdTxtFile.GetServerPath(),
                                                                                    Id.Value));
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as ComponentUniqueIdBuilder;

            if (theOther == null)
                return false;

            return this.Id.Equals(theOther.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
