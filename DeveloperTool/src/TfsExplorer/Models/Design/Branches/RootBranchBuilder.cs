using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    internal class RootBranchBuilder 
    {
        public RootBranchBuilder(TFS.ITfsGateway tfsGateway, 
                                 Func<ISourceControlFolder, IRootBranch> createRootBranchInstance,
                                 Func<IPublishPayloadBuilder> publishPayloadBuilderFactory)
        {
            _tfsGateway = tfsGateway;
            _createRootBranchInstance = createRootBranchInstance;
            _publishPayloadBuilderFactory = publishPayloadBuilderFactory;
        }


        TFS.ITfsGateway _tfsGateway;
        Func<ISourceControlFolder, IRootBranch> _createRootBranchInstance;
        Func<IPublishPayloadBuilder> _publishPayloadBuilderFactory;

        public IRootBranch Build(IRootBranchExtended sourceBranch, Action<ProgressCallbackData> progressCallback)
        {

            //branch components
            var newRootBranchFolder = BranchComponents(sourceBranch,
                                                      branchingProgress =>
                                                      {
                                                          //branching progress represents 80% from total 
                                                          var newPercentage = branchingProgress.Percentage * 80m / 100m;
                                                          progressCallback?.Invoke(new ProgressCallbackData(newPercentage, branchingProgress.ActionDescription));
                                                      });

            //create brach instance
            progressCallback?.Invoke(new ProgressCallbackData(80, $"Create first Publish.xml"));
            var newBranchInstance = _createRootBranchInstance(newRootBranchFolder.ToSourceControlFolder());
            DoTheFirstPublish(newBranchInstance);

            //create branch in the approval system
            progressCallback?.Invoke(new ProgressCallbackData(85, $"Create branch in the GGP Approval System"));
            CreateBranchInApprovalSystem(sourceBranch.Version);

            //GetLatest
            progressCallback?.Invoke(new ProgressCallbackData(95, $"GetLatest for {newRootBranchFolder.GetServerPath().AsString()}"));
            newRootBranchFolder.ToSourceControlFolder().GetLatest();
            
            progressCallback?.Invoke(new ProgressCallbackData(100, "Done"));

            return newBranchInstance;

           
        }

        private void CreateBranchInApprovalSystem(RootBranchVersion sourceBranchVersion)
        {
            using (var proxy = DeveloperToolServiceProxyFactory.CreateProxy())
            {
                proxy.CreateNewBranch(new DeveloperToolService.CreateNewBranchRequest()
                {
                    SourceBranchName = sourceBranchVersion.ToString(),
                    TargetBranchName = (sourceBranchVersion + 1).ToString()
                });
            }
        }

        private Folders.IRootFolder BranchComponents(IRootBranchExtended sourceBranch, Action<ProgressCallbackData> progressCallback)
        {
            
            var newRootFolder = _tfsGateway.CreateRootFolder(sourceBranch.Version + 1);
            new BranchBuilder(newRootFolder.QA.Main,
                             sourceBranch.GetQaBranch().GetComponents(),
                             sourceBranch.Location.QA.Main,
                             progressCallback)
                .Build();
            return newRootFolder;
            

            //return _tfsGateway.GetRootFolder(sourceBranch.Version + 1);
        }

        private void DoTheFirstPublish(IRootBranch newBranchInstance)
        {
            var firstPublishPayloadBuilder = new FirstPublishPayloadBuilder(_publishPayloadBuilderFactory(),
                                                                           newBranchInstance.Version.GetFirstVersion());

            foreach (var comp in newBranchInstance.GetQaBranch().GetComponents())
            {
                comp.AcceptCommandVisitor(() => firstPublishPayloadBuilder);
            }

            newBranchInstance.GetQaBranch().Publish(firstPublishPayloadBuilder.Build());
        }

       

        private class FirstPublishPayloadBuilder : ILogicalComponentVisitor
        {
            IPublishPayloadBuilder _publishPayloadBuilder;
            VersionNumber _version;
            public FirstPublishPayloadBuilder(IPublishPayloadBuilder publishPayloadBuilder, VersionNumber version)
            {
                _publishPayloadBuilder = publishPayloadBuilder;
                _version = version;
            }

            public IPublishPayload Build()
            {
                return _publishPayloadBuilder.Build();
            }

            void ILogicalComponentVisitor.Visit(IGameComponent game)
            {
            }

            void ILogicalComponentVisitor.Visit(IServerPath location)
            {
            }

            void ILogicalComponentVisitor.Visit(IGameEngineComponent gameEngine)
            {
                _publishPayloadBuilder.AddGameEngine(gameEngine.EngineName, _version);
            }

            void ILogicalComponentVisitor.Visit(ICoreComponent coreComponent)
            {
                _publishPayloadBuilder.AddCoreComponent(coreComponent.Name, _version);
            }
        }

    }
}
