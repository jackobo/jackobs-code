using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class DeployableFileDefinition
    {
        public DeployableFileDefinition(string relativePath, DeployEnvironment environment)
        {
            this.RelativePath = relativePath;
            this.Environment = environment;
        }

        public string RelativePath { get; private set; }
        public DeployEnvironment Environment { get; private set; }

    }
}
