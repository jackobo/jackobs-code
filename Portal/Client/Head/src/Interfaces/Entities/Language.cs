using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Client.Interfaces.Entities
{
    public sealed class Language
    {
        public Language(string name, string iso2, string iso3)
        {
            this.Name = name;
            this.Iso2 = iso2;
            this.Iso3 = iso3;
        }

        
        public string Iso2 { get; }
        
        public string Iso3 { get; }
        
        public string Name { get; }

        public static bool operator ==(Language l1, Language l2)
        {
            if (!object.ReferenceEquals(l1, null))
                return l1.Equals(l2);
            else if (!object.ReferenceEquals(l2, null))
                return l2.Equals(l1);
            return true;

        }

        public static bool operator !=(Language l1, Language l2)
        {

            return !(l1 == l2);
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as Language;

            if (theOther == null)
                return false;

            return this.Name == theOther.Name
                   && this.Iso2 == theOther.Iso2
                   && this.Iso3 == theOther.Iso3;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Iso2.GetHashCode() ^ this.Iso3.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
