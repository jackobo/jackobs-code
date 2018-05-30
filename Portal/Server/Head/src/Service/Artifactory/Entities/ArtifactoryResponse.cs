using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactoryResponse
    {
        ArtifactoryErrorCollection errors { get; set; }
    }

    public class ArtifactoryResponse : IArtifactoryResponse
    {
        public ArtifactoryResponse()
        {
            this.errors = new ArtifactoryErrorCollection();
        }

        public ArtifactoryErrorCollection errors { get; set; }


    }

    public class ArtifactoryErrorCollection : List<ArtifactoryError>
    {
        public ArtifactoryErrorCollection()
        {

        }

        public ArtifactoryErrorCollection(IEnumerable<ArtifactoryError> errors)
            : base(errors)
        {

        }
        public bool AllNotFound()
        {
            return this.All(er => er.status == WellKnownErrorCodes.NotFound);
        }
    }


    public class ArtifactoryListResponse<TItem> : List<TItem>, IArtifactoryResponse
    {
        public ArtifactoryListResponse()
        {
            this.errors = new ArtifactoryErrorCollection();
        }

        public ArtifactoryErrorCollection errors { get; set; }
    }

    public class ArtifactoryError
    {
        public ArtifactoryError()
        {

        }

        public ArtifactoryError(string errorCode, string errorMessage)
        {
            this.status = errorCode;
            this.message = errorMessage;
        }

        public string status { get; set; }
        public string message { get; set; }
    }

    public static class ArtifactoryResponseExtensions
    {
        public static TResponse TryParse<TResponse>(string content) where TResponse : class, IArtifactoryResponse
        {

            try
            {
                return JsonConvert.DeserializeObject<TResponse>(content);
            }
            catch
            {
                return null;
            }
        }

        public static TResponse ParseObject<TResponse>(string content) where TResponse : class, IArtifactoryResponse
        {
            var response = JsonConvert.DeserializeObject<TResponse>(content);

            if (response.errors.Count > 0)
            {
                throw new ArtifactoryException(response.errors);
            }

            return response;

        }

        public static ArtifactoryListResponse<TResponse> ParseArray<TResponse>(string content) where TResponse : class, IArtifactoryResponse
        {
            
            var errorResponse = TryParse<ArtifactoryResponse>(content);

            if(errorResponse != null && errorResponse.errors.Count > 0)
            {
                throw new ArtifactoryException(errorResponse.errors);
            }
            

            var response = JsonConvert.DeserializeObject<ArtifactoryListResponse<TResponse>>(content);

            if (response.errors.Count > 0)
            {
                throw new ArtifactoryException(response.errors);
            }

            return response;

        }

        public static void ThrowIfError(this IArtifactoryResponse response, string additionalInfo)
        {
            
            if (response != null && response.errors.Count > 0)
            {
                throw new ArtifactoryException(string.Format("{0};{1}",
                                               string.Join("; ", response.errors.Select(er => string.Format("{0}: {1}", er.status, er.message))),
                                               additionalInfo));
            }


        }
    }

}
