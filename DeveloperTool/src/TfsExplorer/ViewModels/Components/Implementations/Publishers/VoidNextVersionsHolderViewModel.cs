using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.ViewModels
{
    public class VoidNextVersionsHolderViewModel : INextVersionProviderViewModel
    {
        public VersionNumber[] AvailableVersions
        {
            get
            {
                return new VersionNumber[0];
            }
        }

        public VersionNumber SelectedVersion
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
