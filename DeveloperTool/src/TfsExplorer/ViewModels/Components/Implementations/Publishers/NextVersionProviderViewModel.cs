using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class NextVersionProviderViewModel : ViewModelBase, INextVersionProviderViewModel
    {
        public NextVersionProviderViewModel(INextVersionProvider nextVersionProvider)
        {
            _availableVersions.AddRange(nextVersionProvider.NextRegularVersion);
            _availableVersions.AddRange(nextVersionProvider.NextMinorVersion);
            this.SelectedVersion = _availableVersions.FirstOrDefault();
        }

        private VersionNumber _selectedVersion;

        public VersionNumber SelectedVersion
        {
            get { return _selectedVersion; }
            set { SetProperty(ref _selectedVersion, value); }
        }

        private List<VersionNumber> _availableVersions = new List<VersionNumber>();

        public VersionNumber[] AvailableVersions
        {
            get { return _availableVersions.ToArray(); }
        }

        public static NextVersionProviderViewModel From(INextVersionProvider nextVersionHolder)
        {
            return new NextVersionProviderViewModel(nextVersionHolder);

        }
    }
}
