using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GamesPortal.Client.Interfaces;
using GamesPortal.Client.Interfaces.PubSubEvents;
using GamesPortal.Client.Interfaces.Services;
using Spark.Infra.Types;


namespace GamesPortal.Client.Interfaces.Entities
{

    public class Game
    {
        public Game(Guid id, string name, int mainGameType, bool isExternal, GamingComponentCategory category, GameInfrastructure[] supportedInfrastructures,  GameType[] gameTypes,  IGamesRepository gamesRepository)
        {

            this.Id = id;
            this.Name = name;
            this.MainGameType = mainGameType;
            this.IsExternal = isExternal;
            this.SupportedInfrastructures = supportedInfrastructures;
            this.GameTypes = gameTypes;
            this.Category = category;
            this.GamesRepository = gamesRepository;
            _versions = new Lazy<GameVersion[]>(() => this.GamesRepository.GetGameVersions(this.Id), true);
        }


        public Guid Id { get; private set; }

        private bool _isExternal;

        public bool IsExternal
        {
            get { return _isExternal; }
            set
            {
                _isExternal = value;
                OnPropertyChanged(this.GetPropertyName(t => t.IsExternal));
            }
        }

        public GamingComponentCategory Category { get; private set; }

        int _mainGameType;
        public int MainGameType
        {
            get { return _mainGameType; }
            set
            {
                _mainGameType = value;
                OnPropertyChanged(this.GetPropertyName(t => t.MainGameType));
            }
        }

        string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    return this.MainGameType.ToString();
                else
                    return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(this.GetPropertyName(t => t.Name));
            }
        }

        GameInfrastructure[] _supportedInfrastructures;

        public GameInfrastructure[] SupportedInfrastructures
        {
            get { return _supportedInfrastructures; }
            set
            {
                _supportedInfrastructures = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SupportedInfrastructures));
            }
        }

        GameType[] _gameTypes;
        public GameType[] GameTypes
        {
            get
            {
                return _gameTypes;
            }
            set
            {
                _gameTypes = value;
                OnPropertyChanged(this.GetPropertyName(t => t.GameTypes));
            }
        }


        public override bool Equals(object obj)
        {
            var theOther = obj as Game;

            if (theOther == null)
                return false;

            return this.MainGameType == theOther.MainGameType;
        }

        public Optional<VersionNumber> GetPreviouslyApprovedVersionForLanguage(Language language, GameVersion currentVersion)
        {
            var previousApproved = this.Versions.Where(v => v.Version < currentVersion.Version && v.Infrastructure == currentVersion.Infrastructure)
                                                .SelectMany(v => v.Regulations.SelectMany(r => r.Languages.Where(l => l.Language == language && l.ApprovalInfo.IsApproved)
                                                                                                           .Select(l => l.Language)
                                                                                                           .Distinct()
                                                                                                           .Select(l => new { v.Version, Language = l })))
                                                .OrderByDescending(v => v.Version)
                                                .FirstOrDefault();


            if (previousApproved == null)
                return Optional<VersionNumber>.None();

            return Optional<VersionNumber>.Some(previousApproved.Version);
                         
        }

        
        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return this.MainGameType.GetHashCode();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;




        protected virtual void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null)
                ev(this, new PropertyChangedEventArgs(propertyName));


        }

        #endregion


        IGamesRepository GamesRepository { get; set; }


        Lazy<GameVersion[]> _versions;
        
        public GameVersion[] Versions
        {
            get
            {
                try
                {
                    return _versions.Value;
                }
                catch
                {
                    _versions = new Lazy<GameVersion[]>(() => this.GamesRepository.GetGameVersions(this.Id), true);

                    throw;
                }
            }
        }


        public void ResetVersions()
        {
            _versions = new Lazy<GameVersion[]>(() => this.GamesRepository.GetGameVersions(this.Id), true);
            OnPropertyChanged(this.GetPropertyName(t => t.Versions));
        }
    }

}
