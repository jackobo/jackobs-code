
namespace LayoutTool.Models
{

    public interface IWcfServiceFactory
    {
        IDisposableLayoutToolService CreateLayoutToolService();
        IDisposableLayoutToolPublisherService CreateLayoutPublisherService();
    }

    public class WcfServiceFactory : IWcfServiceFactory
    {
        public IDisposableLayoutToolService CreateLayoutToolService()
        {
            return new LayoutToolServiceClientWrapper();
        }

        public IDisposableLayoutToolPublisherService CreateLayoutPublisherService()
        {
            return new LayoutToolPublisherServiceClientWrapper();
        }
    }
    
}
