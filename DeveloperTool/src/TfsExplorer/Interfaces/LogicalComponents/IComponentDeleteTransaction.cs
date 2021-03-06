﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IComponentDeleteTransaction
    {
        ILocalPath LocalPath { get; }
        IEnumerable<IComponentUniqueId> GetUniqueIDs();
        void Commit();
    }
}
