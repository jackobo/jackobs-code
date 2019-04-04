using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Models.Build
{
    public class VisualStudioProject : IVisualStudioProject
    {
        public VisualStudioProject(string absolutePath)
        {
            _project = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(absolutePath).FirstOrDefault();
            if(_project == null)
                _project = new Project(absolutePath);
        }

        Project _project;

        public IEnumerable<string> OutputFilesNames
        {
            get
            {
                return new string[]
                {
                    _project.GetAssemblyNameWithExtension(),
                    _project.GetAssemblyNameWithoutExtension() + ".pdb"
                };

                /*.Union(CopyToOutputDirFiles)
                .ToArray();*/
            }
        }
        
        public IEnumerable<string> CopyToOutputDirFiles
        {
            get
            {
                var result = new List<string>();
                foreach(var projectItem in _project.AllEvaluatedItems)
                {
                    if (projectItem.Metadata.Any(m => m.Name == VisualStudioNames.CopyToOutputDirectory))
                        result.Add(projectItem.EvaluatedInclude);
                }
                return result;
            }
        }

        public IEnumerable<string> ReferencedAssemblies
        {
            get
            {
                return _project.ResolveReferencedAssembliesPaths();
            }
        }

        public Optional<string> AssemblyInfoFile
        {
            get
            {
                var files = _project.GetCompilableFiles().ToArray();
                var assemblyInfoFiles = files.Where(f => f.EndsWith("AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase)).ToArray();
                if (assemblyInfoFiles.Length == 0)
                    return Optional<string>.None();

                if (assemblyInfoFiles.Length > 1)
                    throw new ApplicationException($"Too many AssemblyInfo.cs files detected for project {_project.FullPath}");

                return Optional<string>.Some(assemblyInfoFiles[0]);
            }
        }
    }
}
