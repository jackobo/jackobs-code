using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    internal class BranchBuilder 
    {
        List<ILogicalComponent> _sourceComponents;
        public BranchBuilder(Folders.IBranchFolder targetBranchFolder,
                             IEnumerable<ILogicalComponent> sourceComponents,
                             Folders.IBranchFolder sourceBranchFolder,
                             Action<ProgressCallbackData> progressCallback)
        {
            _targetBranchFolder = targetBranchFolder;
            _sourceComponents = sourceComponents.ToList();
            _sourceBranchFolder = sourceBranchFolder;
            _progressCallback = progressCallback;

        }

        Folders.IBranchFolder _targetBranchFolder;
        Folders.IBranchFolder _sourceBranchFolder;
        
        Action<ProgressCallbackData> _progressCallback;

        public void Build()
        {
            BranchComponents();
            BranchBuildTools();
            BranchSolutionFile();
        }

        private void BranchBuildTools()
        {
            if (_sourceBranchFolder.BuildTools.Exists())
            {
                _sourceBranchFolder.BuildTools.ToSourceControlFolder().Branch(_targetBranchFolder.BuildTools.GetServerPath());
            }

        }

        private void BranchSolutionFile()
        {
            if(_sourceBranchFolder.Components.GGPGameServerSln.Exists())
            {
                _sourceBranchFolder.Components.GGPGameServerSln.ToSourceControlFile()
                                    .Branch(_targetBranchFolder.Components.GetServerPath());
            }
            
        }
        

        private void BranchComponents()
        {
            Folders.ComponentsFolder targetComponentsFolder = _targetBranchFolder.Components.Create();
            
            for (int i = 0; i < _sourceComponents.Count; i++)
            {
                var component = _sourceComponents[i];
                try
                {
                    _progressCallback?.Invoke(ProgressCallbackData.Create(i,
                                                                        _sourceComponents.Count,
                                                                        "Branching " + component.Name));

                    component.As<ISupportBranching>().Do(c => c.Branch(targetComponentsFolder));

                    _progressCallback?.Invoke(ProgressCallbackData.Create(i + 1,
                                                                        _sourceComponents.Count,
                                                                        "Branching " + component.Name));
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Failed to branch component {component.Name}!{Environment.NewLine}Error details: {ex.Message}", ex);
                }
            }
        }
    }
}
