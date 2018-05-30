using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Service;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Configurations;
using GamesPortal.Service.Entities;
using Spark.Infra.Types;

namespace Tools.ViewModels
{
    public class GameLanguagesGenerator : INotifyPropertyChanged
    {
        ArtifactoryRestClientFactory _restClientFactory;
        IConfigurationReader _configurationReader;
        public GameLanguagesGenerator(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
            _restClientFactory = new ArtifactoryRestClientFactory(configurationReader);

            InitRepositories(configurationReader);

            CopyLanguagesFromOtherVersionCommand = new Command(CopyLanguagesFromOtherVersion);
            GenerateDefaultLanguagesCommand = new Command(GenerateDefaultLanguages);
            NewHashCommand = new Command<LanguageSelectorViewModel>(NewHash);
            SaveCommand = new Command(Save);
            AddLanguageCommand = new Command<Language>(AddLanguage);
            SynchronizeCommand = new Command(Synchronize);

        }

        private void Save()
        {
            foreach(var regulation in AvailableRegulationsForSelectedVersion)
            {
                var artifactoryProperties = GetLanguageProperties(regulation);
                var request = new UpdateArtifactPropertiesRequest(this.GameType.Value, 
                                                                  regulation, 
                                                                  this.GameVersion.ParsedVersion,
                                                                  artifactoryProperties);
                SelectedRepository.Repository.UpdateArtifactProperties(request);
            }
            
            System.Windows.MessageBox.Show("Done!");
        }


        private bool _useLanguageToRegulationMapping;

        public bool UseLanguageToRegulationMapping
        {
            get { return _useLanguageToRegulationMapping; }
            set
            {
                _useLanguageToRegulationMapping = value;
                OnPropertyChanged(nameof(UseLanguageToRegulationMapping));
            }
        }

        private LanguageProperty[] GetLanguageProperties(string regulation)
        {
            var languages = this.Languages.Where(lng => lng.IsSelected);

            

            if (UseLanguageToRegulationMapping && regulation != "Gibraltar")
                languages = languages.Where(lng => GetMandatoryLanguagesIso3(regulation).Contains(lng.Language.Iso3_1));


             return languages.Select(lng => LanguageProperty.FromLanguage(SelectLanguageIso(lng), lng.Hash))
                      .ToArray();
        }

        private IEnumerable<string> GetMandatoryLanguagesIso3(string regulation)
        {
            using (var dbContext = new GamesPortalDatabaseDataContext())
            {
                var regulationTypeRecord = dbContext.RegulationTypes.FirstOrDefault(row => row.RegulationName == regulation);

                if (regulationTypeRecord == null)
                    return new string[0];

                return regulationTypeRecord.RegulationType_MandatoryLanguages.Select(l => l.LanguageIso3).ToArray();
            }
        }

        private string SelectLanguageIso(LanguageSelectorViewModel lng)
        {
            if (SelectedRepository.Infrastructure.GameTechnology == GameTechnology.Html5)
                return lng.Language.Iso3_1;
            else
                return lng.Language.Iso2;
        }

        private void NewHash(LanguageSelectorViewModel languageSelector)
        {
            languageSelector.Hash = Guid.NewGuid().ToString();
        }

        public ICommand NewHashCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public ICommand SynchronizeCommand { get; private set; }

        private void Synchronize()
        {
            using (var proxy = new BuildMachineAdapterService.GamesPortalToBuildMachineAdapterClient())
            {
                
                proxy.ForceGameSynchronization(new BuildMachineAdapterService.ExplicitForceGameSynchronizationRequest()
                {
                    GameType = this.GameType.Value,
                    VersionFolder = this.GameVersion.ParsedVersion,
                    RepositoryName = this.SelectedRepository.Repository.RepositoryName,
                    GamesFolderName = this.SelectedRepository.Repository.GamesFolderPath
                });

            }
        }

        private void InitRepositories(IConfigurationReader configurationReader)
        {
            var artifactorySettings = configurationReader.ReadSection<ArtifactorySettings>();
            AvailableRepositories = artifactorySettings.GamesRepositories.GetGamesRespositories(_restClientFactory).ToArray();
        }

       

        
        public IArtifactoryRepositoryDescriptor[] AvailableRepositories { get; private set; }
        GamesRepositoryDescriptor _selectedRepository;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GamesRepositoryDescriptor SelectedRepository
        {
            get
            {
                return _selectedRepository;
            }
            set
            {
                _selectedRepository = value;
                OnPropertyChanged(nameof(SelectedRepository));

                LoadAvailableGameTypes();
            }
        }

        private void LoadAvailableGameTypes()
        {
            int[] gameTypes = new int[0];
            if (SelectedRepository != null)
                gameTypes = SelectedRepository.Repository.GetGames().OrderBy(gt => gt).ToArray();

            this.AvailableGameTypes = gameTypes;
            
        }

        int[] _availableGameTypes;
        public int[] AvailableGameTypes
        {
            get { return _availableGameTypes; }
            set
            {
                _availableGameTypes = value;
                OnPropertyChanged(nameof(AvailableGameTypes));
                this.GameType = null;
            }
        }

        int? _gameType;
        public int? GameType
        {
            get { return _gameType; }
            set
            {
                _gameType = value;
                OnPropertyChanged(nameof(GameType));

                LoadVersions();
            }
        }

        private string[] _availableRegulationsForSelectedVersion = new string[0];

        public string[] AvailableRegulationsForSelectedVersion
        {
            get { return _availableRegulationsForSelectedVersion; }
            set
            {
                _availableRegulationsForSelectedVersion = value;
                OnPropertyChanged(nameof(AvailableRegulationsForSelectedVersion));
                OnPropertyChanged(nameof(AvailableRegulationsForSelectedVersionDescription));
            }
        }

        public string AvailableRegulationsForSelectedVersionDescription
        {
            get
            {
                return string.Join(", ", AvailableRegulationsForSelectedVersion);
            }
        }

        private void LoadVersions()
        {
            var versions = new List<VersionNumber>();
            

            if (this.SelectedRepository != null && this.GameType != null)
                
                foreach (var regulation in this.SelectedRepository.Repository.GetComponentRegulations(this.GameType.Value))
                {
                    foreach(var versionFolder in this.SelectedRepository.Repository.GetVersionFolders(this.GameType.Value, regulation))
                    {
                        versions.Add(new VersionNumber(versionFolder));
                    }
                }

            this.AvailableVersions = versions.Distinct().OrderByDescending(v => v).ToArray();
        }

        VersionNumber[] _availableVersions;
        public VersionNumber[] AvailableVersions
        {
            get { return _availableVersions; }
            set
            {
                _availableVersions = value;
                OnPropertyChanged(nameof(AvailableVersions));
                this.GameVersion = null;
            }
        }
        VersionNumber _gameVersion;
        public VersionNumber GameVersion
        {
            get { return _gameVersion; }
            set
            {
                _gameVersion = value;
                OnPropertyChanged(nameof(GameVersion));
                LoadRegulations();
                LoadLanguages();
            }
        }

        private void LoadRegulations()
        {
            if(GameVersion == null)
            {
                AvailableRegulationsForSelectedVersion = new string[0];
                return;
            }

            var regulations = new List<string>();


            foreach(var regulation in SelectedRepository.Repository.GetComponentRegulations(this.GameType.Value))
            {
                SelectedRepository.Repository.GetArtifact(this.GameType.Value, regulation, this.GameVersion.ParsedVersion)
                                             .Do(Artifact => regulations.Add(regulation));
                
            }


            AvailableRegulationsForSelectedVersion = regulations.Distinct().ToArray();
        }

        private VersionNumber _versionToCopyFrom;

        public VersionNumber VersionToCopyFrom
        {
            get { return _versionToCopyFrom; }
            set
            {
                _versionToCopyFrom = value;
                OnPropertyChanged(nameof(VersionToCopyFrom));
            }
        }

      

        public ICommand CopyLanguagesFromOtherVersionCommand { get; private set; }
        public ICommand GenerateDefaultLanguagesCommand { get; private set; }

        private void CopyLanguagesFromOtherVersion()
        {
            this.Languages = ReadLanguages(this.VersionToCopyFrom);
            
        }

        private void GenerateDefaultLanguages()
        {
            var languages = new ObservableCollection<LanguageSelectorViewModel>();

            foreach(var lang in _defaultLanguages)
            {
                languages.Add(new LanguageSelectorViewModel(lang));
            }
            
            this.Languages = languages;
            
        }

        public Language[] LanguagesThatCanBeAdded
        {
            get
            {
                var result = _defaultLanguages.Where(language => !Languages.Select(l => l.Language).Contains(language))
                                        .ToArray();

                return result;
                                        
            }
        }

        public ICommand AddLanguageCommand { get; private set; }

        private void AddLanguage(Language language)
        {
            this.Languages.Add(new LanguageSelectorViewModel(language));
            OnPropertyChanged(nameof(LanguagesThatCanBeAdded));
        }


        private Language[] _defaultLanguages = new Language[]
        {
            Language.English,
            Language.Spanish,
            Language.Italian,
            Language.Danish,
            Language.Romanian,
            Language.French,
            Language.German
        };

        ObservableCollection<LanguageSelectorViewModel> _languages = new ObservableCollection<LanguageSelectorViewModel>();
        public ObservableCollection<LanguageSelectorViewModel> Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
                OnPropertyChanged(nameof(Languages));
                OnPropertyChanged(nameof(LanguagesThatCanBeAdded));
            }
        }
        
        private void LoadLanguages()
        {
            this.Languages = ReadLanguages(this.GameVersion);
        }

        private ObservableCollection<LanguageSelectorViewModel> ReadLanguages(VersionNumber version)
        {
            

            if (version == null)
                return new ObservableCollection<LanguageSelectorViewModel>();

            
            foreach(var regulation in AvailableRegulationsForSelectedVersion)
            {
                var languages = new ObservableCollection<LanguageSelectorViewModel>();
                this.SelectedRepository.Repository.GetArtifact(GameType.Value, 
                                                               regulation, 
                                                               version.ParsedVersion)
                                                   .Do(artifact => languages = ReadLanguagesFromArtifact(artifact));

                if (languages.Any())
                    return languages;
                
            }
            
       
            return new ObservableCollection<LanguageSelectorViewModel>();

        }


        private ObservableCollection<LanguageSelectorViewModel> ReadLanguagesFromArtifact(Artifact artifact)
        {
            var languages = new ObservableCollection<LanguageSelectorViewModel>();
            foreach (var property in artifact.Properties)
            {
                if (!LanguageProperty.IsLanguageHash(property.Key))
                    continue;

                var languageProperty = LanguageProperty.FromKey(property.Key, property.ConcatValues());

                languages.Add(new LanguageSelectorViewModel(Language.Find(languageProperty.Language), languageProperty.Hash));
            }


            return languages;
        }

        public class LanguageSelectorViewModel : INotifyPropertyChanged
        {
            public LanguageSelectorViewModel(Language language)
                : this(language, Guid.NewGuid().ToString())
            {

            }
            public LanguageSelectorViewModel(Language language, string hash)
            {
                this.Language = language;
                this.Hash = hash;
            }

            public Language Language { get; private set; }
            
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            string _hash;
            public string Hash
            {
                get { return _hash; }
                set
                {
                    _hash = value;
                    OnPropertyChanged(nameof(Hash));
                }
            }

            bool _isSelected = true;
            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

     
    }
}
