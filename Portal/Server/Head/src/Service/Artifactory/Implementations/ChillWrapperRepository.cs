using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service.Artifactory
{
    public class ChillWrapperRepository : ArtifactoryRepository, IChillWrapperRepository
    {
        public ChillWrapperRepository(string repositoryName, string folder, IArtifactoryRestClientFactory restClientFactory, GamingComponentCategory componentType)
            : base(repositoryName, restClientFactory)
        {
            this.ComponentCategory = componentType;
            _folder = folder;
        }


        string _folder;
        public GamingComponentCategory ComponentCategory { get; private set; }

        public override string GetRootFolderRelativeUrl()
        {
            return string.Format("{0}/{1}", this.RepositoryName, GetComponentTypeFolder());
        }

        public override int[] GetGames()
        {
            return new int[] { (int)this.ComponentCategory };
        }

        private string GetComponentTypeFolder()
        {
            return _folder;
            /*
            switch (this.ComponentType)
            {
                case GamingComponentType.Wrapper:
                    return "Wrapper";
                case GamingComponentType.Chill:
                    return "Wrapper/chill";
                default:
                    throw new ArgumentException(string.Format("Unknown gaming component type {0}", this.ComponentType));

            }
            */
        }

        protected override string GetComponentFolderRelativeUrl(int componentId)
        {
            if (componentId != (int)this.ComponentCategory)
            {
                throw new ArgumentException(string.Format("Unexpected componentId!  Expected {0} but was {1}", (int)this.ComponentCategory, componentId));
            }

            return GetRootFolderRelativeUrl();
        }
    }
}
