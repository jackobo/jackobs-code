using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Actions
{
    public class RenameComponentAction : RenameDeleteAction
    {
        public RenameComponentAction(IComponentRenameDeleteHandler renameDeleteHandler, ILogicalComponent component)
            : base(renameDeleteHandler, component)
        {
            
        }
        

        public override string Caption
        {
            get
            {
                return "Rename";
            }
        }

        public override void Execute(object parameter)
        {
            RenameDeleteHandler.StartRenaming(Component);
        }
    }
}
