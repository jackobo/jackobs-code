using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public interface ICountryInformationProvider
    {
        Country[] GetCountries();
    }

    public sealed class Country
    {
      
        public Country(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public int Id { get; private set; }
        public string Name { get; private set; }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as Country;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
