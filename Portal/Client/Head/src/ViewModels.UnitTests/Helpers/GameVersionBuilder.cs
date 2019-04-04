using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using Spark.Infra.Types;

namespace GamesPortal.Client.ViewModels.Helpers
{
    public class GameVersionBuilder
    {
        private VersionNumber _version = new VersionNumber("1.0.0.0");
        private GameInfrastructure _infrastructure = new GameInfrastructure(GameTechnology.Html5, PlatformType.Both);
        private List<GameVersionRegulation> _regulations = new List<GameVersionRegulation>();
        private string _triggeredBy = "florin";
        private string _createdBy = "popescu";
        private DateTime _createdDate = DateTime.Now;
        private Guid _id = Guid.NewGuid();


        private GameVersionBuilder()
        {
        }
        
        public static GameVersionBuilder GameVersion()
        {
            return new GameVersionBuilder();
        }

        public GameVersionBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }
        public GameVersionBuilder WithInfrastructure(GameInfrastructure infrastructure)
        {
            _infrastructure = infrastructure;
            return this;
        }


        public GameVersionBuilder WithRegulation(GameVersionRegulation regulation)
        {
            _regulations.Add(regulation);
            return this;
        }
        public GameVersionBuilder TriggeredBy(string triggeredBy)
        {
            _triggeredBy = triggeredBy;
            return this;
        }

        public GameVersionBuilder CreatedBy(string createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public GameVersionBuilder CreatedOn(DateTime createdDate)
        {
            _createdDate = createdDate;
            return this;
        }

        public GameVersion Build()
        {
            var gameVersion = new GameVersion();
            gameVersion.Category = GamingComponentCategory.Game;
            gameVersion.Id = _id;
            gameVersion.Version = _version;
            gameVersion.Infrastructure = _infrastructure;
            gameVersion.Regulations = _regulations.ToArray();
            gameVersion.TriggeredBy = _triggeredBy;
            gameVersion.CreatedBy = _createdBy;
            gameVersion.CreatedDate = _createdDate;
            
            return gameVersion;
        }
    }
}
