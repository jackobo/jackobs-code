using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GGPGameServer.ApprovalSystem.Common;

namespace GamesPortal.Client.Interfaces.Entities
{
    public class GameVersionRelease
    {
        public GameVersionRelease(Guid gameId,
                                    int mainGameType, 
                                    string name, 
                                    bool isExternal, 
                                    GameInfrastructure gameInfrastructure, 
                                    Guid gameVersionId,
                                    VersionNumber version, 
                                    string regulations,
                                    DateTime createdDate,
                                    string createdBy,
                                    string triggeredBy)
        {
            this.GameId = gameId;
            this.MainGameType = mainGameType;
            this.Name = name;
            this.IsExternal = isExternal;
            this.GameInfrastructure = gameInfrastructure;
            this.GameVersionId = gameVersionId;
            this.Version = version;
            this.Regulations = regulations;

            this.CreatedDate = createdDate;
            this.CreatedBy = createdBy;
            this.TriggeredBy = triggeredBy;
        }

        [Browsable(false)]
        public Guid GameId { get; private set; }
        public int MainGameType { get; private set; }
        public string Name { get; private set; }
        public bool IsExternal { get; private set; }
        public GameInfrastructure GameInfrastructure { get; private set; }

        [Browsable(false)]
        public Guid GameVersionId { get; private set; }
        public VersionNumber Version { get; private set; }

        public string Regulations { get; private set; }

        public DateTime CreatedDate { get; set; }
        
        public string CreatedBy { get; set; }
        
        public string TriggeredBy { get; set; }
    }
}
