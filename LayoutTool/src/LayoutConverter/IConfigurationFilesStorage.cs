using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutConverter;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Spark.Infra.Windows;

namespace LayoutConverter
{
    public interface IConfigurationFilesStorage
    {
        InputFile GetNavigationPlanFile(SkinIndentity skinIdentity);
    }

    public class HardCodedConfigurationFilesStorage : IConfigurationFilesStorage
    {
        public HardCodedConfigurationFilesStorage(IFileSystemManager fileSystemManager)
        {
            _fileSystemManager = fileSystemManager;
        }

        IFileSystemManager _fileSystemManager;
        public InputFile GetNavigationPlanFile(SkinIndentity skinIdentity)
        {
            var fileFullPath = @"C:\CasinoTools\NDLLayoutAdmin\Head\src\LayoutConverter\OriginalFiles\navigation_plan_ndl.xmm";
            return new InputFile(Path.GetFileName(fileFullPath), 
                                _fileSystemManager.ReadAllText(fileFullPath),
                                new PathDescriptor(fileFullPath));
        }


    }

    public class ArtifactoryConfigurationFilesStorage : IConfigurationFilesStorage
    {
        public InputFile GetNavigationPlanFile(SkinIndentity skinIdentity)
        {
            throw new InvalidCommandLineArgumentsException("Reading navigation file from Artifactory is not implemented!");
        }
    }
}
