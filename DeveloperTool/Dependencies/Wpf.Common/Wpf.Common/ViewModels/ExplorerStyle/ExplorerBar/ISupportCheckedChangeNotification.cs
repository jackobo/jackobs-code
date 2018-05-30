using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels
{
    public interface ISupportCheckedChangeNotification
    {
        void RaiseIsCheckedChanged();
        
    }

}
