using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGPInstallerBuilder
{
    public interface IInstallerDefinitionReader
    {
        InstallerDefinition Read(BuildTaskInfo taskInfo);
    }
}
