using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    [TestFixture]
    public class DownloadAndUnzipFileActionTests
    {
        [Test]
        public void Caption_AfterInitializationShouldReturnTheCaptionFromDownloadAction()
        {
            var downloadAction = CreateDownloadActionStub("http://localhost/test");
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());

            Assert.AreEqual("http://localhost/test", action.Caption);
        }

        [Test]
        public void Status_AfterInitializationShouldReturnTheStatus_Waiting()
        {
            var downloadAction = CreateDownloadActionStub("http://localhost/test");
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());

            Assert.AreEqual(BackgroundActionStatus.Waiting, action.Status);
        }

        [Test]
        public void Start_ShouldCall_StartInDownloadAction()
        {
            var downloadAction = MockRepository.GenerateMock<IBackgroundAction>();
            downloadAction.Expect(a => a.Start());
            
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());
            action.Start();

            downloadAction.VerifyAllExpectations();
        }


        [Test]
        public void Start_IfAlreadyInProgress_ThrowInvalidOperationException()
        {
            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());
            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.InProgress);

            Assert.Throws<InvalidOperationException>(() => action.Start());
            
        }


        [Test]
        public void DownloadAction_WhenRaiseStatusPropertyChangedWithValueDone_StartUnzipAction()
        {
            
            var unzipAction = MockRepository.GenerateMock<IBackgroundAction>();
            unzipAction.Expect(a => a.Start());

            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            var action = new DownloadAndUnzipFileAction(downloadAction, unzipAction);
            
            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.Done);

            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs("Status"));

            unzipAction.VerifyAllExpectations();
            

        }


        [Test]
        public void DownloadAction_AfterStartingUnzipAction_StatusShouldStillBeInProgress()
        {
            var unzipAction = MockRepository.GenerateStub<IBackgroundAction>();
            unzipAction.Stub(a => a.Start()).WhenCalled(arg => unzipAction.Stub(a => a.Status).Return(BackgroundActionStatus.InProgress));
            
            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            var action = new DownloadAndUnzipFileAction(downloadAction, unzipAction);
            

            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.Done);

            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs("Status"));

            Assert.AreEqual(BackgroundActionStatus.InProgress, action.Status);
        }


        [TestCase("Status")]
        [TestCase("Caption")]
        [TestCase("ProgressPercentage")]
        public void IfDownloadActionIsCurrentAction_AnyPropertyChangedInDownloadActionShouldRaisePropertyChangeInParentAction(string expectedPropertyName)
        {
           
            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            downloadAction.Stub(a => a.Start()).WhenCalled(arg => downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.InProgress));
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());
            
            string actualPropertyName = null;
            action.PropertyChanged += (sender, args) => actualPropertyName = args.PropertyName;

            action.Start();

            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs(expectedPropertyName));

            Assert.AreEqual(expectedPropertyName, actualPropertyName);
        }


        [Test]
        public void IfDownloadActionIsCurrentAction_ShouldNotRaisePropertyChangedWhenPropertyIsStatusAndValueIsDone()
        {

            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());

            var changedProperties = new List<string>();
            action.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);

            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            

            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs("Status"));

            Assert.IsFalse(changedProperties.Contains("Status"));
        }



        [TestCase("Status")]
        [TestCase("Caption")]
        [TestCase("ProgressPercentage")]
        public void IfDownloadActionIsNOTCurrentAction_ShouldNotRaiseAnyPropertyChangeAnymore(string propertyName)
        {

            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());

            

            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs("Status"));

            bool propertyChangedWasRaised = false;
            action.PropertyChanged += (sender, args) => propertyChangedWasRaised = true;

            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs(propertyName));

            Assert.IsFalse(propertyChangedWasRaised);
        }


        [TestCase("Status")]
        [TestCase("Caption")]
        [TestCase("ProgressPercentage")]
        public void IfUnzipActionIsCurrentAction_AnyPropertyChangedInDownloadActionShouldRaisePropertyChangeInParentAction(string expectedPropertyName)
        {

            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            var unzipAction = MockRepository.GenerateStub<IBackgroundAction>();
            var action = new DownloadAndUnzipFileAction(downloadAction, unzipAction);

            string actualPropertyName = null;
            action.PropertyChanged += (sender, args) => actualPropertyName = args.PropertyName;

            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs("Status"));


            unzipAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs(expectedPropertyName));

            Assert.AreEqual(expectedPropertyName, actualPropertyName);
        }


        [TestCase("Status")]
        [TestCase("Caption")]
        [TestCase("ProgressPercentage")]
        public void IfUnzipActionIsNotTheCurrentAction_ShouldNotRaiseAnyPropertyChange(string propertyName)
        {

            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            var unzipAction = MockRepository.GenerateStub<IBackgroundAction>();
            var action = new DownloadAndUnzipFileAction(downloadAction, unzipAction);

            bool propertyChangedWasCalled = false;
            action.PropertyChanged += (sender, args) => propertyChangedWasCalled = true;

            action.Start();

            unzipAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs(propertyName));

            Assert.IsFalse(propertyChangedWasCalled);
        }

        [Test]
        public void Caption_WhenUnzipActionIsStarted_ShowTheCaptionFromTheUnzipAction()
        {
            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            var unzipAction = CreateUnzipActionStub("Unzip file1");
            var action = new DownloadAndUnzipFileAction(downloadAction, unzipAction);

            
            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs("Status"));


            Assert.AreEqual("Unzip file1", action.Caption);
        }

        [TestCase(0,0,0)]
        [TestCase(50, 0, 25)]
        [TestCase(100, 0, 50)]
        [TestCase(100, 50, 75)]
        [TestCase(100, 100, 100)]
        public void ProgressPercentage_IsComputedFromBothActions(int downloadActionProgress, int unzipActionProgress, int expectedProgress)
        {
            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            downloadAction.Stub(a => a.ProgressPercentage).Return(downloadActionProgress);
            var unzipAction = MockRepository.GenerateStub<IBackgroundAction>();
            unzipAction.Stub(a => a.ProgressPercentage).Return(unzipActionProgress);
            var action = new DownloadAndUnzipFileAction(downloadAction, unzipAction);

            
            Assert.AreEqual(expectedProgress, action.ProgressPercentage);
        }



        [Test]
        public void Cancel_IfDownloadActionIsCurrentAction_ShouldCallCancelInDownloadAction()
        {

            var downloadAction = MockRepository.GenerateMock<IBackgroundAction>();
            downloadAction.Expect(a => a.Cancel());
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());


            action.Start();
            

            action.Cancel();

            downloadAction.VerifyAllExpectations();
        }


        [Test]
        public void Cancel_IfDownloadActionIsNotCurrentAction_ShouldNotCallCancelInDownloadAction()
        {

            var downloadAction = MockRepository.GenerateMock<IBackgroundAction>();
            downloadAction.Expect(a => a.Cancel()).Repeat.Never();
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());


            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs("Status"));

            action.Cancel();

            downloadAction.VerifyAllExpectations();
        }


        [Test]
        public void Cancel_IfUnzipActionIsCurrentAction_ShouldCallCancelInUnzipAction()
        {

            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            var unzipAction = MockRepository.GenerateMock<IBackgroundAction>();
            unzipAction.Expect(a => a.Cancel());
            
            var action = new DownloadAndUnzipFileAction(downloadAction, unzipAction);


            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs("Status"));

            action.Cancel();

            unzipAction.VerifyAllExpectations();
        }


        [Test]
        public void Cancel_IfUnzipActionIsNotCurrentAction_ShouldNotCallCancelInUnzipAction()
        {

            var downloadAction = MockRepository.GenerateStub<IBackgroundAction>();
            var unzipAction = MockRepository.GenerateMock<IBackgroundAction>();
            unzipAction.Expect(a => a.Cancel()).Repeat.Never();

            var action = new DownloadAndUnzipFileAction(downloadAction, unzipAction);


            action.Start();

            
            action.Cancel();

            unzipAction.VerifyAllExpectations();
        }

        [Test]
        public void Dispose_ShouldAlwaysCallDisposeForDownloadAction1()
        {

            var downloadAction = MockRepository.GenerateMock<IBackgroundAction>();
            downloadAction.Expect(a => a.Dispose());
            
            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());

            action.Start();

            downloadAction.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            downloadAction.Raise(a => a.PropertyChanged += null, downloadAction, new PropertyChangedEventArgs("Status"));

            action.Dispose();
            
            downloadAction.VerifyAllExpectations();
        }

        [Test]
        public void Dispose_ShouldAlwaysCallDisposeForDownloadAction2()
        {

            var downloadAction = MockRepository.GenerateMock<IBackgroundAction>();
            downloadAction.Expect(a => a.Dispose());

            var action = new DownloadAndUnzipFileAction(downloadAction, CreateUnzipActionStub());

            action.Dispose();

            downloadAction.VerifyAllExpectations();
        }

        [Test]
        public void Dispose_ShouldAlwaysCallDisposeForUnzipAction()
        {

            var unzipAction = MockRepository.GenerateMock<IBackgroundAction>();
            unzipAction.Expect(a => a.Dispose());

            var action = new DownloadAndUnzipFileAction(CreateDownloadActionStub(), unzipAction);

            action.Dispose();

            unzipAction.VerifyAllExpectations();
        }

        private IBackgroundAction CreateDownloadActionStub(string caption = "download", BackgroundActionStatus status = BackgroundActionStatus.Waiting)
        {
            var action = MockRepository.GenerateStub<IBackgroundAction>();
            action.Stub(a => a.Caption).Return(caption);
            action.Stub(a => a.Status).Return(status);
            return action;
        }

        

        private IBackgroundAction CreateUnzipActionStub(string caption = "unzip", BackgroundActionStatus status = BackgroundActionStatus.Waiting)
        {
            var action = MockRepository.GenerateStub<IBackgroundAction>();
            action.Stub(a => a.Caption).Return(caption);
            action.Stub(a => a.Status).Return(status);
            return action;

        }

        
    }
}
