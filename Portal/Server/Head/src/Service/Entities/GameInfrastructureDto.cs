using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Types;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public enum GamingComponentCategory
    {
        [EnumMember]
        Wrapper = 0,
        [EnumMember]
        Chill = 1,
        [EnumMember]
        Game = 2
    }

    public class ChillWrapperType : SmartEnum<GamingComponentCategory, ChillWrapperType>
    {
        public ChillWrapperType(GamingComponentCategory id, string name, GameInfrastructureDTO infrastructure)
            : base(id, name)
        {
            this.Infrastructure = infrastructure;
        }

        public static readonly ChillWrapperType Chill = new ChillWrapperType(GamingComponentCategory.Chill, "Chill", new GameInfrastructureDTO(GameTechnology.Html5, PlatformType.PcAndMobile));
        public static readonly ChillWrapperType Wrapper = new ChillWrapperType(GamingComponentCategory.Wrapper, "Wrapper", new GameInfrastructureDTO(GameTechnology.Flash, PlatformType.PC));


        public  GameInfrastructureDTO Infrastructure { get; private set; }
        

       
        public static ChillWrapperType FindFromId(int id)
        {
            var result = TryFindFromId(id);
            if (result.Any())
                return result.First();


            throw new ArgumentException($"The value {id} is not a chill or wrapper component type");
        }

        public static Optional<ChillWrapperType> TryFindFromId(int id)
        {
            var result = All.FirstOrDefault(item => (int)item.Id == id);

            if (result == null)
                return Optional<ChillWrapperType>.None();
            else
                return Optional<ChillWrapperType>.Some(result);
        }
    }

    [DataContract]
    public enum PlatformType
    {
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        PC = 1,
        [EnumMember]
        Mobile = 2,
        [EnumMember]
        PcAndMobile = 3
    }


    [DataContract]
    public enum GameTechnology
    {
        [EnumMember]
        Flash = 0,
        [EnumMember]
        Html5 = 1
    }

    [DataContract]
    public sealed class GameInfrastructureDTO
    {
        
        public GameInfrastructureDTO()
        {

        }
        

        public GameInfrastructureDTO(int gameTechnology, int platformType)
            
        {
            this.GameTechnology = (GameTechnology)gameTechnology;
            this.PlatformType = (PlatformType)platformType;
        }

        public GameInfrastructureDTO(GameTechnology gameTechnology, PlatformType platformType)
        {
            GameTechnology = gameTechnology;
            PlatformType = platformType;
        }


        [DataMember]
        public GameTechnology GameTechnology { get; set; }

        [DataMember]
        public PlatformType PlatformType { get; set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as GameInfrastructureDTO;
            if (theOther == null)
                return false;

            return this.GameTechnology == theOther.GameTechnology && this.PlatformType == theOther.PlatformType;
        }


        public static bool operator ==(GameInfrastructureDTO i1, GameInfrastructureDTO i2)
        {
            if (!object.ReferenceEquals(i1, null))
                return i1.Equals(i2);
            else if (!object.ReferenceEquals(i2, null))
                return i2.Equals(i1);
            return true;
        }

        public static bool operator !=(GameInfrastructureDTO i1, GameInfrastructureDTO i2)
        {
            return !(i1 == i2);
        }


        public override int GetHashCode()
        {
            return this.GameTechnology.GetHashCode() ^ this.PlatformType.GetHashCode();
        }

        public override string ToString()
        {
            return GameTechnology + " " + PlatformType;
        }
    }
}
