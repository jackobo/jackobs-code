﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.ViewModels
{
    public interface IGameEngineViewModel : IComponentViewModel
    {
        ObservableCollection<IGameViewModel> Games { get; }
    }
}
