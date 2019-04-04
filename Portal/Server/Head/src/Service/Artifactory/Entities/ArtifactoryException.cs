using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service.Artifactory
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

        public ArtifactoryException(IEnumerable<ArtifactoryError> errors)
            : base(string.Join(Environment.NewLine, errors.Select(er => string.Format("{0}: {1}", er.status, er.message))))
        {
            this.Errors = errors.ToArray();
        }


        public ArtifactoryError[] Errors { get; private set; } = new ArtifactoryError[0];

    }
}
