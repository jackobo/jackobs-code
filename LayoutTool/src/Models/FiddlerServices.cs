using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fiddler;
using LayoutTool.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Prism.Logging;
using Spark.Infra.Logging;
using Spark.Wpf.Common.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;

namespace LayoutTool.Models
{
    public class FiddlerServices : IFiddlerServices
    {
        private static readonly int FiddlerPort = 8899;

        public FiddlerServices(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            this.Logger = ServiceLocator.GetInstance<ILoggerFactory>().CreateLogger(this.GetType());
            SubscribeToFiddlerEvents();
            Start();
        }

        ILogger Logger { get; set; }
        
        IServiceLocator ServiceLocator { get; set; }

        IApplicationServices ApplicationServices { get; set; }

        public void Start()
        {
            if (!FiddlerApplication.IsStarted())
            {
                SubscribeToApplicationShuttingDown();


                Fiddler.CONFIG.IgnoreServerCertErrors = false;
                Fiddler.CONFIG.bCaptureCONNECT = false;
                Fiddler.CONFIG.bCaptureFTP = false;
                Fiddler.CONFIG.bMITM_HTTPS = false;

                //FiddlerApplication.Prefs.SetInt32Pref("fiddler.network.timeouts.serverpipe.send.initial", -1);

                // Because we've chosen to decrypt HTTPS traffic, makecert.exe must
                // be present in the Application folder.


                var flags = FiddlerCoreStartupFlags.Default
                            & (~FiddlerCoreStartupFlags.DecryptSSL)
                            /*& (~FiddlerCoreStartupFlags.RegisterAsSystemProxy)*/;

                FiddlerApplication.Startup(FiddlerPort, flags);
                //URLMonInterop.SetProxyInProcess("127.0.0.1:" + FiddlerPort, "");
            }
            else
            {
                FiddlerApplication.oProxy.Attach();
            }

            System.Threading.Thread.Sleep(200);

        }


        public void Stop()
        {
            if (FiddlerApplication.IsStarted())
            {
                FiddlerApplication.oProxy.Detach();
                System.Threading.Thread.Sleep(200);
            }
        }

        private void ShutDown()
        {
            if (FiddlerApplication.IsStarted())
            {
                FiddlerApplication.Shutdown();
                System.Threading.Thread.Sleep(200);
            }
        }


        private void SubscribeToApplicationShuttingDown()
        {
            if (this.ApplicationServices != null)
                return;


            var application = this.ServiceLocator.TryResolve<IApplicationServices>();

            if (application == null)
                return;

            this.ApplicationServices = application;
            this.ApplicationServices.ShuttingDown += ApplicationServices_ShuttingDown;
        }

        private void ApplicationServices_ShuttingDown(object sender, EventArgs e)
        {
            ShutDown();
        }

        private void SubscribeToFiddlerEvents()
        {

            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            FiddlerApplication.BeforeResponse += FiddlerApplication_BeforeResponse;
            
        }

        ReaderWriterLockSlim _overridesProvidersLockSlim = new ReaderWriterLockSlim();


        private void FiddlerApplication_BeforeRequest(Session oS)
        {
            try
            {
                ExecuteForEachProvider(provider => BeforeRequestProcessing(provider, oS));
            }
            catch (Exception ex)
            {
                LogHttpCaptureException(ex);
            }
        }

        private void FiddlerApplication_BeforeResponse(Session oS)
        {
            try
            {
                ExecuteForEachProvider(provider => BeforeResponseProcessing(provider, oS));
            }
            catch (Exception ex)
            {
                AppendAntiCacheHeaders(oS);
                LogHttpCaptureException(ex);
            }
        }
        
        private void ExecuteForEachProvider(Action<IFiddlerOverrideProvider> action)
        {
            _overridesProvidersLockSlim.EnterReadLock();
            try
            {
                foreach (var provider in _overridesProviders)
                {
                    action(provider);
                }
            }
            finally
            {
                _overridesProvidersLockSlim.ExitReadLock();
            }
        }

        HashSet<string> _alreadyLoggedExceptions = new HashSet<string>();

      

        private void BeforeRequestProcessing(IFiddlerOverrideProvider provider, Session session)
        {
            var overrideMode = provider.GetOverrideMode(session.fullUrl);
            if (overrideMode == FiddlerOverrideMode.Normal)
            {
                session.bBufferResponse = true;
            }
            else if (overrideMode == FiddlerOverrideMode.BypassServer)
            {
                session.utilCreateResponseAndBypassServer();
            }
        }

        private void BeforeResponseProcessing(IFiddlerOverrideProvider provider, Session session)
        {

            var overrideMode = provider.GetOverrideMode(session.fullUrl);
            if (overrideMode == FiddlerOverrideMode.NoOverride)
                return;

            AppendAntiCacheHeaders(session);

            var overrideContentResponse = provider.GetOverrideContent(session.fullUrl, session.GetResponseBodyAsString());
            if (overrideContentResponse == null || overrideContentResponse.Content == null)
                return;


            foreach (var h in (overrideContentResponse.ExtraHttpHeaders ?? new KeyValuePair<string, string>[0]))
            {
                session.ResponseHeaders.Add(h.Key, h.Value);
            }

            if (overrideMode == FiddlerOverrideMode.HeadersOnly)
                return;


            if (overrideContentResponse.Content.GetType() == typeof(byte[]))
            {
                var contentBytes = (byte[])overrideContentResponse.Content;

                using (var memS = new MemoryStream(contentBytes))
                {
                    session.LoadResponseFromStream(memS, string.Empty);
                }
            }
            else
            {
                session.utilSetResponseBody(overrideContentResponse.Content.ToString());
            }

        }

        private static void AppendAntiCacheHeaders(Session oS)
        {
            oS.ResponseHeaders.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            oS.ResponseHeaders.Add("max-age", "0");
            oS.ResponseHeaders.Add("Pragma", "no-cache");
            oS.ResponseHeaders.Add("Expires", "0");
        }


        
        private List<IFiddlerOverrideProvider> _overridesProviders = new List<IFiddlerOverrideProvider>();
        
        public void RegisterFilesOverrideProvider(IFiddlerOverrideProvider provider)
        {
            ServiceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(() =>
            {
                _overridesProvidersLockSlim.EnterWriteLock();
                try
                {
                    _overridesProviders.Add(provider);
                }
                finally
                {
                    _overridesProvidersLockSlim.ExitWriteLock();
                }
            });
        }

        public void UnregisterFilesOverrideProvider(IFiddlerOverrideProvider provider)
        {
            ServiceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(() =>
            {
                _overridesProvidersLockSlim.EnterWriteLock();
                try
                {
                    _overridesProviders.Remove(provider);
                }
                finally
                {
                    _overridesProvidersLockSlim.ExitWriteLock();
                }
            });
        }



        private void LogHttpCaptureException(Exception exception, [CallerMemberName]string operationName = "")
        {
            var exceptionText = exception.ToString();



            if (!_alreadyLoggedExceptions.Contains(exceptionText))
            {

                try
                {
                    _alreadyLoggedExceptions.Add(exceptionText);
                }
                catch (Exception ex)
                {
                    Logger.Exception($"Failed to add the exception text to the {nameof(_alreadyLoggedExceptions)}!", ex);
                }

                Logger.Exception($"{nameof(FiddlerServices)}.{operationName} failed!", exception);
            }



        }

     
       
    }
}
