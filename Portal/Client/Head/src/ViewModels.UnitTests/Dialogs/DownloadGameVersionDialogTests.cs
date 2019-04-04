using System;
using System.Linq;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using Spark.Infra.Types;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Infra.Exceptions;

namespace GamesPortal.Client.ViewModels.Dialogs
{
    [TestFixture]
    public class DownloadGameVersionDialogTests
    {
        [Test]
        public void Constructor_ShouldLoadTheSupportedRegulationsList()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));

            var dialog = CreateDialog(gameVersion);

            Assert.AreEqual(2, dialog.SupportedRegulations.Count);
        }


        [Test]
        public void Constructor_ByDefaultAllRegulationsAreSelected()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));

            var dialog = CreateDialog(gameVersion);

            Assert.IsTrue(dialog.SupportedRegulations.All(r => r.Selected));
        }

        [Test]
        public void Constructor_SupportedRegulationsShouldBeSortedByName()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"),
                                                CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"),
                                                CreateGameVersionRegulation(RegulationType.GetRegulation("Denmark"), "NDL"));

            var dialog = CreateDialog(gameVersion);
            Assert.AreEqual("888Italy", dialog.SupportedRegulations[0].Regulation.Name);
            Assert.AreEqual("Denmark", dialog.SupportedRegulations[1].Regulation.Name);
            Assert.AreEqual("Gibraltar", dialog.SupportedRegulations[2].Regulation.Name);
        }


        [Test]
        public void Constructor_SupportedRegulationsShouldNotBeDuplicated()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL", "DL", "Bingo"));

            var dialog = CreateDialog(gameVersion);
            Assert.AreEqual(1, dialog.SupportedRegulations.Count);
        }

        [Test]
        public void SelectFolderCommand_ShouldCallDialogServices_SelectFolderMethod()
        {
            
            var dialogServices = MockRepository.GenerateMock<IDialogServices>();
            dialogServices.Expect(dlgs => dlgs.SelectFolder()).Return("");
            var dialog = CreateDialog(dialogServices);
                        
            dialog.SelectDestinationFolderCommand.Execute(null);
            
            dialogServices.VerifyAllExpectations();
        }


        [Test]
        public void SelectFolderCommand_IfDialogServiceSelectFolderReturnsAFolder_SetsTheDestinationProperty()
        {
            
            var dialogServices = MockRepository.GenerateStub<IDialogServices>();
            dialogServices.Stub(dlgs => dlgs.SelectFolder()).Return(@"C:\Downloads");
            var dialog = CreateDialog(dialogServices);

            dialog.SelectDestinationFolderCommand.Execute(null);

            Assert.AreEqual(@"C:\Downloads", dialog.DestinationFolder);
        }

        [TestCase("")]
        [TestCase(null)]
        public void SelectFolderCommand_IfDialogServiceSelectFolderReturnsNullOrEmpty_DontChangeTheDestinationFolder(string selectedFolder)
        {

            var dialogServices = MockRepository.GenerateStub<IDialogServices>();
            dialogServices.Stub(dlgs => dlgs.SelectFolder()).Return(selectedFolder);
            var dialog = CreateDialog(dialogServices);

            dialog.DestinationFolder = @"C:\Downloads";

            dialog.SelectDestinationFolderCommand.Execute(null);

            Assert.AreEqual(@"C:\Downloads", dialog.DestinationFolder);
        }

        [Test]
        public void UnselectAllRegulationsCommnad_SetsSelectedToFalseForAll()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL" ), 
                                                CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));

            var dialog = CreateDialog(gameVersion);
            dialog.UnselectAllRegulationsCommand.Execute(null);

            Assert.IsTrue(dialog.SupportedRegulations.All(r => !r.Selected));
        }

        [Test]
        public void SelectAllRegulationsCommnad_SetsSelectedToTrueForAll()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));

            var dialog = CreateDialog(gameVersion);
            dialog.UnselectAllRegulationsCommand.Execute(null);
            dialog.SelectAllRegulationsCommand.Execute(null);
            
            Assert.IsTrue(dialog.SupportedRegulations.All(r => r.Selected));
        }



        [Test]
        public void ExecuteOk_IfNoRegulationSelected_ThrowsValidationException()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL" ));

            var dialog = CreateDialog(gameVersion);
            dialog.UnselectAllRegulationsCommand.Execute(null);

            Assert.Throws<ValidationException>(() => dialog.ExecuteOk());

        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void ExecuteOk_IfDestinationFolderIsNullOrEmpty_ThrowsValidationException(string destinationFolder)
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));

            var dialog = CreateDialog(gameVersion);
            dialog.DestinationFolder = destinationFolder;

            Assert.Throws<ValidationException>(() => dialog.ExecuteOk());

        }

        [TestCase(@"C:\temp           ")]
        [TestCase(@"           C:\temp           ")]
        [TestCase(@"           C:\temp")]
        public void DestinationFolder_ShouldTrimTheValue(string destinationFolder)
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));

            var dialog = CreateDialog(gameVersion);
            dialog.DestinationFolder = destinationFolder;

            Assert.AreEqual(@"C:\temp", dialog.DestinationFolder);
        }


        [Test]
        public void ExecuteOk_ShouldCallBackgroundActionArea_AddAction()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), 
                                                CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));
            var backgroundActionsArea = MockRepository.GenerateMock<NotificationArea.IBackgroundActionsArea>();
            backgroundActionsArea.Expect(a => a.AddAction(null)).IgnoreArguments();
            var dialog = CreateDialog(backgroundActionsArea);
            dialog.ExecuteOk();
            backgroundActionsArea.VerifyAllExpectations();
        }


        [Test]
        public void ExecuteOk_IfExtractFileContentIsFalse_AddADownloadFileAction()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), 
                                                CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));
            var backgroundActionsArea = MockRepository.GenerateStub<NotificationArea.IBackgroundActionsArea>();

            object actualAction = null;
            backgroundActionsArea.Stub(a => a.AddAction(null)).IgnoreArguments().WhenCalled(invocation =>actualAction =  invocation.Arguments[0]);

            var dialog = CreateDialog(backgroundActionsArea);
            dialog.ExtractFileContent = false;
            dialog.ExecuteOk();

            Assert.AreEqual(typeof(NotificationArea.DownloadFileAction), actualAction.GetType());
        }

        [Test]
        public void ExecuteOk_IfExtractFileContentIsTrue_AddADownloadAndUnzipFileAction()
        {
            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), 
                                                CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));
            var backgroundActionsArea = MockRepository.GenerateStub<NotificationArea.IBackgroundActionsArea>();

            object actualAction = null;
            backgroundActionsArea.Stub(a => a.AddAction(null)).IgnoreArguments().WhenCalled(invocation => actualAction = invocation.Arguments[0]);

            var dialog = CreateDialog(backgroundActionsArea);
            dialog.ExtractFileContent = true;
            dialog.ExecuteOk();

            Assert.AreEqual(typeof(NotificationArea.DownloadAndUnzipFileAction), actualAction.GetType());
        }
        

        [Test]
        public void ExecuteOk_ShouldMakeOneCallToTheAddActionAccrodingWithTheSelectedRegulations()
        {

            var backgroundActionsArea = MockRepository.GenerateStrictMock<NotificationArea.IBackgroundActionsArea>();
            backgroundActionsArea.Expect(b => b.AddAction(null)).IgnoreArguments().Repeat.Times(2);

            var dialog = CreateDialog(CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL"), 
                                                        CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"), 
                                                        CreateGameVersionRegulation(RegulationType.GetRegulation("Denmark"), "NDL")), 
                                      CreateDefaultDialogServicesStub(),
                                      backgroundActionsArea);

            dialog.SupportedRegulations.First(r => r.Regulation.Name == "Gibraltar").Selected = false;

            dialog.ExecuteOk();
                        
            backgroundActionsArea.VerifyAllExpectations();

        }


       


        [Test]
        public void ExecuteOk_ShouldAppendToRegulationName_GameName_And_FileNameToTheDestinationFolder()
        {

            var backgroundActionsArea = MockRepository.GenerateStub<NotificationArea.IBackgroundActionsArea>();
            NotificationArea.DownloadFileAction actualAction = null;
            backgroundActionsArea.Stub(b => b.AddAction(null)).IgnoreArguments().WhenCalled(invocation => actualAction = (NotificationArea.DownloadFileAction)invocation.Arguments[0]);

            
            
            
            var game = CreateGame();
            game.Name = "Elm Street";
            game.MainGameType = 130017;

            var gameVersion = CreateGameVersion(CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL"));
            gameVersion.Regulations[0].DownloadInfo.FileName = "file.zip";

            var dialog = CreateDialog(gameVersion,
                                      CreateDefaultDialogServicesStub(),
                                      backgroundActionsArea,
                                      game);

            dialog.ExtractFileContent = false;
            dialog.DestinationFolder = @"C:\Downloads";

            dialog.ExecuteOk();

            Assert.AreEqual(@"C:\Downloads\888Italy\Elm Street_file.zip", actualAction.DestinationPath);
        }
        

        [Test]
        public void ExecuteOk_ShouldPersistUsedDestinationFolder()
        {
            
            var dialog1 = CreateDialogWithoutDestinationFolder(CreateGameVersion(), 
                                                                CreateDefaultDialogServicesStub(),
                                                                CreateDefaultBackgroundActionsAreaStub());

            dialog1.DestinationFolder = @"C:\temp\downloads\thegames";

            dialog1.ExecuteOk();

            var dialog2 = CreateDialogWithoutDestinationFolder(CreateGameVersion(),
                                                               CreateDefaultDialogServicesStub(),
                                                               CreateDefaultBackgroundActionsAreaStub());

            Assert.AreEqual(@"C:\temp\downloads\thegames", dialog2.DestinationFolder);

        }

        private DownloadGameVersionDialog CreateDialog(IDialogServices dialogServices)
        {
            return CreateDialog(CreateGameVersion(), dialogServices, CreateDefaultBackgroundActionsAreaStub());
        }


        private DownloadGameVersionDialog CreateDialog(NotificationArea.IBackgroundActionsArea backgroundActionsArea)
        {
            return CreateDialog(CreateGameVersion(), CreateDefaultDialogServicesStub(), backgroundActionsArea);
        }

        private DownloadGameVersionDialog CreateDialog(GameVersion gameVersion)
        {
            return CreateDialog(gameVersion, CreateDefaultDialogServicesStub(), CreateDefaultBackgroundActionsAreaStub());
        }

        private static IDialogServices CreateDefaultDialogServicesStub()
        {
            var dialogServices = MockRepository.GenerateStub<IDialogServices>();
            dialogServices.Stub(d => d.SelectFolder(true)).IgnoreArguments().Return(@"C:\temp");
            return dialogServices;
        }

        private static NotificationArea.IBackgroundActionsArea CreateDefaultBackgroundActionsAreaStub()
        {
            return MockRepository.GenerateStub<NotificationArea.IBackgroundActionsArea>();
        }
       

        private DownloadGameVersionDialog CreateDialog(GameVersion gameVersion, IDialogServices dialogServices, NotificationArea.IBackgroundActionsArea backgroundActionsArea, Game game = null)
        {

            var dialog = CreateDialogWithoutDestinationFolder(gameVersion, dialogServices, backgroundActionsArea, game);

            dialog.DestinationFolder = @"C:\default";
            return dialog;

        }

        private DownloadGameVersionDialog CreateDialogWithoutDestinationFolder(GameVersion gameVersion, IDialogServices dialogServices, NotificationArea.IBackgroundActionsArea backgroundActionsArea, Game game = null)
        {

            var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            serviceLocator.Stub(l => l.GetInstance<IDialogServices>()).Return(dialogServices);
            serviceLocator.Stub(l => l.GetInstance<NotificationArea.IBackgroundActionsArea>()).Return(backgroundActionsArea);
            

            if (game == null)
                game = CreateGame();

            var dialog = new DownloadGameVersionDialog(game,
                                                        gameVersion,
                                                        serviceLocator);

           
            return dialog;

        }

        private static Game CreateGame()
        {
            return Helpers.GameHelper.CreateGame(DEFAULT_GAME_ID, 130017, "Elm Street");
        }

        private static readonly Guid DEFAULT_GAMEVERSION_ID = new Guid("47C324DF-4632-4944-8FB8-901B49465701");
        private static readonly Guid DEFAULT_GAME_ID = new Guid("E01C9FA6-DB2F-47E7-93DD-39A5D34B43D1");

        private GameVersion CreateGameVersion(params GameVersionRegulation[] regulations)
        {
            var gameVersion = new GameVersion();
            gameVersion.Category = GamingComponentCategory.Game;
            gameVersion.CreatedBy = "florin";
            gameVersion.CreatedDate = new DateTime(2016, 1, 1);
            gameVersion.Id = DEFAULT_GAMEVERSION_ID;
            gameVersion.Infrastructure = new GameInfrastructure(GameTechnology.Html5, PlatformType.PC);
            gameVersion.TriggeredBy = "eli";
            gameVersion.Version = new VersionNumber("1.0.2.5");
            

            if (regulations == null || regulations.Length == 0)
                regulations = new GameVersionRegulation[]
                    {
                        CreateGameVersionRegulation(RegulationType.GetRegulation("Gibraltar"), "NDL", "DL" ),
                        CreateGameVersionRegulation(RegulationType.GetRegulation("888Italy"), "NDL")
                    };

            gameVersion.Regulations = regulations;

            return gameVersion;
        }

        private GameVersionRegulation CreateGameVersionRegulation(RegulationType regulation, params string[] clientTypes)
        {
            var approvalInfo = MockRepository.GenerateStub<IGameVersionApprovalInfo>();
            return new GameVersionRegulation(regulation,
                                            new DownloadInfo(string.Format("http://localhost/{0}/file.zip", regulation.Name), "file.zip", 10000, "md5"),
                                            approvalInfo,
                                            new GameVersionRegulationLanguage[0]);
        }
    }
}
