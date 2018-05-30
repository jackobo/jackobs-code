using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Client.Models.GamesPortalService
{
   

    public partial class GameInfrastructureDTO
    {
        // override object.Equals
        public override bool Equals(object obj)
        {
            var theOther = obj as GameInfrastructureDTO;
            if (theOther == null)
                return false;

            return this.GameTechnology == theOther.GameTechnology
                && this.PlatformType == theOther.PlatformType;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.GameTechnology.GetHashCode() ^ this.PlatformType.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.GameTechnology.ToString()} - {this.PlatformType.ToString()}";
        }
    }
}
