using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public class AvailableGamesViewModel : WorkAreaItemBase
    {
        public AvailableGamesViewModel(IWorkArea workArea)
            : base(workArea)
        {
            _propertiesToFilterOn = typeof(GameInfoViewModel).GetProperties().ToArray();

            ReadAllGames();
            this.Games = new ObservableCollection<GameInfoViewModel>();

            ReLoadGames();


        }

        private void ReadAllGames()
        {
            _allGames = this.GamesInformationProvider.GetAllGames().Select(g => new GameInfoViewModel(g)).ToArray();
        }

        void GamesInformationProvider_GamesChanged(object sender, EventArgs e)
        {
            UIServices.InvokeOnMainThread(() =>
                {
                    ReadAllGames();
                    ReLoadGames();

                });
        }


        string _filter;
        System.Text.RegularExpressions.Regex _regEx;
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {

                if (_filter == value)
                    return;

                _filter = value;


                if (_filter != null)
                {
                    _filter = _filter.Trim();
                }

                if (string.IsNullOrEmpty(_filter))
                {
                    _regEx = null;
                }
                else
                {
                    _regEx = new System.Text.RegularExpressions.Regex(_filter, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }

                OnPropertyChanged(this.GetPropertyName(t => t.Filter));

                ReLoadGames();


                
            }
        }
        

        private void ReLoadGames()
        {
            this.Games.Clear();

            var newGames = new ObservableCollection<GameInfoViewModel>();
            foreach (var game in _allGames.Where(g => RespectsFilter(g)))
            {
                newGames.Add(game);
            }

            this.Games = newGames;
        }

        PropertyInfo[] _propertiesToFilterOn;
        private bool RespectsFilter(GameInfoViewModel g)
        {
            if (_regEx == null)
                return true;


            foreach (var pinfo in _propertiesToFilterOn)
            {
                var value = pinfo.GetValue(g, new object[0]);

                if (value != null && _regEx.IsMatch(value.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        Models.IGamesInformationProvider GamesInformationProvider
        {
            get
            {
                return this.WorkArea.GamesInformationProvider;
            }
            
        }

        private GameInfoViewModel[] _allGames;

        ObservableCollection<GameInfoViewModel> _games = new ObservableCollection<GameInfoViewModel>();
        public ObservableCollection<GameInfoViewModel> Games
        {
            get
            {
                return _games;
            }
            set
            {
                _games = value;

                OnPropertyChanged(this.GetPropertyName(t => t.Games));
            }
        }
    }

    public class GameInfoViewModel
    {
        public GameInfoViewModel(Models.GameInfoModel gameInfo)
        {
            this.GameInfo = gameInfo;
        }

        Models.GameInfoModel GameInfo { get; set; }

        public int GameType
        {
            get { return this.GameInfo.GameType; }
        }

        public string Name
        {
            get { return this.GameInfo.FriendlyName; }
        }

        public string Version
        {
            get { return this.GameInfo.GameVersion; }
        }

        public bool IsSubGame
        {
            get { return this.GameInfo.IsSubGame; }
        }

        public bool IsISDEnabled
        {
            get { return this.GameInfo.IsISDInUse; }
        }


        public string Operator
        {
            get
            {
                if (this.GameInfo.OperatorId == 0)
                    return "888";

                if (this.GameInfo.OperatorId == 1)
                    return "Bingo";

                return this.GameInfo.OperatorId.ToString();
            }
        }

        public string Groups
        {
            get
            {

                if (this.GameInfo.GameGroups.IsNullOrEmpty())
                    return string.Empty;

                return string.Join(", ", this.GameInfo.GameGroups.Select(g => g.ToString()));
            }
        }

        public string EngineAssembly
        {
            get { return this.GameInfo.GameEngineAssemblyName; }
        }

        public string EngineVersion
        {
            get { return this.GameInfo.GameEngineVersion; }
        }

       

    }

}
