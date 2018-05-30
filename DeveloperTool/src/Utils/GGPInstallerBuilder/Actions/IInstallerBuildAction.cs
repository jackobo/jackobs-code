using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGPInstallerBuilder.Actions
{
    public interface IInstallerBuildAction
    {
        void Execute(IInstallerBuildContext buildContext);
    }
}
