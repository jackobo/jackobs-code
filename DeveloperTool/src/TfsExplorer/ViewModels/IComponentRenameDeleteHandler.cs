using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public interface IComponentRenameDeleteHandler : IViewModel
    {
        void StartRenaming(ILogicalComponent component);
        void StartDeleting(ILogicalComponent component);
        bool InProgress { get; }
    }

   
}
