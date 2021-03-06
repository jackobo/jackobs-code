﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.ViewModels
{
    public interface IGameEnginePublisherViewModel : IComponentPublisherViewModel
    {
        IEnumerable<IGamePublisherViewModel> Games { get; }

    }
}
