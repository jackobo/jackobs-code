using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spark.Infra.Types;
using Prism.Logging;
using Spark.Wpf.Common.Interfaces.UI;

using Spark.Infra.Windows;
using Spark.Infra.Logging;

namespace LayoutTool.MainProxy
{
    
    internal class EmbeddedMainProxyMockInstaller
    {

        ILogger _logger;
        IMainProxyMockController _mainProxyMockController;
        AppDomain _appDomain;

        public EmbeddedMainProxyMockInstaller(ILoggerFactory loggerFactory,
                                            IOperatingSystemServices operatingSystemServices,
                                            IApplicationServices applicationServices)
        {
            _logger = loggerFactory.CreateLogger(this.GetType());

            _applicationServices = applicationServices;
            _operatingSystemServices = operatingSystemServices;

        

            InstallMainProxyMock();

            InitializeMainProxyMockController();

            _applicationServices.ShuttingDown += ApplicationServices_ShuttingDown;
            
        }

        

        IApplicationServices _applicationServices;
        IOperatingSystemServices _operatingSystemServices;


        private void InitializeMainProxyMockController()
        {
            _appDomain = CreateAppDomain();
            _mainProxyMockController = CreateController(_appDomain);
            try
            {
                _mainProxyMockController.Start();
            }
            catch
            {
                ShutDown();
                throw;
            }
        }
  

        public void InstallMainProxyMock()
        {
            ExtractMainProxyBinaries();
            UpdateMainProxyConfigurationFile();
            UpdateLoggingXmlFile();
            CreateGenGaEventSource();
        }

        private void CreateGenGaEventSource()
        {
            string source = "GenGa";
            string log = "Application";

            try
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }
            }
            catch(Exception ex)
            {
                _logger.Exception($"{nameof(EmbeddedMainProxyMockInstaller)}.{nameof(InstallMainProxyMock)} failed!",
                                    ex);
            }
        }

        private void UpdateLoggingXmlFile()
        {
            try
            {
                var loggingXmlFilePath = Path.Combine(MainProxyMockBaseDirectory, "logging.xml");

                var xmlDoc = XDocument.Load(loggingXmlFilePath);

                var listenersXmlElement = xmlDoc.Root.Element("listeners");

                var rollingFileListenerElement = listenersXmlElement.Elements("add").FirstOrDefault(element => element.Attribute("name").Value == "Rolling File Listener");

                var logFile = Path.Combine(_applicationServices.GetUserAppDataFolder(), "MainProxyLogs", "MainProxyTrace.log");
                rollingFileListenerElement.Attribute("fileName").SetValue(logFile);
                xmlDoc.Save(loggingXmlFilePath);
            }
            catch (Exception ex)
            {
                _logger.Exception($"{nameof(EmbeddedMainProxyMockInstaller)}.{nameof(UpdateLoggingXmlFile)} failed!",
                                   ex);
            }
        }

        private void ExtractMainProxyBinaries()
        {

            _operatingSystemServices.FileSystem.DeleteFolder(MainProxyMockBaseDirectory);
            _operatingSystemServices.FileSystem.UnzipFile(MainProxyMockZipFile, MainProxyMockBaseDirectory);
        }

        private void UpdateMainProxyConfigurationFile()
        {
            var map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = MainProxyMockConfigurationFile;


            var configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            if (configuration.AppSettings.Settings.AllKeys.Contains(ConfigurationKeys.PlayerManagementMode))
            {
                configuration.AppSettings.Settings.Remove(ConfigurationKeys.PlayerManagementMode);
            }
            configuration.AppSettings.Settings.Add(ConfigurationKeys.PlayerManagementMode, ConfigurationKeys.PlayerManagementMode_Main);


            if (configuration.AppSettings.Settings.AllKeys.Contains(ConfigurationKeys.MainProxyListenPort))
            {
                configuration.AppSettings.Settings.Remove(ConfigurationKeys.MainProxyListenPort);
            }

            configuration.AppSettings.Settings.Add(ConfigurationKeys.MainProxyListenPort, Port.ToString());

            configuration.Save();
        }


        private AppDomain CreateAppDomain()
        {

            AppDomainSetup setup = new AppDomainSetup()
            {
                ApplicationBase = MainProxyMockBaseDirectory,
                ConfigurationFile = MainProxyMockConfigurationFile,
                ApplicationName = "MainProxyMock",
                PrivateBinPath = MainProxyMockBaseDirectory // + ";" + AppDomain.CurrentDomain.BaseDirectory

            };
            return AppDomain.CreateDomain("MainProxyMock", AppDomain.CurrentDomain.Evidence, setup);

        }


        private IMainProxyMockController CreateController(AppDomain appDomain)
        {
            return (IMainProxyMockController)appDomain.CreateInstanceFromAndUnwrap(this.GetType().Assembly.GetName().Name + ".dll",
                                                                                   typeof(MainProxyMockController).FullName);
        }


        private int Port
        {
            get { return 8500; }
        }

        private string MainProxyMockConfigurationFile
        {
            get { return Path.Combine(MainProxyMockBaseDirectory, "MainProxyMock.exe.config"); }
        }
        

        private string MainProxyMockBaseDirectory
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MainProxy"); }
        }

        private string MainProxyMockZipFile
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "MainProxyMock.zip"); }
        }


        private void ApplicationServices_ShuttingDown(object sender, EventArgs e)
        {
            ShutDown();

        }

        private void ShutDown()
        {
            try
            {
                if (_mainProxyMockController != null)
                    _mainProxyMockController.Stop();
            }
            catch (Exception ex)
            {
                _logger.Exception($"{nameof(EmbeddedMainProxyMockInstaller)}.{nameof(ShutDown)}: Stopping MainProxyMock failed!", ex);
            }

            try
            {
                if (_appDomain != null)
                    AppDomain.Unload(_appDomain);
            }
            catch (Exception ex)
            {
                _logger.Exception($"{nameof(EmbeddedMainProxyMockInstaller)}.{nameof(ShutDown)}: Unloading MainProxyMock AppDomain failed!", ex);
            }
        }
    }
}
