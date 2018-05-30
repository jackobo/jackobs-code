using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public abstract class BinaryOutputFile : IOutputFile
    {
        public BinaryOutputFile(BuildOutputFileDefinition fileDefinition)
        {
            FileDefinition = fileDefinition;
        }

        
        protected BuildOutputFileDefinition FileDefinition { get; private set; }

        public DeployableFileDefinition GetDeployableFileDefinition()
        {
            return new DeployableFileDefinition(this.FileDefinition.DistributionRelativePath, this.FileDefinition.DeployEnvironment);
        }

        public virtual ILocalPath ResolveBuildOutputPath(ILocalPath basePath)
        {
            return basePath.Subpath(FileDefinition.BuildOutputRelativePath);
            
        }

        public ILocalPath ResolveDistributionPath(ILocalPath basePath)
        {
            return basePath.Subpath(this.FileDefinition.DistributionRelativePath);
        }
    }
}
