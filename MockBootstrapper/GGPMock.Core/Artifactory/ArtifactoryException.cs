using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Artifactory
{
    public class ArtifactoryException : ApplicationException
    {
        public ArtifactoryException(string message)
            : base(message)
        {

        }


        public ArtifactoryException(string message, Exception innerException)
            : base(message, innerException)
        {

        }


    }
}
