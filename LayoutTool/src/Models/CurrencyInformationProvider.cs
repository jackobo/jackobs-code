using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models
{
    public class CurrencyInformationProvider : Interfaces.ICurrencyInformationProvider
    {
        public CurrencyInformationProvider(IWcfServiceFactory wcfServiceFactory)
        {
            _wcfServiceFactory = wcfServiceFactory;
        }

        IWcfServiceFactory _wcfServiceFactory;
        public CurrencyInfo[] GetCurrencies()
        {
            using (var proxy = _wcfServiceFactory.CreateLayoutToolService())
            {
                return proxy.GetCurrencies().Currencies.Select(c => new CurrencyInfo(c.Iso3, c.Name)).ToArray();
            }
        }
    }
}
