using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.DeveloperToolService;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Design
{
    public class InstallerBranchBuilder
    {
        public InstallerBranchBuilder(TFS.ITfsGateway tfsGateway,
                                     Func<IPublishPayloadBuilder> publishPayloadBuilderFactory,
                                     IPublishPayloadSerializer publishPayloadSerializer)
        {
            _tfsGateway = tfsGateway;
            _publishPayloadBuilderFactory = publishPayloadBuilderFactory;
            _publishPayloadSerializer = publishPayloadSerializer;
        }

        public Action<ProgressCallbackData> ProgressCallback { get; set; }

        
        TFS.ITfsGateway _tfsGateway;
        Func<IPublishPayloadBuilder> _publishPayloadBuilderFactory;
        IPublishPayloadSerializer _publishPayloadSerializer;


        public void Build(Guid installerId,
                          Folders.IInstallerFolder installerFolder)
        {
            var qaMainFolder = installerFolder.Root.QA.Main;

            CallProgress(0, "Reading installer content");

            var installerContent = ReadInstallerContent(installerId);

            var actions = GetActionsList(installerContent, qaMainFolder, installerFolder);

            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                CallProgress((decimal)i / (decimal)actions.Count, action.Description);
                action.Execute();
                CallProgress((decimal)(i + 1) / (decimal)actions.Count, action.Description);
            }
        }


        private GetInstallerContentResponse ReadInstallerContent(Guid installerId)
        {
            using (var proxy = DeveloperToolServiceProxyFactory.CreateProxy())
            {
                return proxy.GetInstallerContent(new GetInstallerContentRequest() { InstallerId = installerId });
            }
        }
        private void CallProgress(decimal percentage, string description)
        {
            if (ProgressCallback == null)
                return;

            ProgressCallback(new ProgressCallbackData(percentage * 100, description));
        }

        private List<BuildAction> GetActionsList(GetInstallerContentResponse installerContent, 
                                                 Folders.QAMainFolder qaMainFolder,
                                                 Folders.IInstallerFolder installerFolder)
        {
            var actions = new List<BuildAction>();
            actions.AddRange(GetBranchCoreComponentsActions(installerFolder.Components, installerContent));
            actions.AddRange(GetBranchEnginesAndGamesActions(installerFolder.Components, installerContent));
            actions.Add(new BuildAction("Branch BuildTools", () => BranchBuildTools(qaMainFolder.BuildTools, installerFolder.BuildTools)));
            actions.Add(new BuildAction("Branch GGP Solution file", () => BranchSolutionFile(qaMainFolder, installerFolder)));
            actions.Add(new BuildAction("Branch BuildCustomization.xml", () => BranchBuildCustomizationXmlFile(qaMainFolder, installerFolder)));
            actions.Add(new BuildAction("Create Publish.xml file", () => CreatePublishXmlFile(installerFolder, installerContent)));
            actions.Add(new BuildAction("Create LatestPublish.xml file", () => CreateLatestPublishXmlFile(installerFolder, installerContent)));
            actions.Add(new BuildAction($"Get Latest for {installerFolder.GetServerPath().AsString()}", () => installerFolder.ToSourceControlFolder().GetLatest()));
            return actions;
        }

        private void BranchBuildCustomizationXmlFile(QAMainFolder qaMainFolder, IInstallerFolder installerFolder)
        {
            if(qaMainFolder.Components.BuildCustomizationXml.Exists())
            {
                qaMainFolder.Components.BuildCustomizationXml.ToSourceControlFile().Branch(installerFolder.Components.GetServerPath());
            }
        }

        private class BuildAction
        {
            public BuildAction(string description, Action action)
            {
                this.Description = description;
                this._action = action;
            }

            public string Description { get; private set; }
            private Action _action;

            public void Execute()
            {
                _action();
            }
        }


        private byte[] CreatePublishPayload(GetInstallerContentResponse installerContent)
        {
            var publishPayloadBuilder = _publishPayloadBuilderFactory();

            foreach(var coreComponent in installerContent.CoreComponents)
            {
                publishPayloadBuilder.AddCoreComponent(coreComponent.Name, new VersionNumber(coreComponent.Version));
            }

            foreach (var gameEngine in installerContent.GameEngines)
            {
                publishPayloadBuilder.AddGameEngine(new GameEngineName(gameEngine.Name), 
                                                    new VersionNumber(gameEngine.Version));
            }

            foreach (var gameMath in installerContent.GamesMaths)
            {
                publishPayloadBuilder.AddGameMath(gameMath.Name, 
                                                  new GameEngineName(gameMath.EngineName), 
                                                  new VersionNumber(gameMath.Version));
            }

            foreach (var gameLimits in installerContent.GamesLimits)
            {
                publishPayloadBuilder.AddGameLimits(gameLimits.Name, 
                                                    new GameEngineName(gameLimits.EngineName), 
                                                    new VersionNumber(gameLimits.Version));
            }

            return _publishPayloadSerializer.Serialize(publishPayloadBuilder.Build());
        }

        private void CreatePublishXmlFile(IInstallerFolder installerFolder, GetInstallerContentResponse installerContent)
        {
            installerFolder.Trigger.PublishXml.SetBinaryContent(CreatePublishPayload(installerContent));
        }

        private void CreateLatestPublishXmlFile(IInstallerFolder installerFolder, GetInstallerContentResponse installerContent)
        {
            installerFolder.PublishHistory.LatestPublishXml.SetBinaryContent(CreatePublishPayload(installerContent));
        }

        private void BranchSolutionFile(Folders.QAMainFolder qaMainFolder, Folders.IInstallerFolder installerFolder)
        {
            _tfsGateway.Branch(qaMainFolder.Components.GGPGameServerSln.GetServerPath(),
                               installerFolder.Components.GGPGameServerSln.GetServerPath(),
                               false);
        }


        private void BranchBuildTools(Folders.BuildToolsFolder sourceFolder, Folders.BuildToolsFolder targetFolder)
        {
            _tfsGateway.Branch(sourceFolder.GetServerPath(),
                               targetFolder.GetServerPath(),
                               false);
        }

        

        private IEnumerable<BuildAction> GetBranchEnginesAndGamesActions(Folders.ComponentsFolder installerComponentsFolder, DeveloperToolService.GetInstallerContentResponse installerContent)
        {
            var actions = new List<BuildAction>();

            foreach (var gameEngine in installerContent.GameEngines)
            {
                actions.Add(new BuildAction($"Branching game engine {gameEngine.Name}",
                   () =>
                   {
                       var targetPath = installerComponentsFolder.EnginesAndGames.GameEngine(gameEngine.Name).Engine.GetServerPath();
                       BranchComponent(gameEngine.ProjectFolder, targetPath, gameEngine.ChangeSet);
                   }));
            }

            foreach (var gameMath in installerContent.GamesMaths)
            {
                actions.Add(new BuildAction($"Branching math for game {gameMath.Name} in engine {gameMath.EngineName}",
                  () =>
                  {
                      var targetPath = installerComponentsFolder.EnginesAndGames.GameEngine(gameMath.EngineName)
                                                                         .Games
                                                                         .Game(gameMath.Name)
                                                                         .Math
                                                                         .GetServerPath();
                      BranchComponent(gameMath.ProjectFolder, targetPath, gameMath.ChangeSet);
                  }));
            }

            foreach (var gameLimits in installerContent.GamesLimits)
            {
                actions.Add(new BuildAction($"Branching limits for game {gameLimits.Name} in engine {gameLimits.EngineName}",
                           () =>
                           {
                               var targetPath = installerComponentsFolder.EnginesAndGames.GameEngine(gameLimits.EngineName)
                                                                                         .Games
                                                                                         .Game(gameLimits.Name)
                                                                                         .Limits
                                                                                         .GetServerPath();
                               BranchComponent(gameLimits.ProjectFolder, targetPath, gameLimits.ChangeSet);
                           }));
            }

            return actions;
        }

        private IEnumerable<BuildAction> GetBranchCoreComponentsActions(Folders.ComponentsFolder installerComponentsFolder, DeveloperToolService.GetInstallerContentResponse installerContent)
        {

            var actions = new List<BuildAction>();

            foreach (var coreComponent in installerContent.CoreComponents)
            {
                if (coreComponent.Name == Folders.PackagesFolder.WellKnownName)
                {
                    actions.Add(new BuildAction($"Branch {coreComponent.Name}",
                       () =>
                       {
                           var targetPath = installerComponentsFolder.Packages.GetServerPath();
                           BranchComponent(coreComponent.ProjectFolder, targetPath, coreComponent.ChangeSet);
                       }));
                }
                else
                {
                    actions.Add(new BuildAction($"Branch core component {coreComponent.Name}",
                        () =>
                        {
                            var targetPath = installerComponentsFolder.Core.CoreComponent(coreComponent.Name).GetServerPath();
                            BranchComponent(coreComponent.ProjectFolder, targetPath, coreComponent.ChangeSet);
                        }));
                }

            }
            return actions;
        }

        private void BranchComponent(string sourcePath, IServerPath targetPath, int changeSet)
        {
            _tfsGateway.Branch(_tfsGateway.CreateServerPath(sourcePath), 
                                targetPath, 
                                true, 
                                changeSet);
        }


    }
}
