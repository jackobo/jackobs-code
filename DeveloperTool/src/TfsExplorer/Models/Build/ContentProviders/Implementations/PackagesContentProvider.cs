using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class PackagesContentProvider : ICoreComponentContentProvider
    {
        public PackagesContentProvider(Folders.PackagesFolder location, IEnumerable<string> assemblies, IBuildCustomizationProvider customizationProvider)
        {
            this.Location = location;
            this.CustomizationProvider = customizationProvider;
            _assemblies = assemblies;
            this.ComponentUniqueIdBuilder = new ComponentUniqueIdBuilder(this.Location.ComponentUniqueIdTxt);
        }

        private Folders.PackagesFolder Location { get; set; }
        public string Name
        {
            get { return Location.Name; }
        }

        public IComponentUniqueIdBuilder ComponentUniqueIdBuilder { get; private set; }

        
        IBuildCustomizationProvider CustomizationProvider;

        IEnumerable<string> _assemblies;

        public IEnumerable<IOutputFile> OutputFiles
        {
            get
            {
                var customizer = new BuildOutputFileCustomizer(GetAssebliesNames(),
                                                              CustomizationProvider.GetCoreComponentCustomizedOutputFiles(this.Name));

                return customizer.Customize(fileDefinition => new CoreComponentOutputFile(fileDefinition));

               
            }
        }

        private IEnumerable<string> GetAssebliesNames()
        {
            return _assemblies.Select(assemblyFile => Path.GetFileName(assemblyFile))
                       .Union(ReadPdbFiles()
                       .Select(assemblyFile => Path.GetFileName(assemblyFile)));
        }

        IEnumerable<ILocalPath> ICompilableComponentContentProvider.AssemblyInfoFiles
        {
            get
            {
                return new ILocalPath[0];
            }
        }

        IEnumerable<string> ICompilableComponentContentProvider.ReferencedAssemblies
        {
            get
            {
                return new string[0];
            }
        }

        private IEnumerable<string> ReadPdbFiles()
        {
            return _assemblies.Select(assemblyFile => Path.Combine(Path.GetDirectoryName(assemblyFile),
                                                        Path.GetFileNameWithoutExtension(assemblyFile) + ".pdb"))
                       .Where(pdbFile => File.Exists(pdbFile))
                       .ToList();
        }

        public IServerPath GetProjectPath()
        {
            return this.Location.GetServerPath();
        }

        public Optional<CoreComponentCustomizationMetaData> GetCustomizationMetaData()
        {
            return this.CustomizationProvider.GetCoreComponentCustomizationMetaData(this.Name);
        }
    }
}
