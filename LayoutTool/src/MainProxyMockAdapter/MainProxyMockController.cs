using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.MainProxy
{

    public interface IMainProxyMockController
    {
        void Start();
        void Stop();
    }
    public class MainProxyMockController : MarshalByRefObject, IMainProxyMockController
    {
        object _mainProxyMockService;
        
        public MainProxyMockController()
        {
        }


        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Start()
        {
            var mainProxyMockCoreAssembly = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MainProxyMock.Core.dll"));

            var mainProxyMockServiceType = mainProxyMockCoreAssembly.GetType("MainProxyMock.MainProxyMockService");

            var ctor =  mainProxyMockServiceType.GetConstructor(new Type[0]);
            _mainProxyMockService = ctor.Invoke(new object[0]);


            var startMethod = _mainProxyMockService.GetType().GetMethod("Start");
            startMethod.Invoke(_mainProxyMockService, new object[0]);
        }


        public void Stop()
        {
            var shutDownMethod = _mainProxyMockService.GetType().GetMethod("ShutDown");
            shutDownMethod.Invoke(_mainProxyMockService, new object[0]);
        }


    }
}
