using System;
using System.Collections.Generic;
using System.Linq;
using LayoutTool.Interfaces;

namespace LayoutTool.Models
{
    public class GamesInformationProvider : IGamesInformationProvider
    {
        public GamesInformationProvider(IWcfServiceFactory wcfServiceFactory)
        {
            _wcfServiceFactory = wcfServiceFactory;
        }


        IWcfServiceFactory _wcfServiceFactory;

        public GameInfo[] GetGamesInfo(int brandId)
        {
            using (var proxy = _wcfServiceFactory.CreateLayoutToolService())
            {
                return proxy.GetGamesInfo(new LayoutToolService.GetGamesInfoRequest() { BrandId = brandId })
                            .Games
                            .Select(g => new GameInfo(g.GameType, g.Name, g.GameGroup, g.IsApproved, g.GameVendor, g.JackpotIds))
                            .ToArray();
            }
        }
        
    }
}
