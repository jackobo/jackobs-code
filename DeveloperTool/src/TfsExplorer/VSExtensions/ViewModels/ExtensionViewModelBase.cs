using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Wpf.Common.ViewModels;

namespace Spark.VisualStudio.Extensions.ViewModels
{
    public class ExtensionViewModelBase : ViewModelBase
    {
        public ExtensionViewModelBase(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        protected IServiceProvider ServiceProvider { get; private set; }

        protected T GetService<T>()
        {
            if (this.ServiceProvider != null)
            {
                return (T)this.ServiceProvider.GetService(typeof(T));
            }
            return default(T);
        }
    }
}
