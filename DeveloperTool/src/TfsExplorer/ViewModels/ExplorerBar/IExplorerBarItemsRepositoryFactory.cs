﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public interface IExplorerBarItemsRepositoryFactory
    {
        IExplorerBarItemsRepository GetRepository(IExplorerBarItem parentItem);
    }
}
