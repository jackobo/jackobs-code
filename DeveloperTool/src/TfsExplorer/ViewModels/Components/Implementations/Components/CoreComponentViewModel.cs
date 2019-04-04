using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.ViewModels
{
    public class CoreComponentViewModel : ComponentViewModel<ICoreComponent>, ICoreComponentViewModel
    {
        public CoreComponentViewModel(ICoreComponent coreComponent)
            : base(coreComponent)
        {

        }

       
    }
}
