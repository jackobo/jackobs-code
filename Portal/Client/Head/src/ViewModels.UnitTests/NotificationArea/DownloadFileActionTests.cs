using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Services;
using GamesPortal.Client.ViewModels.Helpers;
using Spark.Infra.Types;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using Spark.Infra.Windows;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    [TestFixture]
    public class DownloadFileActionTests 
    {
        
        IUnityContainer Container { get; set; }

        [SetUp]
        public void Setup()
        {
            Container = UnityContainerHelper.Create();
        }

        [Test]
        public void Status_DefaultToWaiting()
        {
            var action = CreateDownloadFileAction();
            Assert.AreEqual(BackgroundActionStatus.Waiting, action.Status);
        }
                
        [Test]
        public void Start_ShouldSetTheStatus_ToInProgress()
        {
            var action = CreateDownloadFileAction();
            action.Start();
            Assert.AreEqual(BackgroundActionStatus.InProgress, action.Status);
        }


        [Test]
        public void Start_ShouldCallDownloadMethodInDownloadManager()
        {
            var downloadManager = MockRepository.GenerateMock<IDownloadManager>();
            downloadManager.Expect(dm => dm.DownloadFile(null, null)).IgnoreArguments();

            var action = CreateDownloadFileAction(downloadManager);
            action.Start();

            downloadManager.VerifyAllExpectations();
        }


        [Test]
        public void Start_ProvideTheCorrectUrlToTheDownloadManager()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();
            Uri actualUri = null;
            downloadManager.Stub(dm => dm.DownloadFile(null, null))
                           .IgnoreArguments()
                           .WhenCalled(invocation => actualUri = (Uri)invocation.Arguments[0]);

            var action = CreateDownloadFileAction(downloadManager);
            var expectedUri = new Uri("http://artifactory/games/130017/file.zip");
            action.Uri = expectedUri;
            action.Start();

            Assert.AreEqual(expectedUri, actualUri);
        }


        [Test]
        public void Start_ProvideTheCorrectDownloadPath()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();
            string actualPath = null;
            downloadManager.Stub(dm => dm.DownloadFile(null, null))
                           .IgnoreArguments()
                           .WhenCalled(invocation => actualPath = (string)invocation.Arguments[1]);

            var action = CreateDownloadFileAction(downloadManager);

            var expectedDownloadLocation = @"C:\temp\downloads\elmstreet-1.0.0.5.zip";
            action.DestinationPath = expectedDownloadLocation;
            
            action.Start();

            Assert.AreEqual(expectedDownloadLocation, actualPath);
        }


        [Test]
        public void Start_ShouldCreateTheFolderForTheDownloadLocation()
        {

            var action = CreateDownloadFileAction();
            action.DestinationPath = @"C:\temp\downloads\elmstreet-1.0.0.5.zip";

            var fileSystemManager = Container.Resolve<IFileSystemManager>();
            string actualFolder = null;
            fileSystemManager.Stub(fsm => fsm.CreateFolder(null)).IgnoreArguments()
                             .WhenCalled(invocation => actualFolder = (string)invocation.Arguments[0]);


            action.Start();

            Assert.AreEqual(@"C:\temp\downloads", actualFolder);
        }



        [Test]
        public void Start_ShouldDeleteTheFileIfAlreadyExists()
        {

            var action = CreateDownloadFileAction();
            action.DestinationPath = @"C:\temp\downloads\elmstreet-1.0.0.5.zip";


            var fileSystemManager = MockRepository.GenerateMock<IFileSystemManager>();
            fileSystemManager.Expect(fsm => fsm.FileExists(null)).IgnoreArguments().Return(true);
            fileSystemManager.Expect(fsm => fsm.DeleteFile(null)).IgnoreArguments();

            Container.RegisterInstance<IFileSystemManager>(fileSystemManager);

         

            action.Start();

            fileSystemManager.VerifyAllExpectations();
        }

        [Test]
        public void Caption_AfterInitialization_ShowWaitingForMessage()
        {

            var action = CreateDownloadFileAction();
            action.Uri = new Uri("http://localhost/file.zip");

            Assert.AreEqual("Waiting for downloading http://localhost/file.zip", action.Caption);
        }

        [Test]
        public void Caption_AfterStartDownloading_ShouldShowDownloadingMessage()
        {

            var action = CreateDownloadFileAction();
            action.Uri = new Uri("http://localhost/file.zip");

            action.Start();

            Assert.AreEqual("Downloading http://localhost/file.zip", action.Caption);
        }

        [TestCase("Caption")]
        [TestCase("Status")]
        public void Start_ShouldRaise_PropertyChange(string expectedPropertyName)
        {

            var action = CreateDownloadFileAction();
            var raisedProperties = new List<string>();
            action.PropertyChanged += (sndr, args) => raisedProperties.Add(args.PropertyName);
            action.Start();
            Assert.AreEqual(1, raisedProperties.Count(p => p == expectedPropertyName));
        }

        [Test]
        public void ProgressPercentage_ShouldBeChangedWhenDownloadManagerRaisesDownloadProgressChanged()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();

            var action = CreateDownloadFileAction(downloadManager);
            
            action.Start();

            downloadManager.Raise(a => a.DownloadProgressChanged += null, downloadManager, new ActionProgressChangedEventArgs(33));

            Assert.AreEqual(33, action.ProgressPercentage);
        }

        [Test]
        public void ProgressPercentage_ShouldBeSetTo100WhenDownloadCompletedEventIsRaised()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();

            var action = CreateDownloadFileAction(downloadManager);

            action.Start();

            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(null, false, null));

            Assert.AreEqual(100, action.ProgressPercentage);
        }

        [Test]
        public void State_WhenDownloadIsFinished_ShouldBeSetToDone()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();

            var action = CreateDownloadFileAction(downloadManager);

            action.Start();

            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(null, false, null));

            Assert.AreEqual(BackgroundActionStatus.Done, action.Status);
        }


        [Test]
        public void State_IfDownloadFailed_ShouldBeSetToError()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();

            var action = CreateDownloadFileAction(downloadManager);

            action.Start();

            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(new ApplicationException(), false, null));

            Assert.AreEqual(BackgroundActionStatus.Error, action.Status);
        }

        [Test]
        public void ErrorMessage_IfDownloadFailed_ShouldBeSetToTheExceptionMessage()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();

            var action = CreateDownloadFileAction(downloadManager);

            action.Start();

            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(new ApplicationException("Test"), false, null));

            Assert.AreEqual("Test", action.ErrorMessage);
        }


        [Test]
        public void ErrorDetails_IfDownloadFailed_ShouldBeSetToTheExceptionToString()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();

            var action = CreateDownloadFileAction(downloadManager);

            action.Start();

            var exception = new ApplicationException("Test");
            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(exception, false, null));

            Assert.AreEqual(exception.ToString(), action.ErrorDetails);
        }

        [Test]
        public void ErrorDetails_IfDownloadCanceled_ShouldSetTheCancelStatusEvenIfAnExceptionWasThrown()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();

            var action = CreateDownloadFileAction(downloadManager);

            action.Start();

            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(new ApplicationException("Test"), true, null));

            Assert.AreEqual(BackgroundActionStatus.Canceled, action.Status);
        }

        [Test]
        public void Start_IfCalledAfterAFinishedDownload_ShouldResetThePercentageCompleted()
        {
            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();

            var action = CreateDownloadFileAction(downloadManager);

            action.Start();

            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(null, false, null));

            action.Start();

            Assert.AreEqual(0, action.ProgressPercentage);
        }


        [Test]
        public void Start_IfInProgress_ThrowInvalidOperationException()
        {
            var action = CreateDownloadFileAction();
            action.Start();
            Assert.Throws<InvalidOperationException>(() => action.Start());
        }


        [Test]
        public void Cancel_ShouldCallCancelInDownloadManager()
        {
            var downloadManager = MockRepository.GenerateMock<IDownloadManager>();
            downloadManager.Expect(dm => dm.Cancel());

            var action = CreateDownloadFileAction(downloadManager);

            action.Start();
            action.Cancel();


            downloadManager.VerifyAllExpectations();
        }


        [Test]
        public void Cancel_ShouldCallCancelInDownloadManagerOnlyIfIsInProgress()
        {
            var downloadManager = MockRepository.GenerateMock<IDownloadManager>();
            downloadManager.Expect(dm => dm.Cancel()).Repeat.Never();

            var action = CreateDownloadFileAction(downloadManager);
            action.Start();
            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(null, false, null));
            action.Cancel();


            downloadManager.VerifyAllExpectations();
        }


        [Test]
        public void Cancel_ShouldSetTheStateToCanceled()
        {
            
            var action = CreateDownloadFileAction();
            action.Start();
            action.Cancel();

            Assert.AreEqual(BackgroundActionStatus.Canceled, action.Status);
            
        }


        [Test]
        public void Cancel_ShouldSetTheStateToCanceledOnlyIfWasInProgress()
        {

            var downloadManager = MockRepository.GenerateStub<IDownloadManager>();
            
            var action = CreateDownloadFileAction(downloadManager);
            action.Start();
            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(null, false, null));
            action.Cancel();

            Assert.AreNotEqual(BackgroundActionStatus.Canceled, action.Status);

            
        }

        [Test]
        public void Cancel_WithoutStart_ShouldBeIgnored()
        {

            var action = CreateDownloadFileAction();

            action.Cancel();
        }


        [Test]
        public void Dispose_ShouldCallDownloadManager_Dispose()
        {

            var downloadManager = MockRepository.GenerateMock<IDownloadManager>();
            downloadManager.Expect(dm => dm.Dispose());

            var action = CreateDownloadFileAction(downloadManager);
            action.Start();

            action.Dispose();


            downloadManager.VerifyAllExpectations();


        }


        [Test]
        public void WhenDownloadFinished_ShouldDisposeTheDownloadManager()
        {

            var downloadManager = MockRepository.GenerateMock<IDownloadManager>();
            downloadManager.Expect(dm => dm.Dispose());

            var action = CreateDownloadFileAction(downloadManager);
            action.Start();

            downloadManager.Raise(a => a.DownloadCompleted += null, downloadManager, new System.ComponentModel.AsyncCompletedEventArgs(null, false, null));


            downloadManager.VerifyAllExpectations();


        }


        private DownloadFileAction CreateDownloadFileAction()
        {
            return CreateDownloadFileAction(MockRepository.GenerateStub<IDownloadManager>());
        }

        private DownloadFileAction CreateDownloadFileAction(IDownloadManager downloadManager)
        {
            Container.RegisterInstance<IDownloadManager>(downloadManager);
            Container.RegisterInstance<IFileSystemManager>(MockRepository.GenerateStub<IFileSystemManager>());
            
            var action = new DownloadFileAction(Container.Resolve<IServiceLocator>(), new Uri("http://localhost/test"), @"C:\temp\file.zip");
            
            return action;
        }

    }
}
