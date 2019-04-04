using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface ICoreComponentContentProvider : ICompilableComponentContentProvider
    {
        Optional<CoreComponentCustomizationMetaData> GetCustomizationMetaData();
    }

    public class CoreComponentCustomizationMetaData
    {
        public CoreComponentCustomizationMetaData(int? componentType, string friendlyName)
        {
            this.ComponentType = componentType;
            this.FriendlyName = friendlyName;
        }
        public int? ComponentType { get; private set; }
        public string FriendlyName { get; private set; }

    }
}
