using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public interface INextVersionProviderViewModel : IViewModel
    {
        VersionNumber SelectedVersion { get; set; }
        VersionNumber[] AvailableVersions { get; }
    }
}
