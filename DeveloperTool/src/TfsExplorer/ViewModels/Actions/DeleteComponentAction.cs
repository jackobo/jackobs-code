using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Actions
{
    public class DeleteComponentAction : RenameDeleteAction
    {
        public DeleteComponentAction(IComponentRenameDeleteHandler renameDeleteHandler, 
                                     ILogicalComponent component)
            : base(renameDeleteHandler, component)
        {
            
        }


        public override string Caption
        {
            get
            {
                return "Delete";
            }
        }

        public override void Execute(object parameter)
        {
            RenameDeleteHandler.StartDeleting(Component);
        }
    }
}
