using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Web.Administration;
using Spark.Wpf.Common.Interfaces.UI;

namespace LayoutTool.Models.IIS
{
    public class IISServerManager : IWebServerManager
    {
        ServerManager _iisManager;
        public IISServerManager(IApplicationServices application)
        {
            _iisManager = new ServerManager();
            application.ShuttingDown += Application_ShuttingDown;
        }

        private void Application_ShuttingDown(object sender, EventArgs e)
        {
            _iisManager.Dispose();
        }

        public IWebSite[] GetWebSites()
        {
            return _iisManager.Sites.Select(s => new WebSite(s)).ToArray();
            
        }
        
    }
}
