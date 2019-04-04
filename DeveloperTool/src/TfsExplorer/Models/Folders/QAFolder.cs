using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class QAFolder : EnvironmentFolder
    {
        public QAFolder(RootBranchFolder logicalBranchFolder)
            : base("QA", logicalBranchFolder)
        {

        }

        public override IBranchFolder GetMain()
        {
            return this.Main;
        }

        public QAMainFolder Main
        {
            get { return new QAMainFolder(this); }
        }

        public QAInstallersFolder Installers
        {
            get { return new QAInstallersFolder(this); }
        }

        public HotFixTriggerFolder<QAFolder> HotfixTrigger
        {
            get { return new HotFixTriggerFolder<QAFolder>(this); }
        }
    }
}
