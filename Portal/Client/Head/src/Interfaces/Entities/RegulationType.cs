using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Client.Interfaces.Entities
{
    public class RegulationType : IComparable<RegulationType>, IComparable
    {
        public RegulationType(string name)
        {
            this.Name = name;
        }


        public string Name{get; private set;}

        private static ConcurrentDictionary<string, RegulationType> _regulationsDictionary = new ConcurrentDictionary<string,RegulationType>();


        public static RegulationType GetRegulation(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;


            if (!_regulationsDictionary.ContainsKey(name))
                _regulationsDictionary.TryAdd(name, new RegulationType(name));
            
            return _regulationsDictionary[name];
        }


        


        public static RegulationType[] AllRegulations
        {
            get { return _regulationsDictionary.Values.ToArray(); }
        }


        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as RegulationType;

            if (theOther == null)
                return false;

            return this.Name == theOther.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        public static bool operator ==(RegulationType r1, RegulationType r2)
        {
            if (!object.ReferenceEquals(r1, null))
                return r1.Equals(r2);
            else if (!object.ReferenceEquals(r2, null))
                return r2.Equals(r1);
            return true;

        }

        public static bool operator !=(RegulationType r1, RegulationType r2)
        {
            return !(r1 == r2);
        }


        #region IComparable<RegulationType> Members

        public int CompareTo(RegulationType other)
        {
            if (other == null)
                return 1;

            return this.Name.CompareTo(other.Name);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return this.CompareTo(obj as RegulationType);
        }

        

        #endregion
    }
}
