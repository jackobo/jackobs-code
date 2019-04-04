using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder
{
    public class InstallerBuilderServices : IInstallerBuildServices
    {
        
        public InstallerBuilderServices(ILoggerFactory loggerFactory, 
                                        IWorkspaceSelector workspaceSelector)
        {
            LoggerFactory = loggerFactory;
            _operatingSystemServices = new OperatingSystemServices(loggerFactory);
            _workspaceSelector = workspaceSelector;
            

        }

        public ILoggerFactory LoggerFactory { get; private set; }
        IOperatingSystemServices _operatingSystemServices;
        IWorkspaceSelector _workspaceSelector;

        IFileSystemAdapter _fileSystemAdapter;
        public IFileSystemAdapter FileSystemAdapter
        {
            get
            {
                if(_fileSystemAdapter == null)
                    _fileSystemAdapter = new FileSystemAdapter(_operatingSystemServices.FileSystem);

                return _fileSystemAdapter;
            }
        }

        public IInstallerDefinitionReader InstallerDefinitionReader
        {
            get
            {
                return new InstallerDefinitionReader(SourceControlAdapter);
            }
        }

        ISourceControlAdapter _sourceControlAdapter;
        public ISourceControlAdapter SourceControlAdapter
        {
            get
            {
                if(_sourceControlAdapter == null)
                {
                    _sourceControlAdapter = new TfsSourceControlAdapter(_workspaceSelector);
                }
                return _sourceControlAdapter;
            }

        }
    }
}
