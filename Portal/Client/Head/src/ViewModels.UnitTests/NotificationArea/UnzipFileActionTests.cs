using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    [TestFixture]
    public class UnzipFileActionTests
    {

        [Test]
        public void Constructor_InitialProgressPercentageShouldBeZero()
        {
            var action = CreateUnzipFileAction();
            Assert.AreEqual(0, action.ProgressPercentage);
        }


        [Test]
        public void Constructor_InitialStatusShouldBeWaiting()
        {
            var action = CreateUnzipFileAction();
            Assert.AreEqual(BackgroundActionStatus.Waiting, action.Status);
        }

        [Test]
        public void Start_SetTheStatus_ToInProgress()
        {
            var action = CreateUnzipFileAction();

            action.Start();

            Assert.AreEqual(BackgroundActionStatus.InProgress, action.Status);

        }

        [Test]
        public void Start_IfAlreadyInProgress_ThrowInvalidOperationException()
        {
            var action = CreateUnzipFileAction();

            action.Start();

            Assert.Throws<InvalidOperationException>(() => action.Start());
        }


        [Test]
        public void Start_ShouldCall_UnzipInTheZipFileExtractor()
        {

            var zipFileExtractor = MockRepository.GenerateMock<IZipFileExtractor>();
            zipFileExtractor.Expect(extractor => extractor.Unzip(null, null)).IgnoreArguments();
            
            var action = CreateUnzipFileAction(CreateServiceLocator(zipFileExtractor));

            action.Start();

            zipFileExtractor.VerifyAllExpectations();
        }

        [Test]
        public void Start_WhenZipExtractorRaisesProgressChanged_UpdateProgressPercentageProperty()
        {

            var zipFileExtractor = MockRepository.GenerateStub<IZipFileExtractor>();
            
            var action = CreateUnzipFileAction(CreateServiceLocator(zipFileExtractor));

            action.Start();


            zipFileExtractor.Raise(a => a.UnzipProgressChanged += null, zipFileExtractor, new ActionProgressChangedEventArgs(33));

            Assert.AreEqual(33, action.ProgressPercentage);
        }


        [Test]
        public void Start_WhenZipExtractorRaisesUnzipCompleted_StatusShouldBeDone()
        {

            var zipFileExtractor = MockRepository.GenerateStub<IZipFileExtractor>();

            var action = CreateUnzipFileAction(CreateServiceLocator(zipFileExtractor));

            action.Start();
            
            zipFileExtractor.Raise(a => a.UnzipCompleted += null, zipFileExtractor, new AsyncCompletedEventArgs(null, false, null));

            Assert.AreEqual(BackgroundActionStatus.Done, action.Status);
        }


        [Test]
        public void Start_WhenZipExtractorRaisesUnzipCompletedWithException_StatusShouldBeError()
        {

            var zipFileExtractor = MockRepository.GenerateStub<IZipFileExtractor>();

            var action = CreateUnzipFileAction(CreateServiceLocator(zipFileExtractor));

            action.Start();

            zipFileExtractor.Raise(a => a.UnzipCompleted += null, zipFileExtractor, new AsyncCompletedEventArgs(new ApplicationException(), false, null));

            Assert.AreEqual(BackgroundActionStatus.Error, action.Status);
        }

        [Test]
        public void Start_WhenZipExtractorRaisesUnzipCompletedWithException_ErrorMessageShouldContainExceptionMessage()
        {

            var zipFileExtractor = MockRepository.GenerateStub<IZipFileExtractor>();

            var action = CreateUnzipFileAction(CreateServiceLocator(zipFileExtractor));

            action.Start();

            zipFileExtractor.Raise(a => a.UnzipCompleted += null, zipFileExtractor, new AsyncCompletedEventArgs(new ApplicationException("The message"), false, null));

            Assert.AreEqual("The message", action.ErrorMessage);
        }


        [Test]
        public void Start_WhenZipExtractorRaisesUnzipCompletedWithException_ErrorDetailsShouldContainTheWholeExceptionString()
        {

            var zipFileExtractor = MockRepository.GenerateStub<IZipFileExtractor>();

            var action = CreateUnzipFileAction(CreateServiceLocator(zipFileExtractor));

            action.Start();

            var exception = new ApplicationException("The message");
            zipFileExtractor.Raise(a => a.UnzipCompleted += null, zipFileExtractor, new AsyncCompletedEventArgs(exception, false, null));

            Assert.AreEqual(exception.ToString(), action.ErrorDetails);
        }

        [Test]
        public void Start_WhenZipExtractorRaisesUnzipCompletedWithCancel_StatusShouldBeCancel()
        {

            var zipFileExtractor = MockRepository.GenerateStub<IZipFileExtractor>();

            var action = CreateUnzipFileAction(CreateServiceLocator(zipFileExtractor));

            action.Start();

            zipFileExtractor.Raise(a => a.UnzipCompleted += null, zipFileExtractor, new AsyncCompletedEventArgs(null, true, null));

            Assert.AreEqual(BackgroundActionStatus.Canceled, action.Status);
        }


        [Test]
        public void Cancel_ShouldCall_Cancel_InTheZipFileExtractor()
        {

            var zipFileExtractor = MockRepository.GenerateMock<IZipFileExtractor>();
            zipFileExtractor.Expect(extractor => extractor.Cancel()).IgnoreArguments();

            var action = CreateUnzipFileAction(CreateServiceLocator(zipFileExtractor));

            action.Start();
            action.Cancel();

            zipFileExtractor.VerifyAllExpectations();
        }


        [Test]
        public void Cancel_IfCalledWithoutStart_ShouldNotFail()
        {
            var action = CreateUnzipFileAction();
            action.Cancel();
        }



        [Test]
        public void Dispose_ShouldCall_Dispose_InTheZipFileExtractor()
        {

            var zipFileExtractor = MockRepository.GenerateMock<IZipFileExtractor>();
            zipFileExtractor.Expect(extractor => extractor.Dispose()).IgnoreArguments();

            var action = CreateUnzipFileAction(CreateServiceLocator(zipFileExtractor));

            action.Start();
            action.Dispose();

            zipFileExtractor.VerifyAllExpectations();
        }


        [Test]
        public void Disposed_IfCalledWithoutStart_ShouldNotFail()
        {
            var action = CreateUnzipFileAction();
            action.Dispose();
        }

        private UnzipFileAction CreateUnzipFileAction(IServiceLocator serviceLocator)
        {
            return new UnzipFileAction(@"C:\temp\download\file.zip", @"C:\temp\extracted", serviceLocator);
        }
        
        private UnzipFileAction CreateUnzipFileAction()
        {
            return CreateUnzipFileAction(CreateServiceLocator());
        }

        private IServiceLocator CreateServiceLocator()
        {
            return CreateServiceLocator(MockRepository.GenerateStub<IZipFileExtractor>());
        }

        private IServiceLocator CreateServiceLocator(IZipFileExtractor zipFileExtractor)
        {
            var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            serviceLocator.Stub(l => l.GetInstance<IZipFileExtractor>()).Return(zipFileExtractor);

            return serviceLocator;
        }

    }
}
