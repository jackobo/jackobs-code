using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IGameOutputFile : IOutputFile
    {
        ILocalPath SourceFile { get; }
    }

    public abstract class GameOutputFile : IGameOutputFile
    {
        public GameOutputFile(string gameName, Folders.GameEngineFolder engineFolder, ILocalPath sourceFile) 
        {
            GameName = gameName;
            EngineName = engineFolder.Name;
            SourceFile = sourceFile;
        }

        protected string GameName { get; private set; }
        protected string EngineName { get; private set; }
        public ILocalPath SourceFile { get; private set; }


        protected string FileName
        {
            get
            {
                return SourceFile.GetName();
            }
        }

        
        public ILocalPath ResolveBuildOutputPath(ILocalPath basePath)
        {
            return basePath.Subpath(Folders.GamesFolder.WellKnownName)
                            .Subpath(GameName)
                            .Subpath(FileName);
        }
        
        public ILocalPath ResolveDistributionPath(ILocalPath basePath)
        {
            return basePath.Subpath(this.FileName);
        }

        public DeployableFileDefinition GetDeployableFileDefinition()
        {
            return new DeployableFileDefinition(this.FileName, DeployEnvironment.All);
        }
    }
}
