using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    [TestFixture]
    public class BackgroundActionsAreaTests
    {

        [Test]
        public void AddAction_AddsTheActions_ToTheActionsCollection()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            backgroundActionsArea.AddAction(GenerateBackgroundActionStub());
            backgroundActionsArea.AddAction(GenerateBackgroundActionStub());

            Assert.AreEqual(2, backgroundActionsArea.Actions.Count);
        }


        [Test]
        public void AddAction_IfIsTheFirstAction_StartIt()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var action = MockRepository.GenerateMock<IBackgroundAction>();
            
            action.Expect(a => a.Start());

            backgroundActionsArea.AddAction(action);

            action.VerifyAllExpectations();
        }


        IBackgroundAction GenerateBackgroundActionStub(BackgroundActionStatus status = BackgroundActionStatus.Waiting)
        {
            var action = MockRepository.GenerateStub<IBackgroundAction>();
            action.Stub(a => a.Status).Return(status);
            return action;

        }

        [Test]
        public void AddAction_IfAnotherActionIsExecuting_DontStart()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = GenerateBackgroundActionStub(BackgroundActionStatus.InProgress);
            var a2 = MockRepository.GenerateMock<IBackgroundAction>();


            a2.Expect(a => a.Start()).Repeat.Never();

            backgroundActionsArea.AddAction(a1);
            backgroundActionsArea.AddAction(a2);

            a2.VerifyAllExpectations();

        }

        [Test]
        public void AddAction_IfAllOtherActionsFinishedExecuting_StartAddedAction()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = GenerateBackgroundActionStub(BackgroundActionStatus.Done);
            backgroundActionsArea.AddAction(a1);
            

            var a2 = MockRepository.GenerateMock<IBackgroundAction>();

            a2.Expect(a => a.Start());

            backgroundActionsArea.AddAction(a2);

            a2.VerifyAllExpectations();

        }


        [Test]
        public void AddAction_WhenOneOneActionFinish_StartTheNextOne()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = MockRepository.GenerateStub<IBackgroundAction>();
            backgroundActionsArea.AddAction(a1);

            var a2 = MockRepository.GenerateMock<IBackgroundAction>();
            a2.Expect(a => a.Start());

            backgroundActionsArea.AddAction(a2);


            a1.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            a1.Raise(a => a.PropertyChanged += null, a1, new PropertyChangedEventArgs("Status"));
            
            a2.VerifyAllExpectations();

        }


        [Test]
        public void AddAction_StartingTheFirstAction_SetsTheCurrentActionProperty()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a1);

            Assert.IsTrue(object.ReferenceEquals(a1, backgroundActionsArea.CurrentAction));

        }


        [Test]
        public void AddAction_StartingANewAction_SetsTheCurrentActionProperty()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = MockRepository.GenerateStub<IBackgroundAction>();
            backgroundActionsArea.AddAction(a1);

            var a2 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a2);


            a1.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            a1.Raise(a => a.PropertyChanged += null, a1, new PropertyChangedEventArgs("Status"));

            Assert.IsTrue(object.ReferenceEquals(a2, backgroundActionsArea.CurrentAction));

        }


        [Test]
        public void RemoveActionCommand_WhenExecuted_RemovesTheActionProvidedAsArgument()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a1);

            var a2 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a2);

            var a3 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a3);


            backgroundActionsArea.RemoveActionCommand.Execute(a2);

            
            Assert.AreEqual(2, backgroundActionsArea.Actions.Count);

        }


        [Test]
        public void RemoveActionCommand_WhenExecuted_ShouldCallActionDispose()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a1);

            var a2 = MockRepository.GenerateMock<IBackgroundAction>();
            a2.Expect(a => a.Dispose());
            backgroundActionsArea.AddAction(a2);

            var a3 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a3);


            backgroundActionsArea.RemoveActionCommand.Execute(a2);


            a2.VerifyAllExpectations();

        }


        [Test]
        public void RemoveActionCommand_IfCurrentExecutingActionIsRemoved_ShouldCallCancelMethodInTheBackgroundAction()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = MockRepository.GenerateStub<IBackgroundAction>();
            backgroundActionsArea.AddAction(a1);

            var a2 = MockRepository.GenerateMock<IBackgroundAction>();
            a2.Expect(a => a.Cancel());
            backgroundActionsArea.AddAction(a2);
            


            a1.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            a1.Raise(a => a.PropertyChanged += null, a1, new PropertyChangedEventArgs("Status"));
            
            backgroundActionsArea.RemoveActionCommand.Execute(a2);

            a2.VerifyAllExpectations();

        }

        [Test]
        public void RemoveActionCommand_IfCurrentExecutingActionIsRemoved_CurrentActionShouldBeNull()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = MockRepository.GenerateStub<IBackgroundAction>();
            backgroundActionsArea.AddAction(a1);

            var a2 = MockRepository.GenerateStub<IBackgroundAction>();
            backgroundActionsArea.AddAction(a2);



            a1.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            a1.Raise(a => a.PropertyChanged += null, a1, new PropertyChangedEventArgs("Status"));

            backgroundActionsArea.RemoveActionCommand.Execute(a2);

            Assert.IsNull(backgroundActionsArea.CurrentAction);

        }

        [Test]
        public void RemoveAllCommand_WhenExecuted_ClearsTheActionList()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a1);

            var a2 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a2);
            
            backgroundActionsArea.RemoveAllActionsCommand.Execute(null);

            Assert.AreEqual(0, backgroundActionsArea.Actions.Count);

        }

        [Test]
        public void RemoveAllCommand_WhenExecuted_SetstheCurrentActionToNull()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a1);

            var a2 = GenerateBackgroundActionStub();
            backgroundActionsArea.AddAction(a2);

            backgroundActionsArea.RemoveAllActionsCommand.Execute(null);

            Assert.IsNull(backgroundActionsArea.CurrentAction);

        }

        [Test]
        public void RemoveActionCommand_IfNullIsProvided_ThrowsExecption()
        {

            var backgroundActionsArea = CreateBackgroundActionArea();
            Assert.Throws<ArgumentNullException>(() => backgroundActionsArea.RemoveActionCommand.Execute(null));
        }
        

        [Test]
        public void Constructor_AutoStartActionWhenAddedShouldBeInitializedWithTrue()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();
            Assert.AreEqual(true, backgroundActionsArea.AutoStartActionWhenAdded);
        }

        [Test]
        public void AddAction_IfAutoStartActionWhenAddedFlagIsFalse_DonStartTheAction()
        {

            var backgroundActionsArea = CreateBackgroundActionArea();
            backgroundActionsArea.AutoStartActionWhenAdded = false;
            var action = MockRepository.GenerateMock<IBackgroundAction>();
            action.Expect(a => a.Start()).Repeat.Never();

            backgroundActionsArea.AddAction(action);

            action.VerifyAllExpectations();
        }


        [TestCase(BackgroundActionStatus.Canceled)]
        [TestCase(BackgroundActionStatus.Error)]
        [TestCase(BackgroundActionStatus.Waiting)]
        public void ResumeUnfinishedCommand_ShouldCallStartActionInTheFirstActionWithStatusDifferentThanDone(BackgroundActionStatus statusThatCanResumeFrom)
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            backgroundActionsArea.AutoStartActionWhenAdded = false;

            var a1 = GenerateBackgroundActionStub(BackgroundActionStatus.Done);
            backgroundActionsArea.AddAction(a1);

            var a2 = MockRepository.GenerateMock<IBackgroundAction>();
            a2.Stub(a => a.Status).Return(statusThatCanResumeFrom);
            a2.Expect(a => a.Start());
            backgroundActionsArea.AddAction(a2);

            backgroundActionsArea.ResumeUnfinishedCommand.Execute(null);

            a2.VerifyAllExpectations();

        }

        [Test]
        public void ResumeUnfinishedCommand_ShouldNotCallStartForDoneActions()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            backgroundActionsArea.AutoStartActionWhenAdded = false;

            var a1 = MockRepository.GenerateMock<IBackgroundAction>();
            a1.Stub(a => a.Status).Return(BackgroundActionStatus.Done);
            a1.Expect(a => a.Start()).Repeat.Never();
            backgroundActionsArea.AddAction(a1);

            var a2 = GenerateBackgroundActionStub(BackgroundActionStatus.Waiting);
            backgroundActionsArea.AddAction(a2);

            backgroundActionsArea.ResumeUnfinishedCommand.Execute(null);

            a1.VerifyAllExpectations();

        }

        [Test]
        public void ResumeUnfinishedCommand_IfAnyActionInProgress_ThrowInvalidOperationException()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            backgroundActionsArea.AutoStartActionWhenAdded = false;

            var a1 = GenerateBackgroundActionStub(BackgroundActionStatus.InProgress);           
            backgroundActionsArea.AddAction(a1);

            var a2 = GenerateBackgroundActionStub(BackgroundActionStatus.Waiting);
            backgroundActionsArea.AddAction(a2);

            Assert.Throws<InvalidOperationException>(() => backgroundActionsArea.ResumeUnfinishedCommand.Execute(null));
        }



        [TestCase(true, BackgroundActionStatus.Waiting, BackgroundActionStatus.Canceled, BackgroundActionStatus.Error, BackgroundActionStatus.Done)]
        [TestCase(false, BackgroundActionStatus.InProgress, BackgroundActionStatus.Canceled, BackgroundActionStatus.Error, BackgroundActionStatus.Waiting, BackgroundActionStatus.Done)]
        [TestCase(false, BackgroundActionStatus.InProgress, BackgroundActionStatus.InProgress)]
        [TestCase(false, BackgroundActionStatus.Done, BackgroundActionStatus.Done)]
        public void ResumeUnfinishedCommand_CanExecute_OnlyIfAtLeastOneActionHasTheAppropriateStatus(bool expectedValueForCanExecute, params BackgroundActionStatus[] status)
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            backgroundActionsArea.AutoStartActionWhenAdded = false;

            foreach (var s in status)
            {
                backgroundActionsArea.AddAction(GenerateBackgroundActionStub(s));
            }
                        
            Assert.AreEqual(expectedValueForCanExecute, backgroundActionsArea.ResumeUnfinishedCommand.CanExecute(null));

        }

        [Test]
        public void RemoveAllComman_CanExecute_IfNoAction_ReturnFalse()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            Assert.AreEqual(false, backgroundActionsArea.RemoveAllActionsCommand.CanExecute(null));

        }

        [Test]
        public void RemoveAllComman_CanExecute_IfHasAtLeastOneAction_ReturnTrue()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            backgroundActionsArea.AddAction(GenerateBackgroundActionStub());

            Assert.AreEqual(true, backgroundActionsArea.RemoveAllActionsCommand.CanExecute(null));

        }


        [Test]
        public void StopComman_CanExecute_IfHasAtLeastOneActionInProgress_ReturnTrue()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            backgroundActionsArea.AddAction(GenerateBackgroundActionStub(BackgroundActionStatus.InProgress));

            Assert.AreEqual(true, backgroundActionsArea.StopCommand.CanExecute(null));
        }


        [Test]
        public void StopComman_CanExecute_IfHasNoActionInProgress_ReturnFalse()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            backgroundActionsArea.AddAction(GenerateBackgroundActionStub(BackgroundActionStatus.Done));

            Assert.AreEqual(false, backgroundActionsArea.StopCommand.CanExecute(null));
        }

        [Test]
        public void StopComman_ShouldCallCancelForCurrentExecutingAction()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();

            var a1 = GenerateBackgroundActionStub(BackgroundActionStatus.Done);
            backgroundActionsArea.AddAction(a1);

            var a2 = MockRepository.GenerateMock<IBackgroundAction>();
            a2.Stub(a => a.Status).Return(BackgroundActionStatus.InProgress);
            a2.Expect(a => a.Cancel());
            backgroundActionsArea.AddAction(a2);

            backgroundActionsArea.StopCommand.Execute(null);

            a2.VerifyAllExpectations();
        }

        [Test]
        public void StopComman_IfStopCommandIsExecuted_DoNotContinueWithTheRemainingActions()
        {
            var backgroundActionsArea = CreateBackgroundActionArea();
            backgroundActionsArea.AutoStartActionWhenAdded = false;

            var a1 = MockRepository.GenerateStub<IBackgroundAction>();
            a1.Stub(a => a.Status).Return(BackgroundActionStatus.Canceled);
            backgroundActionsArea.AddAction(a1);
            
            var a2 = MockRepository.GenerateMock<IBackgroundAction>();
            a2.Stub(a => a.Status).Return(BackgroundActionStatus.Waiting);
            a2.Expect(a => a.Start()).Repeat.Never();
            backgroundActionsArea.AddAction(a2);

            backgroundActionsArea.StopCommand.Execute(null);

            a1.Raise(a => a.PropertyChanged += null, a1, new PropertyChangedEventArgs("Status"));

            a2.VerifyAllExpectations();
        }


        private BackgroundActionsArea CreateBackgroundActionArea()
        {
            /*var container = Helpers.UnityContainerHelper.Create();
            container.RegisterInstance<Interfaces.UserInteface.IApplicationServices>(MockRepository.GenerateStub<Interfaces.UserInteface.IApplicationServices>());
            ExecuteOnUIThreadIfPossible
             */ 
            return new BackgroundActionsArea();
        }
    }
}
