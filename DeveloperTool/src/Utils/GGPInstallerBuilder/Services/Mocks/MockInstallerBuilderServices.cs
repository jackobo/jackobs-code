using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Windows;

namespace GGPInstallerBuilder.Mocks
{
    public class MockInstallerBuilderServices : IInstallerBuildServices
    {       
        public MockInstallerBuilderServices(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            _operatingSystemServices = new OperatingSystemServices(loggerFactory);
        }

        public ILoggerFactory LoggerFactory { get; private set; }
        IOperatingSystemServices _operatingSystemServices;
        IFileSystemAdapter _fileSystemAdapter;

        public IFileSystemAdapter FileSystemAdapter
        {
            get
            {
                if (_fileSystemAdapter == null)
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

        public ISourceControlAdapter SourceControlAdapter
        {
            get
            {
                return new MockSourceControlAdapter(_operatingSystemServices.FileSystem);
            }
        }

        
    }
}
