using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Models
{
    public class WcfServiceWrapperBase<T> : IDisposable
        where T : ICommunicationObject, IDisposable, new()
    {
        public WcfServiceWrapperBase()
        {
            Proxy = new T();
        }

        protected T Proxy { get; private set; }

        public void Dispose()
        {
            DisposeTheProxy();
        }

        private void DisposeTheProxy()
        {
            try
            {
                ((IDisposable)Proxy).Dispose();
            }
            catch
            {
                try
                {
                    Proxy.Abort();
                }
                catch
                {

                }
            }
        }
    }
}
