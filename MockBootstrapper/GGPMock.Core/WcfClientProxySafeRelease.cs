using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace GGPMockBootstrapper
{

    public interface IWcfSafeRelease<TChannel> : IDisposable
        where TChannel : class
    {
        TChannel Chanel { get; }
    }

    public class WcfClientProxySafeRelease<T, TChannel> : IWcfSafeRelease<TChannel>
        where TChannel : class
        where T : ClientBase<TChannel>, TChannel, IDisposable
    {
        T _ProxyClient = default(T);


        public WcfClientProxySafeRelease(T proxy)
        {
            _ProxyClient = proxy;
        }

        public T Client
        {
            get { return _ProxyClient; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ProxyClient != null)
                {
                    try
                    {
                        if (_ProxyClient.State == CommunicationState.Faulted)
                        {
                            _ProxyClient.Abort();
                        }
                        else
                        {
                            _ProxyClient.Close();
                        }

                    }
                    catch
                    {
                        try
                        {
                            _ProxyClient.Abort();
                        }
                        catch { }
                    }
                }
            }
        }

        #region IWcfSafeRelease<TChannel> Members

        TChannel IWcfSafeRelease<TChannel>.Chanel
        {
            get { return _ProxyClient; }
        }

        #endregion


    }
}
