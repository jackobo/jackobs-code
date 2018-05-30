using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using LayoutTool.Models;
using LayoutTool.Models.Builders;
using Spark.Infra.Logging;
using Spark.Infra.Types;
using Spark.Infra.Windows;

namespace LayoutConverter
{
    public class InputFilesReader
    {
        public InputFilesReader(IFileSystemManager fileSystem, IWcfServiceFactory wcfServiceFactory, IConfigurationFilesStorage configurationFilesStorage, ILoggerFactory loggerFactory)
        {
            _fileSystem = fileSystem;
            _wcfServiceFactory = wcfServiceFactory;
            _configurationFilesStorage = configurationFilesStorage;
            Logger = loggerFactory.CreateLogger(this.GetType());
        }

        ILogger Logger { get; set; }
        IFileSystemManager _fileSystem;
        IConfigurationFilesStorage _configurationFilesStorage;
        IWcfServiceFactory _wcfServiceFactory;

        public ReadInputFilesResult ReadFiles(CommandLineArgs args)
        {
            var skinDefinitionContext = ReadSkinDefinitionContext(args.LayoutFile);

            ValidateArgs(skinDefinitionContext.DestinationSkin.Selector.Id, args);

            var navigationPlan = ReadInputFileFromDisk(args.NavigationPlanFile);
            

            if (!navigationPlan.Any())
                navigationPlan = Optional<InputFile>.Some(_configurationFilesStorage.GetNavigationPlanFile(skinDefinitionContext.DestinationSkin));

            
            return new ReadInputFilesResult(args.LayoutFile, skinDefinitionContext, navigationPlan.First());
        }
        

        private Optional<InputFile> ReadInputFileFromDisk(Optional<string> optionalFile)
        {
            var result = Optional<InputFile>.None();
            optionalFile.Do(file =>
            {
                if (!_fileSystem.FileExists(file))
                {
                    throw new InvalidCommandLineArgumentsException($"Can't find file {file}");
                }

                result = Optional<InputFile>.Some(new InputFile(Path.GetFileName(file), 
                                                               _fileSystem.ReadAllText(file), 
                                                               new PathDescriptor(file)));
            });

            return result;
        }

        private void ValidateArgs(Guid skinSourceId, CommandLineArgs args)
        {
            if (skinSourceId != WellKnownSkinSourcesIds.Production)
            {
                if (!args.NavigationPlanFile.Any())
                {
                    throw new InvalidCommandLineArgumentsException("You can skip providing the navigation plan file file only when the target layout is a production one!");
                }
            }
        }

        private SkinDefinitionContext ReadSkinDefinitionContext(string layoutFile)
        {
            if (IsTfsPath(layoutFile))
            {
                Logger.Info($"Reading layout file from TFS: {layoutFile}");
                return new SkinDefinitionSerializer().Deserialize(ReadFromTfs(layoutFile));
            }

            Logger.Info($"Reading layout file from disk: {layoutFile}");
            return new SkinDefinitionSerializer().Deserialize(_fileSystem.ReadAllText(layoutFile));
        }

        private string ReadFromTfs(string layoutFile)
        {
            using (var proxy = _wcfServiceFactory.CreateLayoutToolService())
            {
                return proxy.ReadLayoutFromTfs(new LayoutTool.Models.LayoutToolService.ReadLayoutFromTfsRequest() { ServerFilePath = layoutFile })
                            .FileContent;
            }
        }

        private bool IsTfsPath(string layoutFile)
        {
            return layoutFile.StartsWith("$/");
        }
    }


    public class ReadInputFilesResult
    {
        public ReadInputFilesResult(string skinDefinitionFile, SkinDefinitionContext skinDefinitionContext, InputFile navigationPlan)
        {
            this.SkinDefinitionFile = skinDefinitionFile;
            SkinDefinitionContext = skinDefinitionContext;
            NavigationPlan = navigationPlan;
        }

        public string SkinDefinitionFile { get; private set; }
        public SkinDefinitionContext SkinDefinitionContext { get; private set; }
        public InputFile NavigationPlan { get; private set; }
    }
}
