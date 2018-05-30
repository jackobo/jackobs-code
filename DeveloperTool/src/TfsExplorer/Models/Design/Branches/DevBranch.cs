using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public class DevBranch : EnvironmentBranch<Folders.DevFolder>, IDevBranch
    {
        
        public DevBranch(Folders.DevFolder devFolder, IRootBranch owner, IServiceLocator serviceLocator)
            : base(devFolder, owner, serviceLocator)
        {
           
        }



        protected override IComponentsReader ComponentsReader
        {
            get
            {
                return this.ServiceLocator.GetInstance<IComponentsReaderFactory>().DevMainBranchComponentsReader();
            }
        }

        protected override IBranchFolder GetMainFolder()
        {
            return this.Location.Main;
        }


        public IEnumerable<IMergeSet> GetMergeSetsToQA()
        {
            return this.ServiceLocator.GetInstance<IMergeSetsReader>().ReadMergeSets(this.GetComponents(), Location.Parent.QA.Main.Components);
        }
    }
}
