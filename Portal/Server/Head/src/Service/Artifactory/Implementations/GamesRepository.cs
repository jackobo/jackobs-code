using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.Artifactory
{
    public class GamesRepository : ArtifactoryRepository, IGamesRepository
    {
        public GamesRepository(string repositoryName, string gamesFolderPath, IArtifactoryRestClientFactory restClientFactory)
            : base(repositoryName, restClientFactory)
        {
            if (string.IsNullOrEmpty(gamesFolderPath))
                throw new ArgumentNullException($"{nameof(gamesFolderPath)} can't be null or empty");

            GamesFolderPath = gamesFolderPath;
        }
        public override int[] GetGames()
        {
            var content = CreateUnauthenticatedRestClient().Get(GetRootFolderRelativeUrl());

            var storageItem = ParseArtifactoryResponse<ArtifactoryStorageItem>(content);

            var gameTypes = new List<int>();

            foreach (var ch in storageItem.children)
            {
                int gt;
                if (int.TryParse(ch.GetUriValue(), out gt))
                {
                    gameTypes.Add(gt);
                }
            }

            return gameTypes.ToArray();
        }

        public string GamesFolderPath { get; private set; }

        public override string GetRootFolderRelativeUrl()
        {
            return string.Format("{0}/{1}", RepositoryName, GamesFolderPath);
        }


        protected override string GetComponentFolderRelativeUrl(int componentId)
        {
            return string.Format("{0}/{1}", GetRootFolderRelativeUrl(), componentId);
        }

    }
}
