﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Unity;
using Spark.Infra.DependencyInjection;
using Spark.Infra.Exceptions;
using Spark.Infra.Logging;
using Spark.Infra.Windows;
using Spark.Wpf.Common.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.UIServices;

namespace Spark.Wpf.Common
{

    public interface IAppBootstrapper
    {
        string GetApplicationUniqueName();
    }

    public abstract class AppBootstrapper<TApp, TShell> : UnityBootstrapper , IAppBootstrapper, IDependencyInjectionContainer
        where TApp : Application, new()
        where TShell : Window, new()
    {
        protected override System.Windows.DependencyObject CreateShell()
        {
            var mainWindow = new TShell();

            mainWindow.Title = GetApplicationFriendlyName();

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed && System.Deployment.Application.ApplicationDeployment.CurrentDeployment != null)
            {
                mainWindow.Title += string.Format(" [{0}]", System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString());
            }
            return mainWindow;
        }



        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = AppDomain.CurrentDomain.BaseDirectory };
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (TShell)Shell;
            Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;


        }

        protected override void InitializeModules()
        {
            ShowSplashScreen();
            base.InitializeModules();
        }

        protected virtual void ShowSplashScreen()
        {
            var splashModule = new SplashModule<TShell>((TShell)this.Shell, this.Container.Resolve<IEventAggregator>());
            splashModule.Start();
        }

        ILoggerFactory _loggerFactory;

        protected override ILoggerFacade CreateLogger()
        {
            InitLoggerFactory();
            return new LoggerFacade(_loggerFactory);
        }

        protected virtual void InitLoggerFactory()
        {
            _loggerFactory = Log4NetNotifierFactory.FromCurrentUserAppData(GetApplicationUniqueName());
        }

        public abstract string GetApplicationUniqueName();
        
        protected abstract string GetApplicationFriendlyName();

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            RegisterLoggerFactory();

            RegisterOperatingSystemServices();

            RegisterApplicationServices();
        }

        private void RegisterLoggerFactory()
        {
            RegisterInstance(_loggerFactory);
        }

        private void RegisterOperatingSystemServices()
        {
            this.Container.Resolve<OperatingSystemServices>().RegisterWithContainer(this);
        }

        private void RegisterApplicationServices()
        {
            new WpfApplicationServices(GetApplicationUniqueName(), 
                                       Container.Resolve<ILoggerFactory>(),
                                       Container.Resolve<IThreadingServices>())
                                      .RegisterWithContainer(this);
           
        }

        public override void Run(bool runWithDefaultConfiguration)
        {

            
            TApp app = new TApp();
            app.DispatcherUnhandledException += app_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            app.Startup += (sndr, args) =>
            {
                base.Run(runWithDefaultConfiguration);
            };

            app.Run();
        }

        
        protected virtual TApp CreateApp()
        {
            return new TApp();
        }
      

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogAndShowException("CurrentDomain_UnhandledException", e.ExceptionObject);
        }

        void app_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            LogAndShowException("app_DispatcherUnhandledException", e.Exception);
        }



        private void LogAndShowException(string handlerName, object exception)
        {
            LogException(handlerName, exception);

            ShowException(exception);

        }

        private static void ShowException(object exception)
        {
            Application.Current.Dispatcher.Invoke(new Action(delegate ()
            {
                string message = string.Empty;
                var ex = exception as Exception;
                if (ex != null)
                {
                    message = ex.Message;
                }
                else
                {
                    message = ex.ToString();
                }

                MessageBox.Show(message.Substring(0, Math.Min(1000, message.Length)));
            }));
        }

        private void LogException(string handlerName, object exception)
        {
            

            Prism.Logging.Category category = Prism.Logging.Category.Exception;
            Prism.Logging.Priority priority = Prism.Logging.Priority.High;

            if (exception is ValidationException)
            {
                category = Prism.Logging.Category.Info;
                priority = Prism.Logging.Priority.Low;
            }
            
            var msg = string.Format("Application: {0}\n\rHandler: {1}\n\rException details:\r{2}",
                                GetApplicationFriendlyName(),
                                handlerName,
                                exception.ToString());

            Logger.Log(msg, category, priority);
            
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            this.Container.RegisterType<TFrom, TTo>();
        }

        public void RegisterInstance<T>(T instance)
        {
            this.Container.RegisterInstance<T>(instance);
        }
    }
}
