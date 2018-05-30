using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service
{

    [ServiceContract]
    public interface IGamesPortalService
    {
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetGameResponse GetGame(Guid gameId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetAllGamesResponse GetAllGames();

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetGameVersionsResponse GetGameVersions(Guid gameId);
                
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetLatestApprovedGameVersionForEachRegulation GetLatestApprovedGameVersionForEachRegulation();
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetLatestGameVersionForEachGameResponse GetLatestGameVersionForEachRegulation();

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetNeverApprovedGameResponse GetNeverApprovedGames();

        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
        void GetOptions();

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetGamesReleasesResponse GetGameReleases(GetGamesReleasesRequest request);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetRegulationsInfoResponse GetRegulationsInfo();

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetApprovedGamesInPeriodResponse GetQAApprovedGamesInPeriod(GetApprovedGamesInPeriodRequest request);

    }

    [DataContract]
    public class GetApprovedGamesInPeriodRequest
    {
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
    }

    [DataContract]
    public class GetApprovedGamesInPeriodResponse
    {
        public GetApprovedGamesInPeriodResponse()
        {

        }

        public GetApprovedGamesInPeriodResponse(ApprovedGameVersionDTO[] approvedGames)
        {
            this.ApprovedGames = approvedGames;
        }
        [DataMember]
        public ApprovedGameVersionDTO[] ApprovedGames { get; set; }
    }

    [DataContract]
    public class GetRegulationsInfoResponse
    {
        public GetRegulationsInfoResponse()
        {

        }

        public GetRegulationsInfoResponse(RegulationDTO[] regulations)
        {
            this.Regulations = regulations;
        }
        [DataMember]
        public RegulationDTO[] Regulations { get; set; }
    }

    [DataContract]
    public class GetNeverApprovedGameResponse
    {
        public GetNeverApprovedGameResponse()
        {

        }

        public GetNeverApprovedGameResponse(NeverApprovedGameDto[] games)
        {
            this.Games = games;
        }

        [DataMember]
        public NeverApprovedGameDto[] Games { get; set; }
    }

    [DataContract]
    public class NeverApprovedGameDto
    {
        public NeverApprovedGameDto()
        {
        }

        public NeverApprovedGameDto(string gameName, int mainGameType, GameInfrastructureDTO gameInfrastructure, long latestVersion)
        {
            this.GameName = gameName;
            this.MainGameType = mainGameType;
            this.GameInfrastructure = gameInfrastructure;
            this.LatestVersion = latestVersion;
        }
        
        [DataMember]
        public string GameName { get; set; }
        [DataMember]
        public int MainGameType { get; set; }
        [DataMember]
        public GameInfrastructureDTO GameInfrastructure { get; set; }
        [DataMember]
        public long LatestVersion { get; set; }
    }

    [DataContract]
    public class GetGamesReleasesRequest
    {
        [DataMember]
        public DateTime FromDate { get; set; }

        [DataMember]
        public DateTime ToDate { get; set; }
    }

    [DataContract]
    public class GetGamesReleasesResponse
    {

        public GetGamesReleasesResponse()
        {

        }

        public GetGamesReleasesResponse(GameVersionReleaseDTO[] gameVersions)
        {
            this.GameVersions = gameVersions;
        }

        [DataMember]
        public GameVersionReleaseDTO[] GameVersions { get; set; }
    }



    [DataContract]
    public class GetGameResponse
    {
        public GetGameResponse()
        {

        }

        public GetGameResponse(GameDTO game)
        {
            this.Game = game;
        }

        [DataMember]
        public GameDTO Game { get; set; }
    }




    [DataContract]
    public class GetLatestGameVersionForEachGameResponse
    {
        public GetLatestGameVersionForEachGameResponse()
        {

        }

        public GetLatestGameVersionForEachGameResponse(LatestGameVersionForRegulationDTO[] versions)
        {
            this.Versions = versions;
        }

        [DataMember]
        public LatestGameVersionForRegulationDTO[] Versions { get; set; }
    }

    [DataContract]
    public class GetGameVersionDownloadInfoRequest
    {
        public GetGameVersionDownloadInfoRequest()
        {

        }

        public GetGameVersionDownloadInfoRequest(params Guid[] gameVersions )
        {
            this.GameVersions = gameVersions;
        }
        [DataMember]
        public Guid[] GameVersions { get; set; }
    }
    

    [DataContract]
    public class GetAllGamesResponse
    {
        public GetAllGamesResponse()
        {

        }

        public GetAllGamesResponse(Entities.GameDTO[] games)
        {
            this.Games = games;
        }

        [DataMember]
        public Entities.GameDTO[] Games { get; set; }
    }


    [DataContract]
    public class GetGameVersionsResponse
    {
        public GetGameVersionsResponse()
        {

        }

        public GetGameVersionsResponse(Entities.GameVersionDTO[] gameVersions)
        {
            this.GameVersions = gameVersions;
        }

        [DataMember]
        public Entities.GameVersionDTO[] GameVersions { get; set; }
    }

    [DataContract]
    public class GetLatestApprovedGameVersionForEachRegulation
    {
        public GetLatestApprovedGameVersionForEachRegulation()
        {

        }

        public GetLatestApprovedGameVersionForEachRegulation(Entities.LatestApprovedGameVersionForRegulationDTO[] latestApprovedGamesVersions)
        {
            this.LatestApprovedGamesVersions = latestApprovedGamesVersions;
        }

        [DataMember]
        public Entities.LatestApprovedGameVersionForRegulationDTO[] LatestApprovedGamesVersions { get; set; }
    }


}
