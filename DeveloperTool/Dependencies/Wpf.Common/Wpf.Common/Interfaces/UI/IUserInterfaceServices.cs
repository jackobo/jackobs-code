using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.Interfaces.UI
{
    public interface IUserInterfaceServices
    {
        IMessageBox MessageBox { get; }
        IDialogServices DialogServices { get; }

    }
}
