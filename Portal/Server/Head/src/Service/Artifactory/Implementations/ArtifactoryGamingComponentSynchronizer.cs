using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;

namespace GamesPortal.Service.Artifactory
{
    public class ArtifactoryGamingComponentSynchronizer : ArtifactorySynchronizer<IGamesPortalDataContext, GamingComponent, GamingComponentVersion, GamingComponentVersion_Property, GamingComponentVersion_Regulation, GamingComponentVersion_Property_History>
    {
        public ArtifactoryGamingComponentSynchronizer(IGamesPortalInternalServices services, IArtifactorySyncronizationLogger logger)
            : base(services, logger)
        {
            
        }
        

        protected override IGamesPortalDataContext CreateDataAccessLayer()
        {
            return Services.CreateGamesPortalDBDataContext();
        }

        protected override GamingComponent FindComponent(IGamesPortalDataContext dal, int componentID, bool isExternal)
        {
            return dal.GetTable<GamingComponent>().FirstOrDefault(row => row.ComponentType == componentID);
        }

        protected override GamingComponent CreateNewComponent(IGamesPortalDataContext dal, ArtifactoryRepositoryDescriptor repositoryDescriptor, int componentID)
        {
            throw new InvalidOperationException("Gaming components cannot be automatically added");
        }

        protected override void SubmitChanges(IGamesPortalDataContext dal, GamingComponent component, bool isNew)
        {
            dal.SubmitChanges();
        }

        protected override void UpdateComponentProperties(IGamesPortalDataContext dal, GamingComponent component, int componentId)
        {
            //nothing to do here
        }

        protected override string GetComponentTypeForLogging(int componentId)
        {
            return "GamingComponentType = " + (GamingComponentType)componentId;
        }
    }
}
