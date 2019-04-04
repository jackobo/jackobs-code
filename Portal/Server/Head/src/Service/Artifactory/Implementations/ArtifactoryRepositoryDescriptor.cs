using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service.Artifactory
{
    
   
    public abstract class ArtifactoryRepositoryDescriptor<TRepository> : IArtifactoryRepositoryDescriptor
        where TRepository : IArtifactoryRepository
    {
        public ArtifactoryRepositoryDescriptor(GameTechnology technology, bool isExternal, PlatformType platformType, TRepository repository)
        {
            this.Infrastructure = new GameInfrastructureDTO(technology, platformType);
            
            this.IsExternal = isExternal;
            
            this.Repository = repository;
        }


        public GameInfrastructureDTO Infrastructure { get; private set; }

        
        public TRepository Repository { get; private set; }
        public bool IsExternal { get; private set; }

        IArtifactoryRepository IArtifactoryRepositoryDescriptor.Repository
        {
            get
            {
                return this.Repository;
            }
        }

        public abstract GamingComponentCategory ComponentCategory { get; }


        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj?.GetType()))
                return false;

            var theOther = obj as ArtifactoryRepositoryDescriptor<TRepository>;

            return this.Infrastructure.Equals(theOther.Infrastructure)
                    && this.IsExternal == theOther.IsExternal
                    && this.Repository.GetRootFolderRelativeUrl() == theOther.Repository.GetRootFolderRelativeUrl();


        }

        public override int GetHashCode()
        {
            return this.Infrastructure.GetHashCode()
                    ^ this.IsExternal.GetHashCode()
                    ^ this.Repository.GetRootFolderRelativeUrl().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Repository.GetRootFolderRelativeUrl()}; GameTechnology = {this.Infrastructure.GameTechnology}; PlatformType = {this.Infrastructure.PlatformType}; IsExternal = {IsExternal}";
        }

        
    }
    

}
