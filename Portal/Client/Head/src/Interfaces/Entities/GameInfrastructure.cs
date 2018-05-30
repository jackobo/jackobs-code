using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Client.Interfaces.Entities
{
    public enum GameTechnology
    {
        Flash,
        Html5
    }


    public enum PlatformType
    {
        PC = 1,
        Mobile = 2,
        Both = 3
    }

    
    public enum GamingComponentCategory
    {
        Wrapper = 0,
        Chill = 1,
        Game = 2
    }

    public static class PlatformTypeExtenstion
    {
        public static string ToDescription(this PlatformType platformType)
        {
            if (platformType == PlatformType.Both)
                return $"{PlatformType.PC} & {PlatformType.Mobile}";

            return platformType.ToString();
        }
    }

    public class GameInfrastructure : IComparable<GameInfrastructure>, IComparable
    {
        public GameInfrastructure()
        {

        }

        public GameInfrastructure(int gameTechnology, int platformType)
            : this((GameTechnology)gameTechnology, (PlatformType)platformType)
        {

        }

        public GameInfrastructure(GameTechnology gameTechnology, PlatformType platformType)
        {
            GameTechnology = gameTechnology;
            PlatformType = platformType;
        }


        
        public GameTechnology GameTechnology { get; private set; }

        
        public PlatformType PlatformType { get; private set; }

      

        public override bool Equals(object obj)
        {
            var theOther = obj as GameInfrastructure;
            if (theOther == null)
                return false;

            return this.GameTechnology == theOther.GameTechnology && this.PlatformType == theOther.PlatformType;
        }

        public override int GetHashCode()
        {
            return this.GameTechnology.GetHashCode() ^ this.PlatformType.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GameTechnology} {PlatformType.ToDescription()}";   
            
        }

        public int CompareTo(GameInfrastructure other)
        {
            if (other == null)
                return 1;

            var rez = this.GameTechnology.CompareTo(other.GameTechnology);
            if (rez != 0)
                return rez;

            return this.PlatformType.CompareTo(other.PlatformType);
        }

        int IComparable.CompareTo(object obj)
        {
            return CompareTo(obj as GameInfrastructure);
        }

        public static bool operator ==(GameInfrastructure i1, GameInfrastructure i2)
        {
            if (!object.ReferenceEquals(i1, null))
                return i1.Equals(i2);
            else if (!object.ReferenceEquals(i2, null))
                return i2.Equals(i1);
            return true;

        }

        public static bool operator !=(GameInfrastructure i1, GameInfrastructure i2)
        {
            return !(i1 == i2);
        }
    }
}
