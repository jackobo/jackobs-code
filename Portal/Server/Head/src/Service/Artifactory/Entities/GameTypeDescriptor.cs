using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service.Artifactory
{
    public class GameTypeDescriptor
    {
        public GameTypeDescriptor(int gameType, OperatorEnum operatorId)
            : this(gameType, operatorId, null)
        {
        }

        public GameTypeDescriptor(int gameType, OperatorEnum operatorId, string gameName)
        {
            this.GameType = gameType;
            this.OperatorId = operatorId;
            this.GameName = gameName;
        }

        public int GameType { get; set; }
        public OperatorEnum OperatorId { get; set; }
        public string GameName { get; set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as GameTypeDescriptor;

            if (theOther == null)
                return false;

            return this.GameType == theOther.GameType
                && this.OperatorId == theOther.OperatorId;
        }

        public override int GetHashCode()
        {
            return this.GameType.GetHashCode() ^ this.OperatorId.GetHashCode();
        }
    }
}
