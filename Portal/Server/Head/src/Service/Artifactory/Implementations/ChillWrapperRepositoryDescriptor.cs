using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service.Artifactory
{
    public class ChillWrapperRepositoryDescriptor : ArtifactoryRepositoryDescriptor<IChillWrapperRepository>
    {
        public ChillWrapperRepositoryDescriptor(GameInfrastructureDTO infrastructure, IChillWrapperRepository repository)
            : base(infrastructure.GameTechnology, false, infrastructure.PlatformType, repository)

        {
        }


        
        public override GamingComponentCategory ComponentCategory
        {
            get { return this.Repository.ComponentCategory; }
            
        }


    }
}
