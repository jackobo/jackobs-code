using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service
{
    [ServiceContract]
    public interface ILayoutToolPublisher
    {
        [OperationContract]
        void PublishSkinForQA(PublishSkinToQARequest request);
        [OperationContract]
        void PublishSkinForProduction(PublishSkinToProductionRequest request);

        [OperationContract]
        GetCurrentProductionNavigationPlanResponse GetCurrentProductionNavigationPlan(GetCurrentProductionNavigationPlanRequest request);
        
    }

    [DataContract]
    public class GetCurrentProductionNavigationPlanRequest
    {
        [DataMember]
        public int BrandId { get; set; }
        [DataMember]
        public int SkinId { get; set; }
        [DataMember]
        public string ClientVersion { get; set; }
        [DataMember]
        public string ClientVersionJobNumber { get; set; }
    }

    [DataContract]
    public class GetCurrentProductionNavigationPlanResponse
    {
        public GetCurrentProductionNavigationPlanResponse()
        {
        }

        public GetCurrentProductionNavigationPlanResponse(string navigationPlanContent)
        {
            this.NavigationPlanContent = navigationPlanContent;
        }
        
        [DataMember]
        public string NavigationPlanContent { get; set; }
    }

    [DataContract]
    public abstract class PublishSkinRequest
    {
        [DataMember]
        public int BrandId { get; set; }
        [DataMember]
        public int SkinId { get; set; }
        [DataMember]
        public string ClientVersion { get; set; }
        [DataMember]
        public string SkinContent { get; set; }
        [DataMember]
        public bool HasWarnings { get; set; }

    }

    [DataContract]
    public class PublishSkinToQARequest : PublishSkinRequest
    {
        [DataMember]
        public string Environment { get; set; }
    }

    [DataContract]
    public class PublishSkinToProductionRequest : PublishSkinRequest
    {
        [DataMember]
        public string NavigationPlanContent { get; set; }
    }
}
