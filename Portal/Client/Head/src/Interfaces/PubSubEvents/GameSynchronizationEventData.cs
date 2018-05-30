using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;


namespace GamesPortal.Client.Interfaces.PubSubEvents
{
    public class GameSynchronizationFinishedEventData
    {
        public GameSynchronizationFinishedEventData(Guid gameId, Game newGame, Interfaces.ChangeType changeType, bool isExternal)
        {
            this.GameId = gameId;
            this.NewGame = newGame;
            this.ChangeType = changeType;
            this.IsExternal = isExternal;
            
        }

        public Guid GameId { get; set; }
        public Game NewGame { get; private set; }
        public Interfaces.ChangeType ChangeType { get; private set; }
        public bool IsExternal { get; private set; }
    }


  
}
