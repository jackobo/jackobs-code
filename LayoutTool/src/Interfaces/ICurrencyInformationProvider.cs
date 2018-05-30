using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public interface ICurrencyInformationProvider
    {
        CurrencyInfo[] GetCurrencies();
    }

    public sealed class CurrencyInfo
    {
        public CurrencyInfo(string iso3, string name)
        {
            Iso3 = iso3;
            Name = name;
        }
        public string Iso3 { get; private set; }
        public string Name { get; private set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as CurrencyInfo;
            if (theOther == null)
                return false;

            return 0 == string.Compare(this.Iso3, theOther.Iso3, true);
        }

        public override string ToString()
        {
            return this.Iso3.ToUpper();
        }

        public override int GetHashCode()
        {
            return this.Iso3.ToUpper().GetHashCode();
        }
    }
}
