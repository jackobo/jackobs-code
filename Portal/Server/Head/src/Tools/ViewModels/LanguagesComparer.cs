using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using Spark.Infra.Configurations;
using Spark.Infra.Types;

namespace Tools.ViewModels
{
    public class LanguagesComparer
    {
        public LanguagesComparer(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
            RestClientFactory = new ArtifactoryRestClientFactory(configurationReader);

            InitRepositories(configurationReader);
        }

        ArtifactoryRestClientFactory RestClientFactory;
        IConfigurationReader _configurationReader;

        private void InitRepositories(IConfigurationReader configurationReader)
        {

            AvailableRepositories = new List<IArtifactoryRepositoryDescriptor>();
            AvailableRepositories.AddRange(CreateDefaultGamesRepositoryDescriptors());
            AvailableRepositories.AddRange(CreateDefaultChillWrapperRepositoryDescriptors());
        }

        private List<GamesRepositoryDescriptor> CreateDefaultGamesRepositoryDescriptors()
        {
            var defaultGamesRepositoryDescriptors = new List<GamesRepositoryDescriptor>();

            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.PcAndMobile, new GamesRepository("HTML5Game-local", "Games", RestClientFactory)));
            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.Mobile, new GamesRepository("HTML5Game-local", "Games_MOBILE", RestClientFactory)));
            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.PC, new GamesRepository("HTML5Game-local", "Games_PC", RestClientFactory)));

            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Flash, false, PlatformType.PC, new GamesRepository("modernGame-local", "Games", RestClientFactory)));
            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Flash, true, PlatformType.PC, new GamesRepository("externalGame-local", "Games", RestClientFactory)));
            
            return defaultGamesRepositoryDescriptors;
        }

        private IEnumerable<ChillWrapperRepositoryDescriptor> CreateDefaultChillWrapperRepositoryDescriptors()
        {
            var defaultChillWrapperRepositoryDescriptors = new List<ChillWrapperRepositoryDescriptor>();

            defaultChillWrapperRepositoryDescriptors.Add(new ChillWrapperRepositoryDescriptor(ChillWrapperType.Chill.Infrastructure, new ChillWrapperRepository("HTML5Game-local", "Wrapper/chill", RestClientFactory, GamingComponentCategory.Chill)));
            defaultChillWrapperRepositoryDescriptors.Add(new ChillWrapperRepositoryDescriptor(ChillWrapperType.Wrapper.Infrastructure, new ChillWrapperRepository("modernGame-local", "Wrapper", RestClientFactory, GamingComponentCategory.Wrapper)));

            return defaultChillWrapperRepositoryDescriptors;
        }

        public IEnumerable<CompareResult> Compare()
        {
            var inDb = GetApprovedLanguagesInDB();
            var inArt = GetApprovedLanguagesInArtifactory();

            //System.Windows.MessageBox.Show($"In Db: {inDb.Count().ToString()}; InArt: {inArt.Count().ToString()}");

            var result = new List<CompareResult>();

            //indb not in artifactory
            foreach(var deletedItem in inDb.Where(inDBitem => !inArt.Select(inArtItem => inArtItem.Key).Contains(inDBitem.Key)).OrderBy(x => x.Key.ToString()))
            {
                result.Add(new CompareResult(deletedItem, null));
            }


            //artifactory not in indb
            foreach (var newItem in inArt.Where(inArtItem => !inDb.Select(inDbItem => inDbItem.Key).Contains(inArtItem.Key)).OrderBy(x => x.Key.ToString()))
            {
                result.Add(new CompareResult(null, newItem));
            }

            //different
            result.AddRange((from db in inDb
                            join art in inArt on db.Key equals art.Key
                            where !db.ApprovedLanguages.SequenceEqual(art.ApprovedLanguages)
                            select new CompareResult(db, art))
                            .OrderBy(x => x.InDB.Key.ToString()));


            return result;


        }

        public List<IArtifactoryRepositoryDescriptor> AvailableRepositories { get; private set; }


        private IEnumerable<QAApprovedLanguages> GetApprovedLanguagesInDB()
        {
            var result = new List<QAApprovedLanguages>();
            using (var dbContext = new GamesPortalDatabaseDataContext(System.Configuration.ConfigurationManager.ConnectionStrings["GamesPortalDB"].ConnectionString))
            {
                var query = (from game in dbContext.GetTable<Game>()
                            join gameVersion in dbContext.GetTable<GameVersion>() on game.Game_ID equals gameVersion.Game_ID
                            join gameVersionLanguage in dbContext.GetTable<GameVersion_Language>() on gameVersion.GameVersion_ID equals gameVersionLanguage.GameVersion_ID
                            where gameVersionLanguage.QAApprovalDate != null
                            select new { game.MainGameType, game.IsExternal, gameVersion.VersionFolder, gameVersion.Technology, gameVersion.PlatformType, gameVersionLanguage.Regulation, gameVersionLanguage.ArtifactoryLanguage }
                            )
                            .ToArray();


                foreach(var record in query.GroupBy(r => 
                                                        new
                                                        {
                                                            r.MainGameType, 
                                                            r.VersionFolder,
                                                            r.IsExternal,
                                                            GameInfrastructure = new GameInfrastructureDTO(r.Technology, r.PlatformType),
                                                            r.Regulation
                                                        }))
                {
                    result.Add(new QAApprovedLanguages(record.Key.MainGameType, record.Key.VersionFolder, record.Key.IsExternal, record.Key.GameInfrastructure, record.Key.Regulation,
                                                       record.Select(item => item.ArtifactoryLanguage).ToArray()));
                }
            }

            return result;
        }


        private IEnumerable<QAApprovedLanguages> GetApprovedLanguagesInArtifactory()
        {
            var result = new List<QAApprovedLanguages>();
            foreach (var repositoryDescriptor in AvailableRepositories)
            {
                foreach(var gameType in repositoryDescriptor.Repository.GetGames())
                {
                    var regulations = repositoryDescriptor.Repository.GetComponentRegulations(gameType);
                    foreach (var regulation in regulations)
                    {
                        var versions = repositoryDescriptor.Repository.GetVersionFolders(gameType, regulation)
                                                                              .OrderByDescending(v => new VersionNumber(v).ToLong())
                                                                              .ToArray();
                        foreach (var version in versions)
                        {
                            var optionalArtifact = repositoryDescriptor.Repository.GetArtifact(gameType, regulation, version);

                            optionalArtifact.Do(artifact =>
                            {
                                var prop = artifact.Properties.FirstOrDefault(p => p.Key == LanguageProperty.Language_QAApproved);
                                if (prop != null)
                                {
                                    result.Add(new QAApprovedLanguages(gameType,
                                        version,
                                        repositoryDescriptor.IsExternal,
                                        repositoryDescriptor.Infrastructure,
                                        regulation,
                                        prop.Values.SelectMany(v => v.Split(',', ';')).ToArray()));
                                }
                            });
                        }
                    }
                }
            }

            return result;
        }


        public class CompareResult
        {
            public CompareResult(QAApprovedLanguages inDb, QAApprovedLanguages inArtifactory)
            {
                this.InDB = inDb;
                this.InArtifactory = inArtifactory;
            }
            public QAApprovedLanguages InDB { get; private set; }
            public QAApprovedLanguages InArtifactory { get; private set; }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (InDB == null)
                    sb.Append("(not in DB)");
                else
                    sb.Append($"({InDB})");

                sb.Append("\t");

                if (InArtifactory == null)
                    sb.Append("(not in Artifactory)");
                else
                    sb.Append($"({InArtifactory})");

                return sb.ToString();
            }
        }


        public class QAApprovedLanguages
        {
            public QAApprovedLanguages(int gameType, string version, bool isExternal, GameInfrastructureDTO gameInfrastructure, string regulation, string[] approvedLanguages)
            {
                this.Key = new TheKey(gameType, isExternal, version, gameInfrastructure, regulation);
                this.ApprovedLanguages = approvedLanguages.OrderBy(l => l).ToArray();
            }


            public string[] ApprovedLanguages { get; private set; }


            public TheKey Key { get; private set; }

            public override string ToString()
            {
                return $"{Key}; {string.Join(",", ApprovedLanguages)}";
            }

            public class TheKey
            {
                public TheKey(int gameType, bool isExternal, string versionFolder, GameInfrastructureDTO gameInfrastructure, string regulation)
                {
                    this.GameType = gameType;
                    this.IsExternal = isExternal;
                    this.VersionFolder = versionFolder;
                    this.GameInfrastructure = gameInfrastructure;
                    this.Regulation = regulation;
                }

                public int GameType { get; private set; }
                public bool IsExternal { get; private set; }
                public string VersionFolder { get; private set; }
                public GameInfrastructureDTO GameInfrastructure { get; private set; }
                public string Regulation { get; private set; }

                public override bool Equals(object obj)
                {
                    var theOther = obj as TheKey;
                    if (theOther == null)
                        return false;


                    return this.GameType == theOther.GameType
                            && this.IsExternal == theOther.IsExternal
                            && this.GameInfrastructure.Equals(theOther.GameInfrastructure)
                            && this.Regulation == theOther.Regulation
                            && this.VersionFolder == theOther.VersionFolder;
                }

                public override int GetHashCode()
                {
                    return this.GameType.GetHashCode()
                            ^ this.GameInfrastructure.GetHashCode()
                            ^ this.IsExternal.GetHashCode()
                            ^ this.Regulation.GetHashCode()
                            ^ this.VersionFolder.GetHashCode();
                }

                public override string ToString()
                {
                    return $"{GameInfrastructure}; {IsExternal}; {GameType}; {Regulation}; {VersionFolder}";
                }
            }
        }
    }
}
