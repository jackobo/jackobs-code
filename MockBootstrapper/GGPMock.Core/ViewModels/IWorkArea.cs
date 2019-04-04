using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public interface IWorkArea
    {
        TProduct GetProduct<TProduct>() where TProduct : Models.Product;
        IEnvironmentServices EnvironmentServices { get; }
        Models.IGamesInformationProvider GamesInformationProvider { get; }
    }
}
