using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spark.Infra.Types;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.MainProxyDataControlService;
using Prism.Logging;
using Spark.Wpf.Common.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Infra.Logging;

namespace LayoutTool.MainProxy
{
    internal class MainProxyMockAdapter : Interfaces.IMainProxyAdapter
    {
        
        public MainProxyMockAdapter(IApplicationServices applicationServices, 
                                    ILoggerFactory loggerFactory, 
                                    IJackpotInfoProvider jackpotInfoProvider)
        {
            Port = 8500; //default port;

            _logger = loggerFactory.CreateLogger(this.GetType());
            _jackpotInfoProvider = jackpotInfoProvider;

            applicationServices.ShuttingDown += ApplicationServices_ShuttingDown;
            applicationServices.StartNewParallelTask(ReadServerInfoLoop);

            applicationServices.StartNewParallelTask(SetJackpotIds);
        }

        private void SetJackpotIds()
        {
            try
            {
                using (var proxy = new DataControlServiceClient())
                {
                    proxy.SetJackpotInfo(new SetJackpotInfoRequest() { JackpotIds = _jackpotInfoProvider.GetAllJackpotIds() });
                }
            }
            catch(Exception ex)
            {
                _logger.Exception($"{nameof(MainProxyMockAdapter)}.{nameof(MainProxyMockAdapter.SetJackpotIds)} failed!", 
                                  ex);
            }
        }

        IJackpotInfoProvider _jackpotInfoProvider;

        private void ApplicationServices_ShuttingDown(object sender, EventArgs e)
        {
            _stop = true;
            _readServerInfoLoopAutoResetEvent.Set();
        }

        public int Port
        {
            get; private set;
        }

        ILogger _logger;

        private bool _stop = false;

        AutoResetEvent _readServerInfoLoopAutoResetEvent = new AutoResetEvent(false);
        private void ReadServerInfoLoop()
        {
            while (!_stop)
            {
                try
                {
                    using (var proxy = new DataControlServiceClient())
                    {
                        Port = proxy.GetServerInfo().Port;
                    }
                }
                catch(Exception ex)
                {
                    _logger.Exception($"{nameof(MainProxyMockAdapter)}.{nameof(ReadServerInfoLoop)} failed!",
                                      ex);
                }

                _readServerInfoLoopAutoResetEvent.WaitOne(TimeSpan.FromMinutes(1));
                
            }
        }

        public PlayerData GetPlayerData()
        {
            using (var proxy = new DataControlServiceClient())
            {
                return proxy.GetPlayerData(new GetPlayerDataRequest() { CID = 1234567 }).PlayerData;
            }
        }

        public void SetPlayerData(PlayerData playerData)
        {
            using (var proxy = new DataControlServiceClient())
            {
                proxy.SetPlayerData(new SetPlayerDataRequest() { PlayerData = playerData });
            }
        }
        
    }
}
