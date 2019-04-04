using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class CompilableCoreComponentContentProvider : CompilableComponentContentProvider<CoreComponentFolder>, ICoreComponentContentProvider
    {
        public CompilableCoreComponentContentProvider(CoreComponentFolder location, IEnumerable<IVisualStudioProject> visualStudioProjects, IBuildCustomizationProvider customizationProvider)
            : base(new ComponentUniqueIdBuilder(location.ComponentUniqueIdTxt), location, visualStudioProjects, customizationProvider)
        {
        }

        protected override IEnumerable<BuildOutputFileDefinition> GetCustomizedFiles()
        {
            return this.CustomizationProvider.GetCoreComponentCustomizedOutputFiles(this.Name);
        }

        protected override IOutputFile CreateOutputFile(BuildOutputFileDefinition fileDefinition)
        {
            return new CoreComponentOutputFile(fileDefinition);
        }

    
        public override IServerPath GetProjectPath()
        {
            return this.Location.GetServerPath();
        }

        public Optional<CoreComponentCustomizationMetaData> GetCustomizationMetaData()
        {
            return this.CustomizationProvider.GetCoreComponentCustomizationMetaData(this.Name);
        }
    }
}
