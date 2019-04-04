using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactoryRestClient
    {
        string Get(string relativeUri);
        string Put(string relativeUri);

        string Delete(string p);
    }

}
