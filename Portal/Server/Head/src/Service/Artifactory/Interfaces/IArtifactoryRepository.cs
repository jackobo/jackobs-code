using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Infra;
using Spark.Infra.Types;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactoryRepository
    {
        string RepositoryName { get; }
        string[] GetComponentRegulations(int componentId);
        string[] GetVersionFolders(int componentId, string regulation);
        Optional<Artifact> GetArtifact(int componentId, string regulation, string version);
        void UpdateArtifactProperties(UpdateArtifactPropertiesRequest request);

        void DeleteArtifactProperties(DeleteArtifactPropertiesRequest request);

        string GetRootFolderRelativeUrl();
        int[] GetGames();
    }

    public class UpdateArtifactPropertiesRequest
    {
        public UpdateArtifactPropertiesRequest(int componentId, string regulation, string version, params ArtifactoryProperty[] properties)
        {
            ComponentId = componentId;
            Regulation = regulation;
            Version = version;
            Properties = properties;
        }

        public int ComponentId { get; private set; }
        public string Regulation { get; private set; }
        public string Version { get; private set; }
        public ArtifactoryProperty[] Properties { get; private set; }
    }

    public class DeleteArtifactPropertiesRequest
    {
        public DeleteArtifactPropertiesRequest(int componentId, string regulation, string version, params string[] properties)
        {
            ComponentId = componentId;
            Regulation = regulation;
            Version = version;
            Properties = properties;
        }

        public int ComponentId { get; private set; }
        public string Regulation { get; private set; }
        public string Version { get; private set; }
        public string[] Properties { get; private set; }
    }

   

    
}
