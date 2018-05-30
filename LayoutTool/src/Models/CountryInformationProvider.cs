using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models
{
    internal class CountryInformationProvider : Interfaces.ICountryInformationProvider
    {
        public CountryInformationProvider(IWcfServiceFactory wcfServiceFactory)
        {
            _wcfServiceFactory = wcfServiceFactory;
        }

        IWcfServiceFactory _wcfServiceFactory;

        public Country[] GetCountries()
        {
            using (var proxy = _wcfServiceFactory.CreateLayoutToolService())
            {
                return proxy.GetCountries().Countries.Select(c => new Country(c.Id, c.Name)).ToArray();
            }
        }
    }
}
