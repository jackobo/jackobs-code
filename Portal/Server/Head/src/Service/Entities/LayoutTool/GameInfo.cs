using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.Entities.LayoutTool
{
    [DataContract]
    public class GameInfo
    {
        public GameInfo()
        {

        }

        public GameInfo(int gameType, string name, string gameGroup, bool? isApproved, string gameVendor, int[] jackpotIds)
        {
            GameType = gameType;
            Name = name;
            GameGroup = gameGroup;
            IsApproved = isApproved;
            GameVendor = gameVendor;
            JackpotIds = jackpotIds;
        }

        [DataMember]
        public int GameType { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string GameGroup { get; set; }

        [DataMember]
        public bool? IsApproved { get; set; }

        [DataMember]
        public string GameVendor { get; set; }

        [DataMember]
        public int[] JackpotIds { get; set; }

    }

   
}
