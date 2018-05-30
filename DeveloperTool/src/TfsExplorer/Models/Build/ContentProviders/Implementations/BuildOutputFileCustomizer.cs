using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class BuildOutputFileCustomizer
    {
        IEnumerable<string> _regularOutputFilesNames;
        IEnumerable<BuildOutputFileDefinition> _customizedFiles;
        public BuildOutputFileCustomizer(IEnumerable<string> regularOutputFilesNames,
                                         IEnumerable<BuildOutputFileDefinition> customizedFiles)
        {
            _regularOutputFilesNames = regularOutputFilesNames;
            _customizedFiles = customizedFiles;
        }

        public IEnumerable<IOutputFile> Customize(Func<BuildOutputFileDefinition, IOutputFile> createOutputFile)
        {
            var result = new List<IOutputFile>();

            var customizedFilesDictionary = _customizedFiles.ToDictionary(f => f.BuildOutputRelativePath,
                                                                    f => f,
                                                                    StringComparer.OrdinalIgnoreCase);
            foreach (var fileName in _regularOutputFilesNames)
            {
                if (customizedFilesDictionary.ContainsKey(fileName))
                {
                    result.Add(createOutputFile(customizedFilesDictionary[fileName]));
                    customizedFilesDictionary.Remove(fileName);
                }
                else
                {
                    result.Add(createOutputFile(new BuildOutputFileDefinition(fileName, fileName, DeployEnvironment.All)));
                }
            }

            foreach (var remainingCustomizedFile in customizedFilesDictionary)
            {
                result.Add(createOutputFile(remainingCustomizedFile.Value));
            }

            return result;
        }
    }
}
