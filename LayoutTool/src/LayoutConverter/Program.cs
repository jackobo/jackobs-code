using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutConverter;
using LayoutTool.Interfaces;
using LayoutTool.Models;
using LayoutTool.Models.Builders;
using LayoutTool.Models.Builders.Xml;
using Spark.Infra.Exceptions;
using Spark.Infra.Logging;
using Spark.Infra.Windows;

namespace LayoutConverter
{
    class Program
    {
        private static ILoggerFactory LoggerFactory { get; set; }

        private static bool IsDevMachine
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["IsDevMachine"]?.ToLowerInvariant() == "true";
            }
        }

        private static ILogger _logger;
        private static ILogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = LoggerFactory.CreateLogger(typeof(Program));

                return _logger;
            }
        }
        private static IOperatingSystemServices OperatingSystemServices { get; set; }

        private static IFileSystemManager FileSystem
        {
            get { return OperatingSystemServices.FileSystem; }
        }
        static void Main(string[] args)
        {
            LoggerFactory = Log4NetNotifierFactory.FromCurrentUserAppData("NdlLayoutConverter");

            try
            {
                OperatingSystemServices = new OperatingSystemServices(LoggerFactory);

                /*args = new string[]
                {
                @"C:\CasinoTools\NDLLayoutAdmin\Head\src\LayoutConverter\OriginalFiles\LayoutToolOutputFile.lyt",
                @"C:\CasinoTools\NDLLayoutAdmin\Head\src\LayoutConverter\OriginalFiles\navigation_plan_ndl.xmm",
                @"C:\CasinoTools\NDLLayoutAdmin\Head\src\LayoutConverter\OriginalFiles\skin.xml"
                };
                */

                var commandLineArgs = ParseCommandLineArgs(args);
                var inputFiles = ReadInputFiles(commandLineArgs);

                Convert(inputFiles, commandLineArgs);

                Logger.Info("DONE!");

            }
            catch(MissingCommandLineArgumentsException)
            {
                Logger.Info(CommandLineParser.UsageInfo);
                Environment.Exit(1);
            }
            catch (InvalidCommandLineArgumentsException ex)
            {
                Logger.Error(ex.Message);
                Logger.Info(CommandLineParser.UsageInfo);
                Environment.Exit(1);
            }
            catch(ValidationException ex)
            {
                Logger.Error(ex.Message);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                Environment.Exit(1);
            }
        }

        private static void Convert(ReadInputFilesResult inputFiles, CommandLineArgs commandLineArgs)
        {
         
            if (inputFiles.SkinDefinitionContext.Errors.Any(err => err.Severity == LayoutTool.Interfaces.Entities.ErrorServerity.Error))
            {
                throw new ValidationException($"There are some errors inside '{inputFiles.SkinDefinitionFile}' file.{Environment.NewLine}You should take a look inside that file or open it with the Layout Admin Tool.");
            }


            if (inputFiles.SkinDefinitionContext.Errors.Any(err => err.Severity == LayoutTool.Interfaces.Entities.ErrorServerity.Warning))
            {
                Logger.Warning($"Atention! There are some warnings inside '{inputFiles.SkinDefinitionFile}' file.{Environment.NewLine}You should take a look inside that file or open it with the Layout Admin Tool.");
            }


            var converter = CreateConverter(inputFiles);
            var conversionResult = converter.Convert(inputFiles.SkinDefinitionContext.SkinDefinition);

            string conversionResultFolder = GetConvertionResultFolder(commandLineArgs);

            Logger.Info($"Writing conversion result to: {conversionResultFolder}");

            foreach (var file in conversionResult.Files)
            {
                FileSystem.WriteAllText(Path.Combine(conversionResultFolder, file.OriginalFile.FileName), file.NewContent);
            }
        }

        private static string GetConvertionResultFolder(CommandLineArgs commandLineArgs)
        {
            string conversionResultFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConversionResult");
            if (commandLineArgs.OutputFolder.Any())
            {
                conversionResultFolder = commandLineArgs.OutputFolder.First();
            }
            
            if (FileSystem.FolderExists(conversionResultFolder))
                FileSystem.DeleteFolder(conversionResultFolder);

            FileSystem.CreateFolder(conversionResultFolder);
            return conversionResultFolder;
        }

        private static XmlSkinDefinitionConverter CreateConverter(ReadInputFilesResult inputFiles)
        {
            XmlSkinDefinitionConverter converter = new XmlSkinDefinitionConverter();

            converter.NavigationPlan = inputFiles.NavigationPlan;
            return converter;
        }

        private static ReadInputFilesResult ReadInputFiles(CommandLineArgs commandLineArgs)
        {
            var filesReader = new InputFilesReader(FileSystem, 
                                                   new WcfServiceFactory(), 
                                                   GetConfigurationFilesStorage(), 
                                                   LoggerFactory);
            var inputFiles = filesReader.ReadFiles(commandLineArgs);
            return inputFiles;
        }


        static IConfigurationFilesStorage GetConfigurationFilesStorage()
        {
            if (IsDevMachine)
                return new HardCodedConfigurationFilesStorage(FileSystem);
            else
                return new ArtifactoryConfigurationFilesStorage();

        }


        private static CommandLineArgs ParseCommandLineArgs(string[] args)
        {
            var commandLineParser = new CommandLineParser();
            var commandLineArgs = commandLineParser.Parse(args);
            return commandLineArgs;
        }
    }
}
