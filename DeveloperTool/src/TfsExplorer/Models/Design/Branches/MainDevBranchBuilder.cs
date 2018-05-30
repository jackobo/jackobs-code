using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    

    public class MainDevBranchBuilder : IMainDevBranchBuilder
    {
        
        public MainDevBranchBuilder()
        {
        }

        bool InProgress
        {
            get;set;
        }

        public bool CanBuild(Folders.RootBranchFolder logicalBranchFolder)
        {
            return !(InProgress || DevBranchExists(logicalBranchFolder));
        }
        
        private bool DevBranchExists(Folders.RootBranchFolder logicalBranchFolder)
        {
            return logicalBranchFolder.DEV.Main.Components.Exists();
        }
        
        public void Build(Folders.RootBranchFolder logicalBranchFolder, 
                          IEnumerable<ILogicalComponent> sourceComponents,
                          Action<ProgressCallbackData> progressCallback = null)
        {
            if (logicalBranchFolder == null)
                throw new ArgumentNullException(nameof(logicalBranchFolder));

            if (sourceComponents == null)
                throw new ArgumentNullException(nameof(sourceComponents));

            if (DevBranchExists(logicalBranchFolder))
                throw new InvalidOperationException($"DEV branch already exists in {logicalBranchFolder.Name}");
            
            if (InProgress)
                throw new InvalidOperationException("DEV branch creation already in progress");



            InProgress = true;

            try
            {
                var branchBuilder = new BranchBuilder(logicalBranchFolder.DEV.Main.Create(), 
                                                      sourceComponents, 
                                                      logicalBranchFolder.QA.Main,
                                                      progressCallback);
                branchBuilder.Build();
            }
            finally
            {
                InProgress = false;
            }
        }
        
       
    }
}
