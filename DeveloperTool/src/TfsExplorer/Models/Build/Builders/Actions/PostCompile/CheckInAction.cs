using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public class CheckInAction : IBuildAction
    {
        public CheckInAction(string comment)
        {
            _comment = comment;
        }

        string _comment;
        public void Execute(IBuildContext buildContext)
        {
            buildContext.Logger.Info($"Check-in: {_comment}");
            buildContext.SourceControlAdapter.CheckInPendingChanges(_comment);
        }
    }
}
