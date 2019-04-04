using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class ClientTypeEntity
    {
        public ClientTypeEntity()
        {

        }

        public ClientTypeEntity(string name, params RegulationEntity[] supportedRegulations)
        {
            this.Name = name;
            this.SupportedRegulations = supportedRegulations;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public RegulationEntity[] SupportedRegulations { get; set; }


        public override bool Equals(object obj)
        {
            var theOther = obj as ClientTypeEntity;

            if (theOther == null)
                return false;

            return string.Compare(this.Name, theOther.Name, true) == 0;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
        

    }

    [DataContract]
    public class RegulationEntity
    {
        public RegulationEntity()
        {

        }

        public RegulationEntity(string name)
        {
            this.Name = name;
        }

        [DataMember]
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as RegulationEntity;

            if (theOther == null)
                return false;

            return string.Compare(this.Name, theOther.Name, true) == 0;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    
}
