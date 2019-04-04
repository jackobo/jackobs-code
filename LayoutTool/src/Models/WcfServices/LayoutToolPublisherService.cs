using System;
using LayoutTool.Models.LayoutToolPublisherService;

namespace LayoutTool.Models
{
    public interface IDisposableLayoutToolPublisherService : ILayoutToolPublisher, IDisposable
    {

    }

    internal class LayoutToolPublisherServiceClientWrapper : WcfServiceWrapperBase<LayoutToolPublisherClient>, IDisposableLayoutToolPublisherService
    {
        public GetCurrentProductionNavigationPlanResponse GetCurrentProductionNavigationPlan(GetCurrentProductionNavigationPlanRequest request)
        {
            return Proxy.GetCurrentProductionNavigationPlan(request);
        }

        public void PublishSkinForProduction(PublishSkinToProductionRequest request)
        {
            Proxy.PublishSkinForProduction(request);
        }

        public void PublishSkinForQA(PublishSkinToQARequest request)
        {
            Proxy.PublishSkinForQA(request);
        }
    }
}
