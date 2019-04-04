using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace GamesPortal.Service
{
    using GamesPortal.Service.LayoutTool;

    [ServiceContract]
    public interface ILayoutToolService
    {

        [OperationContract]
        DownloadFileContentResponse DownloadFileContent(DownloadFileContentRequest request);

        [OperationContract]
        GetGamesInfoResponse GetGamesInfo(GetGamesInfoRequest request);

        [OperationContract]
        GetCountriesResponse GetCountries();

        [OperationContract]
        GetCurrenciesResponse GetCurrencies();

        [OperationContract]
        GetAllJackpotIdsResponse GetAllJackpotIds();

        [OperationContract]
        ReadLayoutFromTfsResponse ReadLayoutFromTfs(ReadLayoutFromTfsRequest request);
        
    }
}

namespace GamesPortal.Service.LayoutTool
{
   
    [DataContract]
    public class ReadLayoutFromTfsResponse
    {
        public ReadLayoutFromTfsResponse()
        {
        }
        public ReadLayoutFromTfsResponse(string fileContent)
        {
            this.FileContent = fileContent;
        }
                
        [DataMember]
        public string FileContent { get; set; }
    }

    [DataContract]
    public class ReadLayoutFromTfsRequest
    {
        [DataMember]
        public string ServerFilePath { get; set; }
    }
    
    [DataContract]
    public class GetAllJackpotIdsResponse
    {
        public GetAllJackpotIdsResponse()
        {

        }

        public GetAllJackpotIdsResponse(int[] jackpotIds)
        {
            this.JackpotIds = jackpotIds;
        }

        [DataMember]
        public int[] JackpotIds { get; set; }
    }


    [DataContract]
    public class GetCurrenciesResponse
    {
        public GetCurrenciesResponse()
        {

        }

        public GetCurrenciesResponse(Entities.CurrencyDto[] currencies)
        {
            this.Currencies = currencies;
        }

        [DataMember]
        public Entities.CurrencyDto[] Currencies { get; set; }
    }

    [DataContract]
    public class GetCountriesResponse
    {
        public GetCountriesResponse()
        {

        }

        public GetCountriesResponse(Entities.CountryDto[] countries)
        {
            this.Countries = countries;
        }
        [DataMember]
        public Entities.CountryDto[] Countries { get; set; }
    }

    [DataContract]
    public class DownloadFileContentRequest
    {
        [DataMember]
        public string Url { get; set; }
    }


    [DataContract]
    public class GetGamesInfoRequest
    {
        [DataMember]
        public int BrandId { get; set; }
    }

    [DataContract]
    public class GetGamesInfoResponse
    {
        public GetGamesInfoResponse()
        {

        }

        public GetGamesInfoResponse(Entities.LayoutTool.GameInfo[] games)
        {
            this.Games = games;
        }

        [DataMember]
        public Entities.LayoutTool.GameInfo[] Games { get; set; }
    }

    [DataContract]
    public class DownloadFileContentResponse
    {
        public DownloadFileContentResponse()
        {

        }

        
        [DataMember]
        public int HttpErrorCode { get; set; }
        [DataMember]
        public string HttpErrorDescription { get; set; }
        [DataMember]
        public string Content { get; set; }
    }



    
}
