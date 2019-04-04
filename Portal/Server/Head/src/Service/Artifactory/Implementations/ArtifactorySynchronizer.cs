using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using GGPGameServer.ApprovalSystem.Common;

namespace GamesPortal.Service.Artifactory
{
        
    public abstract class ArtifactorySynchronizer<TDataAccessLayer, TComponent, TComponentVersion, TComponentVersionProperty, TComponentVersionDownloadUri, TComponentVersionPropertyHistory>
        where TDataAccessLayer : IDisposable
        where TComponent : class, IComponent<TComponentVersion>
        where TComponentVersion : class, IComponentVersion<TComponentVersionProperty, TComponentVersionDownloadUri, TComponentVersionPropertyHistory>, new()
        where TComponentVersionProperty : class, IComponentVersionProperty, new()
        where TComponentVersionDownloadUri : class, IComponentVersionRegulation , new()
        where TComponentVersionPropertyHistory : class

    {

        public ArtifactorySynchronizer(IGamesPortalInternalServices services, IArtifactorySyncronizationLogger logger)
        {
            Services = services;
            Logger = logger;
            _artifactoryPropertySets =  (Services.ArtifactoryGateway.GetAvailablePropertySets() ?? new PropertySet[0]).ToDictionary(ps => ps.name);
        }

        protected IGamesPortalInternalServices Services { get; private set; }
        protected IArtifactorySyncronizationLogger Logger { get; private set; }

        protected abstract TDataAccessLayer CreateDataAccessLayer();
        protected abstract TComponent FindComponent(TDataAccessLayer dal, int componentID, bool isExternal);
        protected abstract TComponent CreateNewComponent(TDataAccessLayer dal, ArtifactoryRepositoryDescriptor repositoryDescriptor, int componentID);
        protected abstract void SubmitChanges(TDataAccessLayer dal, TComponent component, bool isNew);
        protected abstract void UpdateComponentProperties(TDataAccessLayer dal, TComponent component, int componentId);


        Dictionary<string, PropertySet> _artifactoryPropertySets = new Dictionary<string, PropertySet>();
        private Dictionary<string, List<string>> _ignoredUndefinedPropertiesValues = new Dictionary<string, List<string>>();

        public void IgnoredUndefinedPropertyValue(string propertyKey, string propertyValue)
        {
            if (!_ignoredUndefinedPropertiesValues.ContainsKey(propertyKey))
                _ignoredUndefinedPropertiesValues.Add(propertyKey, new List<string>());

            _ignoredUndefinedPropertiesValues[propertyKey].Add(propertyValue);
        }

        

        public void SynchronizeComponent(ArtifactoryRepositoryDescriptor repositoryDescriptor, int componentId)
        {

            using (var dataAccessLayer = CreateDataAccessLayer())
            {
                var component = FindComponent(dataAccessLayer, componentId, repositoryDescriptor.IsExternal);

                bool isNew = false;

                if (component == null)
                {
                    isNew = true;
                    component = CreateNewComponent(dataAccessLayer, repositoryDescriptor, componentId);
                }

                UpdateComponentProperties(dataAccessLayer, component, componentId);

                OnBeforeSynchronizeVersions(dataAccessLayer, componentId, component);

                SynchronizeComponentVersions(repositoryDescriptor, componentId, component);

                SubmitChanges(dataAccessLayer, component, isNew);
            }
        }

        protected virtual void OnBeforeSynchronizeVersions(TDataAccessLayer dal, int componentId, TComponent component)
        {

        }


        private void SynchronizeComponentVersions(ArtifactoryRepositoryDescriptor repositoryDescriptor, int componentId, TComponent component)
        {
            var repository = repositoryDescriptor.Repository;
            var availableVersions = repository.GetComponentRegulations(componentId)
                                                                    .ToEmptyArrayIfNull()
                                                                    .SelectMany(r => repository.GetVersionFolders(componentId, r)
                                                                                               .ToEmptyArrayIfNull()
                                                                                               .Select(v => new { Regulation = r, VersionFolder = v }))
                                                                    .GroupBy(item => item.VersionFolder)
                                                                    .Select(g => new
                                                                    {
                                                                        VersionFolderName = g.Key,
                                                                        ArtifactsPerRegulation = g.Select(vr => new KeyValuePair<string, Artifact>(vr.Regulation,
                                                                                                                                                   repository.GetArtifact(componentId, vr.Regulation, g.Key)))
                                                                                                  .Where(a => a.Value != null)
                                                                                                  .ToArray()
                                                                    })
                                                                    .ToArray();

            
            foreach (var removedVersion in component.Versions.Where(gv =>  gv.Technology == (int)repositoryDescriptor.Technology
                                                                        && gv.PlatformType == (int)repositoryDescriptor.PlatformType
                                                                        && !availableVersions.Select(av => av.VersionFolderName).Contains(gv.VersionFolder)).ToArray())
            {
                if (!removedVersion.ComponentVersionPropertyHistories.Any())
                {
                    component.Versions.Remove(removedVersion);
                }
            }
            

            foreach (var versionToUpdate in (from existingVersion in component.Versions
                                             join newVersion in availableVersions on existingVersion.VersionFolder equals newVersion.VersionFolderName
                                             where existingVersion.Technology == (int)repositoryDescriptor.Technology && existingVersion.PlatformType == (int)repositoryDescriptor.PlatformType
                                             select new { existingVersion, newVersion })
                                           .ToArray())
            {


                if (versionToUpdate.newVersion.ArtifactsPerRegulation.Length > 0)
                {
                    UpdateComponentVersionFromArtifacts(versionToUpdate.existingVersion, versionToUpdate.newVersion.ArtifactsPerRegulation, repositoryDescriptor, componentId);
                    versionToUpdate.existingVersion.VersionAsLong = ExtractLongVersion(versionToUpdate.newVersion.VersionFolderName, versionToUpdate.newVersion.ArtifactsPerRegulation, componentId, repositoryDescriptor);
                    versionToUpdate.existingVersion.PlatformType = (int)repositoryDescriptor.PlatformType;
                    UpdateComponentVersionDownloadUriFromArtifacts(versionToUpdate.existingVersion, versionToUpdate.newVersion.ArtifactsPerRegulation);
                }
                else
                {
                    component.Versions.Remove(versionToUpdate.existingVersion);
                }

            }


            foreach (var newVersion in availableVersions.Where(av => !component.Versions.Where(gv => gv.Technology == (int)repositoryDescriptor.Technology && gv.PlatformType == (int)repositoryDescriptor.PlatformType)                                                                                                    
                                                                                       .Select(gv => gv.VersionFolder)
                                                                                       .Contains(av.VersionFolderName)))
            {
                if (newVersion.ArtifactsPerRegulation.Length == 0)
                    continue;

                var componentVersion = new TComponentVersion()
                {
                    ComponentVersion_ID = Guid.NewGuid(),
                    Technology = (int)repositoryDescriptor.Technology,
                    PlatformType = (int)repositoryDescriptor.PlatformType,
                    VersionFolder = newVersion.VersionFolderName,
                    VersionAsLong = ExtractLongVersion(newVersion.VersionFolderName, newVersion.ArtifactsPerRegulation, componentId, repositoryDescriptor)
                };


                UpdateComponentVersionFromArtifacts(componentVersion, newVersion.ArtifactsPerRegulation, repositoryDescriptor, componentId);

                component.Versions.Add(componentVersion);

                UpdateComponentVersionDownloadUriFromArtifacts(componentVersion, newVersion.ArtifactsPerRegulation);

            }





        }


        protected virtual void UpdateComponentVersionFromArtifacts(TComponentVersion componentVersion, KeyValuePair<string, Artifact>[] artifactsPerRegulation, ArtifactoryRepositoryDescriptor repositoryDescriptor, int componentId)
        {
            var latestArtifact = artifactsPerRegulation.OrderByDescending(a => a.Value.created).Select(a => a.Value).FirstOrDefault();
            componentVersion.CreatedBy = latestArtifact.createdBy;
            componentVersion.CreatedDate = latestArtifact.created;
            componentVersion.TriggeredBy = latestArtifact.TriggeredBy;


            var allProperties = artifactsPerRegulation.SelectMany(apr => apr.Value.Properties.Select(p => new { UniquePropertyID = p.BuildUniqueID(apr.Key), Regulation = apr.Key, Artifact = apr.Value, Property = p }));


            foreach (var newProperty in allProperties.Where(prop => !componentVersion.ComponentVersionProperties.Select(gvp => ArtifactoryProperty.BuildUniqueID(gvp.Regulation, gvp.PropertyKey))
                                                                                                      .Contains(prop.UniquePropertyID))
                                                    .ToArray())
            {
                var componentVersionPropertyRecord = new TComponentVersionProperty() { Row_ID = Guid.NewGuid(), ComponentVersion_ID = componentVersion.ComponentVersion_ID };

                UpdateComponentVersionPropertyRecord(componentVersionPropertyRecord, newProperty.Artifact, newProperty.Property, newProperty.Regulation, repositoryDescriptor, componentId, componentVersion.VersionFolder);

                componentVersion.ComponentVersionProperties.Add(componentVersionPropertyRecord);

            }


            foreach (var removedProperty in componentVersion.ComponentVersionProperties.Where(gvp => !allProperties.Select(prop => prop.UniquePropertyID)
                                                                                                          .Contains(ArtifactoryProperty.BuildUniqueID(gvp.Regulation, gvp.PropertyKey)))
                                                   .ToArray())
            {
                componentVersion.ComponentVersionProperties.Remove(removedProperty);
            }


            foreach (var propertyToUpdate in (from existingRecord in componentVersion.ComponentVersionProperties
                                              join newValues in allProperties on ArtifactoryProperty.BuildUniqueID(existingRecord.Regulation, existingRecord.PropertyKey) equals newValues.UniquePropertyID
                                              select new { existingRecord, newValues })
                                             .ToArray())
            {
                UpdateComponentVersionPropertyRecord(propertyToUpdate.existingRecord, propertyToUpdate.newValues.Artifact, propertyToUpdate.newValues.Property, propertyToUpdate.newValues.Regulation, repositoryDescriptor, componentId, componentVersion.VersionFolder);
            }

        }

        private long ExtractLongVersion(string versionFolderName, KeyValuePair<string, Artifact>[] regulationsWithArtifacts, int componentId, ArtifactoryRepositoryDescriptor repositoryDescriptor)
        {
            var versionsFromVersionProperty = regulationsWithArtifacts.Select(s => s.Value.ExtractVersionFromVersionProperty())
                                                                     .Where(v => v != null)
                                                                     .Distinct()
                                                                     .ToArray();

            long versionAsLong = VersionNumber.Parse(versionFolderName).ToLong();
            if (versionsFromVersionProperty.Length == 1)
            {
                versionAsLong = versionsFromVersionProperty[0].ToLong();
            }
            else if (versionsFromVersionProperty.Length == 0)
            {
                Logger.Info(string.Format("Missing version property! Repository = {0}; {1}; Regulation = {2}; VersionFolder = {3}",
                                                 repositoryDescriptor.Repository.GetRootFolderRelativeUrl(),
                                                 GetComponentTypeForLogging(componentId),
                                                 string.Join(", ", regulationsWithArtifacts.Select(r => r.Key)),
                                                 versionFolderName
                                                 ));
            }
            else if (versionsFromVersionProperty.Length > 1)
            {
                string versionsPropertiesPerRegulations = string.Join("; ", regulationsWithArtifacts.GroupBy(a => a.Value.ExtractVersionFromVersionProperty())
                                                                                .Where(g => g.Key != null)
                                                                                .Select(g => string.Format("{0} for {1}", g.Key, string.Join(" & ", g.Select(r => r.Key)))));

                Logger.Info(string.Format("Version property has different values for different regulations! Repository = {0}; {1}; VersionFolder = {2}; {3}",
                                                   repositoryDescriptor.Repository.GetRootFolderRelativeUrl(),
                                                   GetComponentTypeForLogging(componentId),
                                                   versionFolderName,
                                                   versionsPropertiesPerRegulations));
            }

            return versionAsLong;
        }

        protected abstract string GetComponentTypeForLogging(int componentId);
        

        private void UpdateComponentVersionDownloadUriFromArtifacts(TComponentVersion componentVersion, KeyValuePair<string, Artifact>[] artifactsPerRegulation)
        {


            foreach (var newUrl in artifactsPerRegulation.Where(ar => !componentVersion.ComponentVersionRegulation.Select(uri => uri.Regulation).Contains(ar.Key)).ToArray())
            {
                var downloadUriRecord = new TComponentVersionDownloadUri();
                UpdateComponentVersionDownloadUriRecordFromArtifact(downloadUriRecord, newUrl.Value);
                downloadUriRecord.ComponentVersion_ID = componentVersion.ComponentVersion_ID;
                downloadUriRecord.Row_ID = Guid.NewGuid();
                downloadUriRecord.Regulation = newUrl.Key;

                componentVersion.ComponentVersionRegulation.Add(downloadUriRecord);
            }


            foreach (var removedUrl in componentVersion.ComponentVersionRegulation.Where(row => !artifactsPerRegulation.Select(ar => ar.Key).Contains(row.Regulation)).ToArray())
            {
                componentVersion.ComponentVersionRegulation.Remove(removedUrl);
            }

            foreach (var item in (from existingUrl in componentVersion.ComponentVersionRegulation
                                  join newUrl in artifactsPerRegulation on existingUrl.Regulation equals newUrl.Key
                                  select new { existingUrl, newUrl }))
            {
                UpdateComponentVersionDownloadUriRecordFromArtifact(item.existingUrl, item.newUrl.Value);
            }

        }

        private void UpdateComponentVersionDownloadUriRecordFromArtifact(TComponentVersionDownloadUri record, Artifact artifact)
        {
            if (!string.IsNullOrEmpty(artifact.downloadUri))
            {
                record.DownloadUri = artifact.downloadUri;
                record.FileName = artifact.ExtractFileName();
            }
            else
            {
                record.DownloadUri = "";
                record.FileName = "";
                Logger.Warn("There is one artifact that has no download URI! Check GameVersion_Regulation table in the database!");
            }

            record.FileSize = artifact.size;
            if (artifact.checksums != null)
            {
                record.MD5 = artifact.checksums.md5;
                record.SHA1 = artifact.checksums.sha1;
            }
        }


        private void UpdateComponentVersionPropertyRecord(TComponentVersionProperty componnentVersionPropertyRecord, Artifact artifact, ArtifactoryProperty property, string regulation, ArtifactoryRepositoryDescriptor repositoryDescriptor, int componentId, string versionFolder)
        {
            CheckPropertyValues(property, repositoryDescriptor, componentId, regulation, versionFolder);

            componnentVersionPropertyRecord.ChangedBy = artifact.modifiedBy;
            componnentVersionPropertyRecord.LastChange = artifact.lastModified;
            componnentVersionPropertyRecord.PropertyKey = property.Key;
            componnentVersionPropertyRecord.PropertyName = property.Name;
            componnentVersionPropertyRecord.PropertySet = property.SetName;
            componnentVersionPropertyRecord.PropertyValue = property.ConcatValues();
            componnentVersionPropertyRecord.Regulation = regulation;

        }

        private void CheckPropertyValues(ArtifactoryProperty property, ArtifactoryRepositoryDescriptor repositoryDescriptor, int componentId, string regulation, string versionFolderName)
        {
            if (!property.HasPropertySet)
                return;

            if (!_artifactoryPropertySets.ContainsKey(property.SetName))
                return;



            var propertySet = _artifactoryPropertySets[property.SetName];

            var propertyDefinition = propertySet.properties.FirstOrDefault(p => p.name == property.Name);

            if (propertyDefinition == null)
                return;

            if (propertyDefinition.propertyType == PropertyType.ANY_VALUE)
                return;

            var undefinedValues = property.Values.Where(v => !propertyDefinition.predefinedValues.Select(pv => pv.value).Contains(v)
                                                             && (_ignoredUndefinedPropertiesValues.ContainsKey(property.Key) ? !_ignoredUndefinedPropertiesValues[property.Key].Contains(v) : true))
                                                 .ToArray();




            if (undefinedValues.Length > 0)
            {
                Logger.Warn(string.Format("Undefined values ({0}) detected for property {1}! Repository = {2}; {3}; Regulation = {4}; VersionFolder = {5}",
                                                 string.Join(", ", undefinedValues),
                                                 property.Key,
                                                 repositoryDescriptor.Repository.GetRootFolderRelativeUrl(),
                                                 GetComponentTypeForLogging(componentId),
                                                 regulation,
                                                 versionFolderName));

            }
        }
    }
}
