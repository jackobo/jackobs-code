using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace Spark.Infra.Logging
{
    public class Log4NetNotifierFactory : ILoggerFactory
    {
      
        private Log4NetNotifierFactory()
        {
            log4net.GlobalContext.Properties["MachineName"] = Environment.MachineName;
            log4net.GlobalContext.Properties["OSVersion"] = Environment.OSVersion.ToString();
        }


        public static ILoggerFactory FromConfigurationFileInApplicationFolder()
        {
            Log4NetFromAppFolderInitializer.Initialize();
            return new Log4NetNotifierFactory();
        }


        public static ILoggerFactory FromCurrentUserAppData(string applicationName)
        {
            Log4NetInCurrentUserAppData.Initialize(applicationName);
            return new Log4NetNotifierFactory();
        }

        public ILogger CreateLogger(Type forType)
        {
            return new Log4NetNotifier(forType);
        }

     
        internal class Log4NetFromAppFolderInitializer
        {
            private static readonly string Log4NetConfigurationFile = "log4net_config.xml";

            #region ILog4NetSystemInitializer Members

            public static void Initialize()
            {
                
                XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", Log4NetConfigurationFile)));
            }

            #endregion
        }


        internal class Log4NetInCurrentUserAppData
        {
            [DllImport("kernel32.dll")]
            static extern IntPtr GetConsoleWindow();

            private Log4NetInCurrentUserAppData(string applicationName)
            {
                this.ApplicationName = applicationName;
            }


            public static void Initialize(string applicationName)
            {
                new Log4NetInCurrentUserAppData(applicationName).Initialize();
            }


            string ApplicationName { get; set; }

            

            public void Initialize()
            {
                List<IAppender> appenders = new List<IAppender>();
                appenders.Add(CreateFileAppender());

                var smtpAppender = CreateSmtpAppender();

                if (smtpAppender != null)
                    appenders.Add(smtpAppender);

                if (IsConsoleApplication)
                    appenders.Add(CreateConsoleAppender());
                

                BasicConfigurator.Configure(appenders.ToArray());
            }

            private IAppender CreateConsoleAppender()
            {
                var consoleAppdender = new ColoredConsoleAppender();
                consoleAppdender.AddMapping(CreateLevelColors(Level.Warn, ColoredConsoleAppender.Colors.Yellow | ColoredConsoleAppender.Colors.HighIntensity));
                consoleAppdender.AddMapping(CreateLevelColors(Level.Error, ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity));
                consoleAppdender.AddMapping(CreateLevelColors(Level.Debug, ColoredConsoleAppender.Colors.Green | ColoredConsoleAppender.Colors.HighIntensity));
                consoleAppdender.AddMapping(CreateLevelColors(Level.Info, ColoredConsoleAppender.Colors.Cyan | ColoredConsoleAppender.Colors.HighIntensity));

                consoleAppdender.Layout = new PatternLayout("[%level]: %message%newline");
                consoleAppdender.ActivateOptions();
                return consoleAppdender;
            }


            ColoredConsoleAppender.LevelColors CreateLevelColors(Level level, ColoredConsoleAppender.Colors foreColor)
            {
                var levelColors = new ColoredConsoleAppender.LevelColors();
                levelColors.Level = level;
                levelColors.ForeColor = foreColor;


                return levelColors;
            }



            private bool IsConsoleApplication
            {
                get
                {
                    return GetConsoleWindow() != IntPtr.Zero;
                }
            }

            private RollingFileAppender CreateFileAppender()
            {
                var layout = new PatternLayout("[%date][%level][%logger]:  %newline%message%newline%newline");
                layout.ActivateOptions();
                var appender = new RollingFileAppender()
                {
                    File = System.IO.Path.Combine(LogFolder, this.ApplicationName + ".log"),
                    AppendToFile = true,
                    MaximumFileSize = "10MB",
                    MaxSizeRollBackups = 3,
                    Layout = layout

                };
                appender.ActivateOptions();
                return appender;
            }
            

            private string LogFolder
            {
                get { return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), this.ApplicationName, "Log"); }
            }

            private SmtpAppender CreateSmtpAppender()
            {                
                try
                {
                    var mailingList = System.Configuration.ConfigurationManager.AppSettings["MailingList"];
                    if (string.IsNullOrEmpty(mailingList))
                        return null;
                    
                    string currentUserEmailAddress = TryGetCurrentUserEmailAddress();
                    if (string.IsNullOrEmpty(currentUserEmailAddress))
                        return null;

                    var smtpAppender = new SmtpAppender();
                    smtpAppender.Authentication = SmtpAppender.SmtpAuthentication.Ntlm;
                    smtpAppender.To = mailingList;
                    smtpAppender.From = currentUserEmailAddress;
                    smtpAppender.Subject = "Error in " + this.ApplicationName;
#warning I should try to discover the smpt server
                    smtpAppender.SmtpHost = "xch-il.888holdings.corp";
                    smtpAppender.BufferSize = 512;
                    smtpAppender.Lossy = true;
                    smtpAppender.Layout = new PatternLayout("%newline%date [%thread] %-5level %logger [%property{NDC}] - %message%newline%newline%newline");
                    smtpAppender.Evaluator = new log4net.Core.LevelEvaluator(log4net.Core.Level.Warn);
                    return smtpAppender;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(SmptAppenderCreationFailLogFile, ex.ToString());
                    return null;
                }
            }

            
            
            private string SmptAppenderCreationFailLogFile
            {
                get
                {
                    return Path.Combine(LogFolder, "Log4Net-SmtpAppender creation fail exception.txt");
                }
            }

            private string TryGetCurrentUserEmailAddress()
            {
                System.DirectoryServices.AccountManagement.UserPrincipal userPrincipal = null;
                DateTime startTime = DateTime.Now;
                //this is a workaround because sometimes System.DirectoryServices.AccountManagement.UserPrincipal.Current fails with a "Could not load file or assembly 'GGPGameServer.ApprovalSystem.Common.XmlSerializers'"
                //because of some JIT synchronizations which I realy don't understand
                do
                {
                    try
                    {
                        userPrincipal = System.DirectoryServices.AccountManagement.UserPrincipal.Current;
                    }
                    catch(Exception ex)
                    {
                        File.WriteAllText(SmptAppenderCreationFailLogFile, ex.ToString());
                        System.Threading.Thread.Sleep(1000);

                    }

                }
                while (userPrincipal == null && DateTime.Now.Subtract(startTime).Seconds <= 5);

                if (userPrincipal == null)
                {
                    throw new ApplicationException("Unable to get current user e-mail address!");
                }
                else
                {
                    TryDeleteSmptAppenderCreationFailLogFile();
                }
                

                return userPrincipal.EmailAddress;
            }

            private void TryDeleteSmptAppenderCreationFailLogFile()
            {
                if (File.Exists(SmptAppenderCreationFailLogFile))
                {
                    try
                    {
                        File.Delete(SmptAppenderCreationFailLogFile);
                    }
                    catch
                    {
                    }
                }
            }
        }

        


    }
}
