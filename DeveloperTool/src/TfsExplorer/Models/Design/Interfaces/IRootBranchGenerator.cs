﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    public interface IRootBranchGenerator
    {
        void CreateRootBranch(IRootBranchExtended sourceBranch, Action<ProgressCallbackData> progressCallback);
    }
}
