using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service.Artifactory
{
    public class GamesRepositoryDescriptor : ArtifactoryRepositoryDescriptor<IGamesRepository>
    {
        public GamesRepositoryDescriptor(GameTechnology technology, bool isExternal, PlatformType platformType, IGamesRepository repository)
            : base(technology, isExternal, platformType, repository)

        {

        }

        public override GamingComponentCategory ComponentCategory
        {
            get
            {
                return GamingComponentCategory.Game;
            }
        }

    }
}
