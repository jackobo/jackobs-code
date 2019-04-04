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
    public abstract class CompilableComponentContentProvider<TLocation> : ICompilableComponentContentProvider
        where TLocation : IFolderHolder
    {
        public CompilableComponentContentProvider(IComponentUniqueIdBuilder componentUniqueIdBuilder,
                                                   TLocation location, 
                                                   IEnumerable<IVisualStudioProject> visualStudioProjects, 
                                                   IBuildCustomizationProvider customizationProvider)
        {
            this.ComponentUniqueIdBuilder = componentUniqueIdBuilder;
            Location = location;
            VisualStudioPojects = visualStudioProjects;
            CustomizationProvider = customizationProvider;
        }

        protected IBuildCustomizationProvider CustomizationProvider { get; private set; }
        protected TLocation Location { get; private set; }
        protected IEnumerable<IVisualStudioProject> VisualStudioPojects { get; private set; }

        public IComponentUniqueIdBuilder ComponentUniqueIdBuilder { get; private set; }

        public abstract IServerPath GetProjectPath();
        
        public string Name
        {
            get { return Location.Name; }
        }

        public IEnumerable<IOutputFile> OutputFiles
        {
            get
            {
                var customizer = new BuildOutputFileCustomizer(this.VisualStudioPojects.SelectMany(p => p.OutputFilesNames),
                                                               GetCustomizedFiles());

                return customizer.Customize(CreateOutputFile);
            }
        }

        protected abstract IEnumerable<BuildOutputFileDefinition> GetCustomizedFiles();
        

        protected abstract IOutputFile CreateOutputFile(BuildOutputFileDefinition definition);

        

        public IEnumerable<string> ReferencedAssemblies
        {
            get
            {
                return this.VisualStudioPojects.SelectMany(p => p.ReferencedAssemblies).Distinct().ToArray();
            }
        }

        public IEnumerable<ILocalPath> AssemblyInfoFiles
        {
            get
            {
                return this.VisualStudioPojects.SelectMany(p => p.AssemblyInfoFile.Select(f => new LocalPath(f))).ToArray();
            }
        }
    }
}
