using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public interface IComponentViewModel : IViewModel, ILogicalComponentHolder
    {
        string Name { get; }
        string Version { get; }
        ComponentMetaDataItem[] MetaData { get; }
    }
    
}
