using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class GameEngineContentProvider : CompilableComponentContentProvider<GameEngineFolder>, IGameEngineContentProvider
    {
        public GameEngineContentProvider(GameEngineFolder location, IEnumerable<IVisualStudioProject> visualStudioProjects, IBuildCustomizationProvider buildCustomizationProvider)
            : base(new ComponentUniqueIdBuilder(location.Engine.ComponentUniqueIdTxt), location, visualStudioProjects, buildCustomizationProvider)
        {
            LoadGames(); 
        }


        protected override IEnumerable<BuildOutputFileDefinition> GetCustomizedFiles()
        {
            return this.CustomizationProvider.GetGameEngineCustomizedOutputFiles(this.Name);
        }
        
        protected override IOutputFile CreateOutputFile(BuildOutputFileDefinition fileDefinition)
        {
            return new GameEngineOutputFile(fileDefinition);
        }

        private void LoadGames()
        {
            var games = new List<IGameContentProvider>();

            foreach (var gameFolder in this.Location.Games.AllGames)
            {
                games.Add(new GameContentProvider(gameFolder, this.ComponentUniqueIdBuilder));
            }

            this.Games = games;
        }

        public IGameContentProvider GetGame(string gameName)
        {
            var game = this.Games.FirstOrDefault(g => string.Compare(g.Name, gameName, true) == 0);
            if (game == null)
                throw new ArgumentException($"Can't find game {gameName} for engine {this.Name}");

            return game;
        }

        

        IEnumerable<IGameContentProvider> Games { get; set; } = new IGameContentProvider[0];

        public override IServerPath GetProjectPath()
        {
            return this.Location.Engine.GetServerPath();
        }

        public IEnumerable<IComponentUniqueIdBuilder> GetUniqueIdBuilders()
        {
            var uniqueIdBuilers = new List<IComponentUniqueIdBuilder>();
            uniqueIdBuilers.Add(this.ComponentUniqueIdBuilder);
            uniqueIdBuilers.AddRange(this.Games.Select(g => g.LimitsContent.ComponentUniqueIdBuilder));
            uniqueIdBuilers.AddRange(this.Games.Select(g => g.MathContent.ComponentUniqueIdBuilder));
            return uniqueIdBuilers;
        }
    }
}
