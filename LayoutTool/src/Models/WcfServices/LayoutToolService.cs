using System;
using LayoutTool.Models.LayoutToolService;

namespace LayoutTool.Models
{
    public interface IDisposableLayoutToolService : ILayoutToolService, IDisposable
    {

    }

    internal class LayoutToolServiceClientWrapper : WcfServiceWrapperBase<LayoutToolServiceClient>, IDisposableLayoutToolService
    {

        public DownloadFileContentResponse DownloadFileContent(DownloadFileContentRequest request)
        {
            return Proxy.DownloadFileContent(request);
        }

        public GetGamesInfoResponse GetGamesInfo(GetGamesInfoRequest request)
        {
            return Proxy.GetGamesInfo(request);
        }

        public GetCountriesResponse GetCountries()
        {
            return Proxy.GetCountries();
        }

        public GetCurrenciesResponse GetCurrencies()
        {
            return Proxy.GetCurrencies();
        }

        public GetAllJackpotIdsResponse GetAllJackpotIds()
        {
            return Proxy.GetAllJackpotIds();
        }
        
        public ReadLayoutFromTfsResponse ReadLayoutFromTfs(ReadLayoutFromTfsRequest request)
        {
            return Proxy.ReadLayoutFromTfs(request);
        }
    }


}
