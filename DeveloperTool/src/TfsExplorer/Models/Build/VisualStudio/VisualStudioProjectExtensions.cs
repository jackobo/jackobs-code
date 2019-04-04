using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace Spark.TfsExplorer.Models.Build
{
    public static class VisualStudioProjectExtensions
    {
        public static string GetAssemblyNameWithoutExtension(this Project project)
        {
            return project.GetProperty(VisualStudioNames.AssemblyName).EvaluatedValue;
        }

        public static string GetAssemblyNameWithExtension(this Project project)
        {
            return project.GetAssemblyNameWithoutExtension() + project.GetOutputExtension();
        }

        public static string GetOutputExtension(this Project project)
        {
            var outputType = project.GetProperty(VisualStudioNames.OutputType).EvaluatedValue;

            if (0 == string.Compare(outputType, VisualStudioNames.Library, true))
            {
                return ".dll";
            }


            if (0 == string.Compare(outputType, VisualStudioNames.exe, true)
                || 0 == string.Compare(outputType, VisualStudioNames.winexe, true))
            {
                return ".exe";
            }

            throw new InvalidOperationException($"Can't handle outputType {outputType} for project " + project.FullPath);

        }

        public static IEnumerable<string> ResolveReferencedAssembliesPaths(this Project project)
        {
            var result = new List<string>();
            foreach (var reference in project.GetItems(VisualStudioNames.Reference))
            {
                var hintPath = reference.Metadata.FirstOrDefault(m => m.Name == VisualStudioNames.HintPath);
                if (hintPath != null)
                {
                    var uri = new Uri(Path.Combine(hintPath.Project.DirectoryPath, hintPath.EvaluatedValue));
                    result.Add(Path.GetFullPath(uri.LocalPath));
                }
            }

            return result;
        }

        public static IEnumerable<string> GetCompilableFiles(this Project project)
        {
            var result = new List<string>();

            foreach (var compileItem in project.GetItems(VisualStudioNames.Compile))
            {                
                if(!string.IsNullOrEmpty(compileItem.EvaluatedInclude))
                {
                    var uri = new Uri(Path.Combine(compileItem.Project.DirectoryPath, compileItem.EvaluatedInclude));
                    result.Add(Path.GetFullPath(uri.LocalPath));
                }
            }

            return result;
        }
    }
}
