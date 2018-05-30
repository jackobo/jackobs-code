using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Build;

namespace GGPInstallerBuilder
{
    public class InstallerDefinitionReader : IInstallerDefinitionReader
    {
        ISourceControlAdapter _sourceControlAdapter;
        public InstallerDefinitionReader(ISourceControlAdapter sourceControlAdapter)
        {
            _sourceControlAdapter = sourceControlAdapter;
        }

        public InstallerDefinition Read(BuildTaskInfo taskInfo)
        {
            var branchServerPath = _sourceControlAdapter
                                        .CreateServerPath(taskInfo.InstallerContentRootServerPath)
                                        .Subpath(taskInfo.Environment.ToString())
                                        .Subpath(taskInfo.BranchName);

            var installerDefinition = CreateInstallerDefinitionHeader(branchServerPath);

            AppendInstallerContent(installerDefinition, branchServerPath);

            return installerDefinition;
        }

        private void AppendInstallerContent(InstallerDefinition installerDefinition, IServerPath branchServerPath)
        {

            var installerContentTxtPath = branchServerPath.Subpath(Constants.Versions)
                                                          .Subpath(installerDefinition.Version.ToString())
                                                          .Subpath(Constants.InstallerContentTxt);

            foreach (var keyValue in StringKeyValueCollection.Parse(_sourceControlAdapter.ReadTextFile(installerContentTxtPath)))
            {
                if(keyValue.Value.StartsWith("$/"))
                    installerDefinition.Components.Add(new InstallerDefinition.ComponentDefinition(keyValue.Name, _sourceControlAdapter.CreateServerPath(keyValue.Value)));
                else //backward compatibility with the installers up to 1.5.x
                    installerDefinition.Components.Add(new InstallerDefinition.ComponentDefinition(keyValue.Name, _sourceControlAdapter.CreateServerPath(ConfigurationManager.AppSettings[ConfigurationKeys.oldDistributionPath]).Subpath(keyValue.Value)));
            }
        }

        private InstallerDefinition CreateInstallerDefinitionHeader(IServerPath branchServerPath)
        {
            var latestTxtFileContent = _sourceControlAdapter.ReadTextFile(branchServerPath.Subpath(Constants.LatestTxt));
            return new InstallerDefinition(latestTxtFileContent);
            
        }
    }
}
