using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace GGPMockBootstrapper.Models
{
    public interface IGamesInformationProvider
    {
        GameInfoModel[] GetAllGames();
        GameInfoModel GetGameInfoOrNull(int gameType);
        void Refresh();
    }

    public class GamesInformationProvider : IGamesInformationProvider
    {

        
        public GamesInformationProvider()
        {
            ReadGamesInformationAsync();

        }

        private void ReadGamesInformationAsync()
        {
            Thread t1 = new Thread(new ThreadStart(() => ReadGamesInformation()));
            t1.IsBackground = true;
            t1.Start();
        }




        List<GameInfoModel> _games = new List<GameInfoModel>();

        public GameInfoModel[] GetAllGames()
        {
           
           return _games.ToArray();         
            
        }


        public GameInfoModel GetGameInfoOrNull(int gameType)
        {
            return GetAllGames().FirstOrDefault(g => g.GameType == gameType);
        }
        

        private void ReadGamesInformation()
        {
            bool shouldRetry = true;
            do
            {
                
                try
                {

                    using (var safeRelease = CreateWcfSasfeRelease())
                    {
                        _games = safeRelease.Chanel.GetAllGames().Select(g => new GameInfoModel(g)).ToList();
                    }

                    shouldRetry = false;
                }
                catch
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }


            } while (shouldRetry);
        }

        public void Refresh()
        {
            ReadGamesInformationAsync();
        }

        private IWcfSafeRelease<GamesInformationService.IGamesInformationService> CreateWcfSasfeRelease()
        {
            return new WcfClientProxySafeRelease<GamesInformationService.GamesInformationServiceClient, GamesInformationService.IGamesInformationService>(new GamesInformationService.GamesInformationServiceClient());
        }


    }

    public class GameInfoModel
    {
       
        public string FriendlyName { get; set; }
        public string GameEngineAssemblyName { get; set; }
        public string GameEngineTypeName { get; set; }
        public string GameEngineVersion { get; set; }
        public int[] GameGroups { get; set; }
        public int GameType { get; set; }
        public string GameUniqueName { get; set; }
        public string GameVersion { get; set; }
        //public int HandlersPoolSize { get; set; }
        public bool IsISDInUse { get; set; }
        public bool IsSubGame { get; set; }
        public int OperatorId { get; set; }
        //public string[] ProtocolAssemblies { get; set; }

        public GameInfoModel(GamesInformationService.GameInfo gameInfo)
        {
            this.FriendlyName = gameInfo.FriendlyName;
            this.GameEngineAssemblyName = gameInfo.GameEngineAssemblyName;
            this.GameEngineTypeName = gameInfo.GameEngineTypeName;
            this.GameEngineVersion = gameInfo.GameEngineVersion;
            this.GameGroups = gameInfo.GameGroups;
            this.GameType = gameInfo.GameType;
            this.GameUniqueName = gameInfo.GameUniqueName;
            this.GameVersion = gameInfo.GameVersion;
            this.IsISDInUse = gameInfo.IsISDInUse;
            this.IsSubGame = gameInfo.IsSubGame;
            this.OperatorId = gameInfo.OperatorId;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.FriendlyName, this.GameType);
        }

        public override int GetHashCode()
        {
            return this.GameType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as GameInfoModel;

            if (theOther == null)
                return false;


            if (this.GameType != theOther.GameType)
                return false;


            if (this.GameGroups != null)
                return this.GameGroups.SequenceEqual(theOther.GameGroups);
            else if (theOther.GameGroups == null)
                return true;
            else
                return false;
                    
        }
    }
}
