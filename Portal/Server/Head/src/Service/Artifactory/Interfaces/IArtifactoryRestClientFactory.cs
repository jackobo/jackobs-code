using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactoryRestClientFactory
    {
        IArtifactoryRestClient CreateUnauthenticatedStorageApi();
        IArtifactoryRestClient CreateAuthenticatedStorageApi();

        IArtifactoryRestClient CreateAuthenticatedUiApi();
    }
}
