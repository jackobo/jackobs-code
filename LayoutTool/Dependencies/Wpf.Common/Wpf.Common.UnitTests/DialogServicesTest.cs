using System;

using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Prism.Logging;

using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.UIServices;
using NSubstitute;
using Spark.Infra.Exceptions;
using Spark.Infra.Logging;

namespace Spark.Wpf.Common
{
    [TestFixture]
    public class DialogServicesTest
    {
       

        [Test]
        public void ShowOkCancelDialogBox_CreatesTheWindowWithTheAppropriateDialogViewModel()
        {
            var window = Substitute.For<IModalWindow>();
            var windowsFactory = CreateWindowsFactory(window);
            
            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));
            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();

            dialogServices.ShowOkCancelDialogBox(dialogViewModel);

            Assert.IsTrue(object.ReferenceEquals(dialogViewModel, window.Commands.DialogViewModel));
        }

        

        [Test]
        public void ShowOkCancelDialogBox_SetsTheOkCommandInTheDialogCommands()
        {

            var window = Substitute.For<IModalWindow>();
            var windowsFactory = CreateWindowsFactory(window);
            
            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));
            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();

            dialogServices.ShowOkCancelDialogBox(dialogViewModel);

            Assert.IsNotNull(window.Commands.OkCommand);
        }


        [Test]
        public void ShowOkCancelDialogBox_SetsTheCancelCommandInTheDialogCommands()
        {

            var window = Substitute.For<IModalWindow>();
            var windowsFactory = CreateWindowsFactory(window);

            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));
            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();

            dialogServices.ShowOkCancelDialogBox(dialogViewModel);

            Assert.IsNotNull(window.Commands.CancelCommand);
        }

        [Test]
        public void ShowOkCancelDialogBox_ShouldCall_ShowDialogMethodOfTheWindow()
        {
            var window = Substitute.For<IModalWindow>();
            

            var windowsFactory = CreateWindowsFactory(window);
            
            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));

            var dialogBoxViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();

            dialogServices.ShowOkCancelDialogBox(dialogBoxViewModel);

            window.Received().ShowModal();
        }

        [Test]
        public void ShowOkCancelDialogBox_WhenOkIsExecutedInTheDialog_ReturnOK()
        {
            var windowsFactory = CreateWindowsFactory(CreateModalWindowStub(cmds => cmds.OkCommand.Execute(null)));
            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));
            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();

            var actualResult = dialogServices.ShowOkCancelDialogBox(dialogViewModel);

            Assert.AreEqual(OkCancelDialogBoxResult.Ok, actualResult);
        }

        [Test]
        public void ShowOkCancelDialogBox_WhenDialogCancelCommandIsExecuted_ReturnCancel()
        {
            var windowsFactory = CreateWindowsFactory(CreateModalWindowStub(cmds => cmds.CancelCommand.Execute(null)));
            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));
            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();

            var actualResult = dialogServices.ShowOkCancelDialogBox(dialogViewModel);

            Assert.AreEqual(OkCancelDialogBoxResult.Cancel, actualResult);
        }


        [Test]
        public void ShowOkCancelDialogBox_WhenDialogOkCommandIsExecuted_CallViewModelExecuteOk()
        {

            var windowsFactory = CreateWindowsFactory(CreateModalWindowStub(cmds => cmds.OkCommand.Execute(null)));

            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));
            
            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();
            

            dialogServices.ShowOkCancelDialogBox(dialogViewModel);


            dialogViewModel.Received().ExecuteOk();
        }

        [Test]
        public void ShowOkCancelDialogBox_WhenDialogCancelCommandIsExecuted_CallViewModelExecuteCancel()
        {

            var windowsFactory = CreateWindowsFactory(CreateModalWindowStub(cmds => cmds.CancelCommand.Execute(null)));

            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));
            
            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();
            

            dialogServices.ShowOkCancelDialogBox(dialogViewModel);


            dialogViewModel.Received().ExecuteCancel();
        }

        [Test]
        public void ShowOkCancelDialogBox_WhenDialogOkCommandIsExecuted_CloseTheWindow()
        {

            var window = CreateModalWindowMock(cmds => cmds.OkCommand.Execute(null));
            
            var windowsFactory = CreateWindowsFactory(window);

            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));

            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();
            
            dialogServices.ShowOkCancelDialogBox(dialogViewModel);

            window.Received().Close();
            
        }

        [Test]
        public void ShowOkCancelDialogBox_WhenDialogCancelCommandIsExecuted_CloseTheWindow()
        {

            var window = CreateModalWindowMock(cmds => cmds.CancelCommand.Execute(null));
            
            var windowsFactory = CreateWindowsFactory(window);
            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));

            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();
            
            dialogServices.ShowOkCancelDialogBox(dialogViewModel);


            window.Received().Close();
        }



        [Test]
        public void ShowOkCancelDialogBox_WhenExceptionIsThrownOnOkCommand_ShouldNotCloseTheWindow()
        {

            var window = CreateModalWindowMock(cmds => cmds.OkCommand.Execute(null));
            
            var windowsFactory = CreateWindowsFactory(window);

            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));

            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();
            dialogViewModel.When(vm => vm.ExecuteOk()).Do(vm => { throw new ApplicationException(); });
            try
            {
                dialogServices.ShowOkCancelDialogBox(dialogViewModel);
            }
            catch (ApplicationException)
            {
            }

            window.DidNotReceive().Close();
        }

        [Test]
        public void ShowOkCancelDialogBox_WhenExceptionIsThrownOnCancelCommand_ShouldNotCloseTheWindow()
        {

            var window = CreateModalWindowMock(cmds => cmds.CancelCommand.Execute(null));
            
            var windowsFactory = CreateWindowsFactory(window);

            var dialogServices = CreateDialogServices(CreateServiceLocator(windowsFactory));

            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();

            dialogViewModel.When(vm => vm.ExecuteCancel())
                           .Do(vm => { throw new ApplicationException(); });
            try
            {
                dialogServices.ShowOkCancelDialogBox(dialogViewModel);
            }
            catch (ApplicationException)
            {

            }

            window.DidNotReceive().Close();

        }


        [Test]
        public void ShowOkCancelDialogBox_WhenValidationExceptionIsThrownOnTheExectueOK_DoNotLogException()
        {
            var serviceLocatorBuilder = new ServiceLocatorStubBuilder();
            serviceLocatorBuilder.WindowsFactory = CreateWindowsFactory(CreateModalWindowStub(cmds => cmds.OkCommand.Execute(null)));


            var logger = Substitute.For<ILogger>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(typeof(DialogServices)).Returns(logger);

            serviceLocatorBuilder.LoggerFactory = loggerFactory;


            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();

            dialogViewModel.When(vm => vm.ExecuteOk())
                           .Do(vm => { throw new ValidationException("Validation failed!"); });

            var dialogServices = CreateDialogServices(serviceLocatorBuilder.Build());
            try
            {
                dialogServices.ShowOkCancelDialogBox(dialogViewModel);
            }
            catch (ValidationException)
            {
            }

            logger.DidNotReceiveWithAnyArgs().Exception("", null);
            
        }


        [Test]
        public void ShowOkCancelDialogBox_WhenValidationExceptionIsThrownOnTheExecuteCancel_DontLogException()
        {
            var serviceLocatorBuilder = new ServiceLocatorStubBuilder();
            serviceLocatorBuilder.WindowsFactory = CreateWindowsFactory(CreateModalWindowStub(cmds => cmds.CancelCommand.Execute(null)));

            var logger = Substitute.For<ILogger>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            serviceLocatorBuilder.LoggerFactory = loggerFactory;

            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();

            dialogViewModel.When(vm => vm.ExecuteCancel())
                           .Do(vm => { throw new ValidationException("Validatation failed!"); });

            var dialogServices = CreateDialogServices(serviceLocatorBuilder.Build());
            try
            {
                dialogServices.ShowOkCancelDialogBox(dialogViewModel);
            }
            catch (ValidationException)
            {
            }


            logger.DidNotReceiveWithAnyArgs().Exception("", null);
        }


        [Test]
        public void ShowOkCancelDialogBox_When()
        {
            var serviceLocatorBuilder = new ServiceLocatorStubBuilder();

            var appServices = Substitute.For<IApplicationServices>();
            
            serviceLocatorBuilder.ApplicationServices = appServices;
            
            var dialogViewModel = Substitute.For<IOkCancelDialogBoxViewModel>();


            var dialogServices = CreateDialogServices(serviceLocatorBuilder.Build());
            dialogServices.ShowOkCancelDialogBox(dialogViewModel);

            appServices.ReceivedWithAnyArgs().ExecuteOnUIThread(null);
        }


        

        IWindowsFactory CreateWindowsFactory(IModalWindow window)
        {
            var windowsFactory = Substitute.For<IWindowsFactory>();
            windowsFactory.CreateModalWindow().Returns(window);
            return windowsFactory;
        }

        IModalWindow CreateModalWindowStub(Action<IDialogBoxCommands> commandToExecute)
        {
            var window = Substitute.For<IModalWindow>();
            window.When(w => w.ShowModal())
                  .Do(w => commandToExecute(window.Commands));
            
            return window;
        }

        IModalWindow CreateModalWindowMock(Action<IDialogBoxCommands> commandToExecute)
        {
            var window = Substitute.For<IModalWindow>();

            window.When(w => w.ShowModal())
                  .Do(w => commandToExecute(window.Commands));
            
            return window;
        }

        private DialogServices CreateDialogServices()
        {
            return CreateDialogServices(CreateServiceLocator());
        }

      

        private DialogServices CreateDialogServices(IServiceLocator serviceLocator)
        {
            return new DialogServices(serviceLocator.GetInstance<IWindowsFactory>(), serviceLocator.GetInstance<IApplicationServices>());
        }



        IServiceLocator CreateServiceLocator()
        {
            return new ServiceLocatorStubBuilder().Build();
        }

       

        IServiceLocator CreateServiceLocator(IWindowsFactory windowFactory)
        {
            var builder = new ServiceLocatorStubBuilder();
            builder.WindowsFactory = windowFactory;
            return builder.Build();
        }


        private class ServiceLocatorStubBuilder
        {
            public ServiceLocatorStubBuilder()
            {
                Logger = Substitute.For<ILogger>();
                LoggerFactory = Substitute.For<ILoggerFactory>();

                LoggerFactory.CreateLogger(Arg.Any<Type>()).Returns(Logger);

                WindowsFactory = Substitute.For<IWindowsFactory>();
                var window = Substitute.For<IModalWindow>();
                WindowsFactory.CreateModalWindow().Returns(window);
                
                MessageBox = Substitute.For<IMessageBox>();
                ApplicationServices = GenerateApplicationServicesStub();
            }

            private IApplicationServices GenerateApplicationServicesStub()
            {
                var appServices = Substitute.For<IApplicationServices>();
                appServices.ExecuteOnUIThread(Arg.Do<Action>(arg => arg()));
                return appServices;
            }
            public ILogger Logger { get; set; }
            public ILoggerFactory LoggerFactory { get; set; }
            public IWindowsFactory WindowsFactory { get; set; }
            public IMessageBox MessageBox { get; set; }
            public IApplicationServices ApplicationServices { get; set; }

            public IServiceLocator Build()
            {
                var serviceLocator = Substitute.For<IServiceLocator>();
                serviceLocator.GetInstance<ILoggerFactory>().Returns(LoggerFactory);
                serviceLocator.GetInstance<IWindowsFactory>().Returns(WindowsFactory);
                serviceLocator.GetInstance<IMessageBox>().Returns(MessageBox);
                serviceLocator.GetInstance<IApplicationServices>().Returns(ApplicationServices);

                return serviceLocator;
            }
        }

    }
}
