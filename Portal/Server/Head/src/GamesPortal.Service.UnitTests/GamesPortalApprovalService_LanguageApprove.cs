using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Helpers;
using NUnit.Framework;
using static GamesPortal.Service.Helpers.MockRecordsFactory;
using NSubstitute;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Artifactory;


namespace GamesPortal.Service
{
    [TestFixture]
    public class GamesPortalApprovalService_LanguageApprove
    {
        [SetUp]
        public void Setup()
        {
            InitInternalServices();
            InitApprover();
            _approvalService = new GamesPortalApprovalService(_internalServices);
        }

        private void InitInternalServices()
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            _internalServices.CallContextServices.GetCallingUserName().Returns(_userName);
            _internalServices.TimeServices.Now.Returns(_currentDate);
            _dal = _internalServices.CreateGamesPortalDBDataContext();
        }

        private void InitApprover()
        {
            _approver = _internalServices.CreateGameLanguageApprover(_dal);
            MockGameLanguageApproverResponse(new GameVersion[0]);
        }

        IGamesPortalDataContext _dal;
        IGamesPortalInternalServices _internalServices;
        GamesPortalApprovalService _approvalService;
        IGameLanguageApprover _approver;
        string _userName = "florin";
        DateTime _currentDate = new DateTime(2016, 5, 6);
        Guid _gameVersionId = Guid.NewGuid();

        private LanguageApproveRequest CreateRequest(params string[] languages)
        {

            if (languages.Length == 0)
                languages = new string[] { "English" };

            return new LanguageApproveRequest(_gameVersionId, languages);
        }

        private void MockGameLanguageApproverResponse(params GameVersion[] gameVersions)
        {
            _approver.Approve(Arg.Any<GameLanguageApprover_Request>())
                     .Returns(new GameLanguageApprover_Response(
                                    gameVersions.GroupBy(gv => gv.Game_ID)
                                                .Select(group => group.First().Game)
                                                .ToArray(),
                                    gameVersions));
        }

        [Test]
        public void LanguageApprove_ShouldCall_DisposeOnDAL()
        {
            _approvalService.LanguageApprove(CreateRequest());

            _dal.Received().Dispose();
        }

        [Test]
        public void LanguageApprove_ShouldCallGameLanguageApprover_Approve()
        {
            GameLanguageApprover_Request request = null;
            _approver.Approve(Arg.Do<GameLanguageApprover_Request>(arg => request = arg));

            _approvalService.LanguageApprove(CreateRequest("English"));

            Assert.AreEqual(_gameVersionId, request.GameVersionId);
            Assert.AreEqual(_currentDate, request.ApprovalDate);
            Assert.AreEqual(_userName, request.ApprovalUser);
            Assert.AreEqual(1, request.Languages.Length);
            Assert.AreEqual("English", request.Languages[0].Name);
        }

      

     

        [Test]
        public void LanguageApprove_ShouldCall_SubmitChanges()
        {   
            _approvalService.LanguageApprove(CreateRequest());

            _dal.Received().SubmitChanges();
        }

        [Test]
        public void LanguageApprove_IfThereAreAffectedGameVersionsCall_GameLanguageToArtifactorySynchronizer_Run_Method_After_SubmitChanges()
        {
            var gameRecord = GameRecord(130017);
            var gameVersionRecord1 = gameRecord
                                    .AddVersionRecord("1.0.5.7");
            var gameVersionRecord2 = gameRecord
                                    .AddVersionRecord("1.2.1.9");


            MockGameLanguageApproverResponse(gameVersionRecord1, gameVersionRecord2);

            _approvalService.LanguageApprove(CreateRequest());

            Received.InOrder(() =>
            {
                _dal.SubmitChanges();
                _internalServices.GameLanguageToArtifactorySynchronizer.Run();
            });
        }

        [Test]
        public void LanguageApprove_IfThereAreNoAffectedGameVersions_DONT_Call_GameLanguageToArtifactorySynchronizer_Run_Method()
        {
       
            MockGameLanguageApproverResponse(new GameVersion[0]);

            _approvalService.LanguageApprove(CreateRequest());

            _internalServices.GameLanguageToArtifactorySynchronizer.DidNotReceive().Run();
        }

        [Test]
        public void LanguageApprove_ForAffectedGameVersion_ShouldNotifySignalRClients_WithTheCorrectData()
        {
            var gameRecord = GameRecord(130017, true);
            var gameVersionRecord = gameRecord
                                    .AddVersionRecord("1.0.5.7");
            


            MockGameLanguageApproverResponse(gameVersionRecord);

            SignalR.GameSynchronizationFinishedData notificationData = null;
            _internalServices.GamesPortalHubContext.GameSynchronizationFinished(Arg.Do<SignalR.GameSynchronizationFinishedData>(arg => notificationData = arg));

            _approvalService.LanguageApprove(CreateRequest());

            Assert.AreEqual(gameRecord.Game_ID, notificationData.GameId);
            Assert.AreEqual(RecordChangeType.Changed, notificationData.ChangeType);
            Assert.AreEqual(gameRecord.IsExternal, notificationData.IsExternal);
        }

        [Test]
        public void LanguageApprove_IfThereIsNoAffectedGame_Should_NOT_NotifySignalRClients()
        {
            

            MockGameLanguageApproverResponse(new GameVersion[0]);
            
            _approvalService.LanguageApprove(CreateRequest());

            _internalServices.GamesPortalHubContext.DidNotReceiveWithAnyArgs().GameSynchronizationFinished(null);
        }
    }
}
