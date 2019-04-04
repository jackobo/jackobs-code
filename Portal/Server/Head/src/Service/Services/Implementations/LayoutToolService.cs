using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using GamesPortal.Service.Entities.LayoutTool;
using GamesPortal.Service.LayoutTool;
using Spark.Infra.Types;
using Microsoft.Practices.Unity;

namespace GamesPortal.Service
{
    
    public class LayoutToolService : WcfServiceBase, ILayoutToolService
    {
        public LayoutToolService()
        {

        }

        public LayoutToolService(IGamesPortalInternalServices services)
            : base(services)
        {

        }


        static readonly int CASINO_GAME_CATEGORY = 3;
        static readonly int PC_PLATFORM_TYPE = 1;
        static readonly int BOTH_PLATFORM_TYPE = 3;

        public DownloadFileContentResponse DownloadFileContent(DownloadFileContentRequest request)
        {
            try
            {
                using (var webClient = CreateWebClient())
                {
                    return new DownloadFileContentResponse() { Content = webClient.DownloadString(request.Url) };
                }
            }
            catch (System.Net.WebException ex)
            {
                var resp = (ex.Response as HttpWebResponse);

                if (resp != null)
                {
                    return new DownloadFileContentResponse() { HttpErrorCode = (int)resp.StatusCode, HttpErrorDescription = resp.StatusDescription };
                }
                else
                {
                    LogException("DownloadFileContent", ex);
                    throw ex;
                }

            }
            catch (Exception ex)
            {
                LogException("DownloadFileContent", ex);
                throw;
            }
        }

        public GetGamesInfoResponse GetGamesInfo(GetGamesInfoRequest request)
        {

            try
            {

                var result = new List<GameInfo>();

                using (var dbSdm = Services.CreateSdmDbContext())
                using(var jackpotInfoDb = Services.CreateJackpotInfoDbContext())
                using (var dbGamesPortal = Services.CreateGamesPortalDBDataContext())
                {
                    var gameGroupNames = dbSdm.GetTable<SDM.GameGroup>().ToDictionary(row => row.GG_GroupID, row => row.GG_GroupName);
                    var gamesWithJackpots = GetJackpotIdsPerGameType(jackpotInfoDb);
                    var approvedGames = GetApprovedGames(dbGamesPortal, dbSdm.GetRegulationIdForBrand(request.BrandId));

                    var allTheGames = dbGamesPortal.GetTable<DataAccessLayer.Game>().ToDictionary(g => g.MainGameType);

                    var gameVendorsNames = dbSdm.GetTable<SDM.GameVendor>().ToDictionary(gv => gv.GV_VendorID, gv => gv.GV_VendorName);

                    foreach (var gameRow in GetAllPlatformGames(dbSdm))
                    {
                        bool? isApproved = null;

                        if (allTheGames.ContainsKey(gameRow.GMT_ID))
                        {
                            isApproved = approvedGames.ContainsKey(gameRow.GMT_ID);
                        }

                        string vendorName = null;

                        if (gameRow.GMT_GV_VendorID != null && gameVendorsNames.ContainsKey(gameRow.GMT_GV_VendorID.Value))
                        {
                            vendorName = gameVendorsNames[gameRow.GMT_GV_VendorID.Value];
                        }
                        else
                        {
                            Logger.Error($"There is no vendor defined for gameType {gameRow.GMT_ID}");
                        }


                        var jackpotIds = gamesWithJackpots.ContainsKey(gameRow.GMT_ID) ? gamesWithJackpots[gameRow.GMT_ID] : new int[0];
                        result.Add(new GameInfo(gameRow.GMT_ID, 
                                                gameRow.GMT_Description, 
                                                gameGroupNames[gameRow.GMT_GroupID], 
                                                isApproved, 
                                                vendorName, 
                                                jackpotIds));
                    }


                }

                return new GetGamesInfoResponse(result.ToArray());
            }
            catch(Exception ex)
            {
                LogException(nameof(GetGamesInfo), ex, request);
                throw;
            }
            
        }

        private static SDM.GameType[] GetAllPlatformGames(SDM.ISdmDataContext dbSdm)
        {
            return dbSdm.GetTable<SDM.GameType>()
                                                                 .Where(row => row.GMT_GCT_ID == CASINO_GAME_CATEGORY
                                                                               && (row.GMT_PT_ID == PC_PLATFORM_TYPE || row.GMT_PT_ID == BOTH_PLATFORM_TYPE))
                                                                 .ToArray();
        }

        private static Dictionary<int, int> GetApprovedGames(DataAccessLayer.IGamesPortalDataContext dbGamesPortal, int regulationTypeId)
        {
            string[] propertiesNames = new string[]
                {
                    Artifactory.WellKnownNamesAndValues.BuildPropertyKey(Artifactory.WellKnownNamesAndValues.NDL, Artifactory.WellKnownNamesAndValues.State),
                    Artifactory.WellKnownNamesAndValues.BuildPropertyKey(Artifactory.WellKnownNamesAndValues.NDL, Artifactory.WellKnownNamesAndValues.PMApproved)
                };

            return dbGamesPortal.GetTable<LatestApprovedGameVersionForEachRegulation>()
                                             .Where(row => row.RegulationType_ID == regulationTypeId
                                                            && row.QA_Version != null
                                                            && (row.PlatformType == (int)PlatformType.PC || row.PlatformType == (int)PlatformType.PcAndMobile))
                                             .Select(row => row.MainGameType)
                                             .Distinct()
                                             .ToDictionary(gt => gt);
        }

       

        private WebClient CreateWebClient()
        {
            return new WebClient() { Encoding = Encoding.UTF8 };
        }

        public GetCountriesResponse GetCountries()
        {
            try
            {
                using (var dbSdm = Services.CreateSdmDbContext())
                {
                    return new GetCountriesResponse( dbSdm.GetTable<SDM.Country>().Where(row => row.CNT_NUM != null)
                                                        .Select(row => new CountryDto(row.CNT_NUM.Value, row.CNT_Name))
                                                        .ToArray());
                }
            }
            catch(Exception ex)
            {
                LogException(nameof(GetCountries), ex);
                throw;
            }
        }

        public GetCurrenciesResponse GetCurrencies()
        {
            try
            {
                using (var dbSdm = Services.CreateSdmDbContext())
                {
                    return new GetCurrenciesResponse(dbSdm.GetTable<SDM.Currency>()
                                               .Select(row => new CurrencyDto(row.CURR_Alpha, row.CUR_Name))
                                               .ToArray());
                }
            }
            catch(Exception ex)
            {
                LogException(nameof(GetCurrencies), ex);
                throw;
            }
        }

        public GetAllJackpotIdsResponse GetAllJackpotIds()
        {
            try
            {
                using (var jackpot = Services.CreateJackpotInfoDbContext())
                {
                    return new GetAllJackpotIdsResponse(jackpot.GetTable<Jackpot.Jackpot>().Select(row => (int)row.JP_ID).ToArray());
                }
            }
            catch(Exception ex)
            {
                LogException(nameof(GetAllJackpotIds), ex);
                throw;
            }
        }


        private Dictionary<int, int[]> GetJackpotIdsPerGameType(IJackpotInfoDBDataContext jackpotInfo)
        {
            return jackpotInfo.GetTable<Jackpot.JackpotGameType>()
                              .Where(row => row.JGT_GT_ID > 0)
                               .Select(row => new { row.JGT_GT_ID, row.JGT_JP_ID })
                               .GroupBy(row => row.JGT_GT_ID)
                               .ToDictionary(g => g.Key, g => g.Select(row => (int)row.JGT_JP_ID).ToArray());


            
        }
        

        public ReadLayoutFromTfsResponse ReadLayoutFromTfs(ReadLayoutFromTfsRequest request)
        {
            var fileContent = Services.TfsGateway.ReadFileContent(request.ServerFilePath);
            if(fileContent.Any())
                return new ReadLayoutFromTfsResponse(fileContent.First());

            throw new ArgumentException($"File {request.ServerFilePath} doesn't exists!");
        }

       
    }
}
