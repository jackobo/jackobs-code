using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class GameEntity
    {
        public GameEntity()
        {

        }

        public GameEntity(Guid id, string name, int mainGameType, bool isExternal, GameInfrastructureDto[] supportedInfrastructures, GameTypeEntity[] gameTypes)
        {
            this.Id = id;
            this.Name = name;
            this.MainGameType = mainGameType;
            this.IsExternal = isExternal;
            this.SupportedInfrastructures = supportedInfrastructures;
            this.GameTypes = gameTypes;
        }

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int MainGameType { get; set; }
        [DataMember]
        public bool IsExternal { get; set; }
        [DataMember]
        public GameInfrastructureDto[] SupportedInfrastructures { get; set; }
        [DataMember]
        public GameTypeEntity[] GameTypes { get; set; }

    }
}
