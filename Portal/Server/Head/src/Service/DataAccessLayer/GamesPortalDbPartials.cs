using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.DataAccessLayer
{

    public partial class GameType
    {
        public GameType(int gameTypeId, string name, Guid gameId, int? operatorId)
        {
            this.GameType_ID = gameTypeId;
            this.Name = name;
            this.Game_ID = gameId;
            this.Operator_ID = operatorId;
            this.Row_ID = Guid.NewGuid();
        }
    }

    public partial class Game : IComponentRecord<GameVersion>
    {
        #region IComponent<GameVersion> Members

        EntitySet<GameVersion> IComponentRecord<GameVersion>.Versions
        {
            get
            {
                return this.GameVersions;
            }
        }

        int IComponentRecord<GameVersion>.ComponentType
        {
            get { return MainGameType; }
        }


        #endregion
    }

    public partial class GameVersion : IComponentVersionRecord<GameVersion_Property, GameVersion_Language, GameVersion_Regulation, GameVersion_Property_History>
    {

        EntitySet<GameVersion_Language> IComponentVersionRecord<GameVersion_Property, GameVersion_Language, GameVersion_Regulation, GameVersion_Property_History>.ComponentVersionLanguages
        {
            get { return this.GameVersion_Languages; }
        }

        Guid IComponentVersionRecord<GameVersion_Property, GameVersion_Language, GameVersion_Regulation, GameVersion_Property_History>.Component_ID
        {
            get
            {
                return this.Game_ID;
            }
            set
            {
                this.Game_ID = value;
            }
        }

        EntitySet<GameVersion_Regulation> IComponentVersionRecord<GameVersion_Property, GameVersion_Language, GameVersion_Regulation, GameVersion_Property_History>.ComponentVersionRegulation
        {
            get
            {
                return this.GameVersion_Regulations;
            }
        }

        Guid IComponentVersionRecord<GameVersion_Property, GameVersion_Language, GameVersion_Regulation, GameVersion_Property_History>.ComponentVersion_ID
        {
            get
            {
                return this.GameVersion_ID;
            }
            set
            {
                this.GameVersion_ID = value;
            }
        }

        EntitySet<GameVersion_Property> IComponentVersionRecord<GameVersion_Property, GameVersion_Language, GameVersion_Regulation, GameVersion_Property_History>.ComponentVersionProperties
        {
            get
            {
                return this.GameVersion_Properties;
            }
           
        }

        EntitySet<GameVersion_Property_History> IComponentVersionRecord<GameVersion_Property, GameVersion_Language, GameVersion_Regulation, GameVersion_Property_History>.ComponentVersionPropertyHistories
        {
            get
            {
                return this.GameVersion_Property_Histories;
            }
        }

    }


    public partial class GameVersion_Regulation : IComponentVersionRegulationRecord
    {
        
        
        Guid IComponentVersionRegulationRecord.ComponentVersion_ID
        {
            get
            {
                return this.GameVersion_ID;
            }
            set
            {
                this.GameVersion_ID = value;
            }
        }

        Guid IComponentVersionRegulationRecord.Row_ID
        {
            get { return this.GameVersionRegulation_ID; }
            set { this.GameVersionRegulation_ID = value; }
        }
        
    }


    public partial class GameVersion_Property : IComponentVersionPropertyRecord
    {

        #region IComponentVersionProperty Members


        Guid IComponentVersionPropertyRecord.ComponentVersion_ID
        {
            get
            {
                return this.GameVersion_ID;
            }
            set
            {
                this.GameVersion_ID = value;
            }
        }

        #endregion
    }

        

}
