using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Configurations;
using Spark.Infra.Logging;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace GamesPortal.Service
{
    public class WcfServiceBase
    {
        protected WcfServiceBase()
        {

        }
        public WcfServiceBase(IGamesPortalInternalServices services)
        {
            Services = services;
        }

        protected IGamesPortalInternalServices Services { get; private set; }

        protected ILogger Logger
        {
            get
            {
                return Services.LoggerFactory.CreateLogger(this.GetType());
            }
        }

        protected IConfigurationReader ConfigurationReader
        {
            get
            {
                return Services.ConfigurationReader;
            }
        }

        
        protected void LogException(string operationName, Exception ex, object request = null)
        {
            if(request == null)
                Logger.Exception(string.Format("{0} failed!", operationName), ex);
            else
                Logger.Exception(string.Format("{0} failed! REQUEST: {1}", operationName, JsonConvert.SerializeObject(request)), ex);
        }


        protected void LogException(Exception ex, [CallerMemberName]string operationName = "")
        {
            LogException(operationName, ex);
        }


        protected void LogException(Exception ex, object request, [CallerMemberName]string operationName = "")
        {
            LogException(operationName, ex, request);
        }
    }
}
