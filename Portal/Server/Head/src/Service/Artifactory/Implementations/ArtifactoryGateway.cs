using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GamesPortal.Service.Configurations;
using GamesPortal.Service.DataAccessLayer;
using GGPGameServer.ApprovalSystem.Common.Logger;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using RestSharp.Authenticators;

namespace GamesPortal.Service.Artifactory
{
    public class ArtifactoryGateway : IArtifactoryGateway
    {
        
        public ArtifactoryGateway(IGamesPortalInternalServices services)
        {
            Services = services;
            Logger = services.LoggerFactory.CreateLogger(typeof(ArtifactoryGateway));
            
            RestClientFactory = new ArtifactoryRestClientFactory(Services);

            _gamesRepositoryDescriptors.Add(new ArtifactoryRepositoryDescriptor(Entities.GameTechnology.Flash, false, Entities.PlatformType.PC, new ArtifactoryGamesRepository("modernGame-local", "Games", RestClientFactory)));
            _gamesRepositoryDescriptors.Add(new ArtifactoryRepositoryDescriptor(Entities.GameTechnology.Html5, false, Entities.PlatformType.Both, new ArtifactoryGamesRepository("HTML5Game-local", "Games", RestClientFactory)));
            _gamesRepositoryDescriptors.Add(new ArtifactoryRepositoryDescriptor(Entities.GameTechnology.Html5, false, Entities.PlatformType.Mobile, new ArtifactoryGamesRepository("HTML5Game-local", "Games_MOBILE", RestClientFactory)));
            _gamesRepositoryDescriptors.Add(new ArtifactoryRepositoryDescriptor(Entities.GameTechnology.Html5, false, Entities.PlatformType.PC, new ArtifactoryGamesRepository("HTML5Game-local", "Games_PC", RestClientFactory)));
            _gamesRepositoryDescriptors.Add(new ArtifactoryRepositoryDescriptor(Entities.GameTechnology.Flash, true, Entities.PlatformType.PC, new ArtifactoryGamesRepository("externalGame-local", "Games", RestClientFactory)));
        }

        ILogNotifier Logger { get; set; }

        IArtifactoryRestClientFactory RestClientFactory { get; set; }
        List<ArtifactoryRepositoryDescriptor> _gamesRepositoryDescriptors = new List<ArtifactoryRepositoryDescriptor>();
        
        IGamesPortalInternalServices Services { get; set; }

        IConfigurationReader ConfigurationReader
        {
            get { return this.Services.ConfigurationReader; }
        }

        public void StartSynchronization()
        {
            if (ConfigurationReader.ReadSection<ArtifactorySettings>().SynchronizationEnabled)
            {
                Thread t = new Thread(new ThreadStart(SynchronizeLoop));
                t.IsBackground = true;
                t.Start();
            }
        }

        public void StopSynchronization()
        {
            _stop = true;
            _autoResetEvent.Set();
        }


        public ArtifactoryRepositoryDescriptor GetRepositoryDescriptor(Entities.GameTechnology gameTechnology, Entities.PlatformType platformType, bool isExternal)
        {
            return _gamesRepositoryDescriptors.FirstOrDefault(r => r.Technology == gameTechnology && r.IsExternal == isExternal && r.PlatformType == platformType);
        }
     
        AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private bool _stop = false;

        public PropertySet[] GetAvailablePropertySets()
        {
            var result = new List<PropertySet>();

            try
            {


                var restClient = this.RestClientFactory.CreateAuthenticatedUiApi();

                foreach (var ps in ArtifactoryResponseExtensions.Parse<ArtifactoryListResponse<PropertySet>>(restClient.Get("propertysets")))
                {
                    try
                    {
                        result.Add(ArtifactoryResponseExtensions.Parse<PropertySet>(restClient.Get("propertysets/" + ps.name)));
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(string.Format("Failed to read property set {0} from Artifactory. For this reason I will not be able to validated values for properies in this property set!", ps.name), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception("Failed to read property sets definitions from Artifactory. This means I can't validate the properties values!", ex);
            }
            
            return result.ToArray();
        }


        private void SynchronizeGames(IArtifactorySyncronizationLogger synchronizationLogger, PropertySet[] availablePropertySets)
        {
            var synchronizer = new ArtifactoryGameSynchronizer(Services, synchronizationLogger);

            foreach (IgnoredUndefinedPropertyValueSettings ingnoredPropertiesValue in this.ConfigurationReader.ReadSection<Artifactory.ArtifactorySettings>().IgnoreUndefinedPropertiesValues)
            {
                synchronizer.IgnoredUndefinedPropertyValue(ingnoredPropertiesValue.Key, ingnoredPropertiesValue.Value);
            }

            foreach (var repositoryDescriptor in _gamesRepositoryDescriptors)
            {
                var gamesRepository = (IArtifactoryGamesRepository)repositoryDescriptor.Repository;
                foreach (var gameType in gamesRepository.GetGames())
                {
                   
                    Logger.Info(string.Format("Start synchronizing game type {0} in repository {1}", gameType, gamesRepository.GetRootFolderRelativeUrl()));

                    try
                    {
                        synchronizer.SynchronizeComponent(repositoryDescriptor, gameType);
                        Logger.Info(string.Format("Finish synchronizing game type {0} in repository {1}", gameType, gamesRepository.GetRootFolderRelativeUrl()));
                    }
                    catch(Exception ex)
                    {
                        Logger.Exception(string.Format("Failed to synchronizing game type {0} in repository {1}!", gameType, gamesRepository.GetRootFolderRelativeUrl()), ex);
                    }

                    
                    if (_stop)
                        return;
                }
            }
        }


        private void SynchronizeGamingComponents(IArtifactorySyncronizationLogger synchronizationLogger)
        {
          
            var repositoryDescriptors = new List<ArtifactoryRepositoryDescriptor>();
            using (var dbContext = this.Services.CreateGamesPortalDBDataContext())
            {
                foreach (var row in dbContext.GetTable<GamingComponent>())
                    repositoryDescriptors.Add(new ArtifactoryRepositoryDescriptor((Entities.GameTechnology)row.Technology, false, (Entities.PlatformType)row.PlatformType, new ArtifactoryGamingComponentRepository(row.RepositoryName, RestClientFactory, (GamingComponentType)row.ComponentType)));
            }
            
            if (repositoryDescriptors.Count == 0)
                return;

            var synchronizer = new ArtifactoryGamingComponentSynchronizer(Services, synchronizationLogger);

            foreach (IgnoredUndefinedPropertyValueSettings ingnoredPropertiesValue in this.ConfigurationReader.ReadSection<Artifactory.ArtifactorySettings>().IgnoreUndefinedPropertiesValues)
            {
                synchronizer.IgnoredUndefinedPropertyValue(ingnoredPropertiesValue.Key, ingnoredPropertiesValue.Value);
            }


            foreach (var repositoryDescriptor in repositoryDescriptors)
            {
                if (_stop)
                    return;

                var repo = repositoryDescriptor.Repository as ArtifactoryGamingComponentRepository;
                Logger.Info(string.Format("Start synchronizing {0} in repository {1}", repo.ComponentType, repositoryDescriptor.Repository.GetRootFolderRelativeUrl()));

                synchronizer.SynchronizeComponent(repositoryDescriptor, (int)repo.ComponentType);

                Logger.Info(string.Format("Finish synchronizing {0} in repository {1}", repo.ComponentType, repositoryDescriptor.Repository.GetRootFolderRelativeUrl()));

            }
        }
        
        private void SynchronizeLoop()
        {
            while (!_stop)
            {
                SynchronizationInProgress = true;
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                Logger.Info(string.Format("Artifactory synchronization started at {0}", DateTime.Now));
                var synchronizationLogger = new ArtifactorySyncronizationLogger();

              

                try
                {
                    var availablePropertySets = GetAvailablePropertySets();

                    if (this.ConfigurationReader.ReadSection<ArtifactorySettings>().EnableGamingComponentsSynchronization)
                    {
                        SynchronizeGamingComponents(synchronizationLogger);
                    }

                    if (!_stop)
                    {
                        SynchronizeGames(synchronizationLogger, availablePropertySets);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Exception("Artifactory synchronization failed!", ex);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(synchronizationLogger.Errors))
                    {
                        Logger.Error(synchronizationLogger.Errors);
                    }

                    if (!string.IsNullOrEmpty(synchronizationLogger.Warnings))
                    {
                        Logger.Warning(synchronizationLogger.Warnings);
                    }

                    if (!string.IsNullOrEmpty(synchronizationLogger.Informations))
                    {
                        Logger.Info(synchronizationLogger.Informations);
                    }

                    timer.Stop();
                    Logger.Info(string.Format("Artifactory synchronization finished in {0}", timer.Elapsed.ToString()));


                    string userName = SynchronizationInitiatedBy;
                    SynchronizationInitiatedBy = null;
                    if (string.IsNullOrEmpty(userName))
                        userName = Environment.UserName;

                    Services.GamesPortalHubContext.FullGamesSynchronizationFinished(new SignalR.FullGamesSynchronizationFinishedData(DateTime.Now, userName));

                    SynchronizationInitiatedBy = null;
                    SynchronizationInProgress = false;
                }

                _autoResetEvent.WaitOne(ConfigurationReader.ReadSection<ArtifactorySettings>().SynchronizationInterval);
            }
        }

        public bool SynchronizationInProgress
        {
            get;
            private set;
        }

        private string SynchronizationInitiatedBy { get; set; }

        public void ForceSynchronization(string initiatedBy)
        {
            SynchronizationInitiatedBy = initiatedBy;
            _autoResetEvent.Set();
        }


        private class ArtifactorySyncronizationLogger : IArtifactorySyncronizationLogger
        {

            private StringBuilder _info = new StringBuilder();
            private StringBuilder _warn = new StringBuilder();
            private StringBuilder _errors = new StringBuilder();

            public ArtifactorySyncronizationLogger()
            {
            }

            public string Informations
            {
                get { return _info.ToString(); }
            }

            public string Warnings
            {
                get { return _warn.ToString(); }
            }

            public string Errors
            {
                get { return _errors.ToString(); }
            }

            #region IArtifactorySyncronizationLogger Members

            public void Info(string message)
            {
                _info.Append(message);
                _info.Append(Environment.NewLine);
                _info.Append(Environment.NewLine);
            }

            public void Warn(string message)
            {
                _warn.Append(message);
                _warn.Append(Environment.NewLine);
                _warn.Append(Environment.NewLine);

            }

            public void Error(string message)
            {
                _errors.Append(message);
                _errors.Append(Environment.NewLine);
                _errors.Append(Environment.NewLine);
            }

            #endregion
        }




       

        
    }


}
