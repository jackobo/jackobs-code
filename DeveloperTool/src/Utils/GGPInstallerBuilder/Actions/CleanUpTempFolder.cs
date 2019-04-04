using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder.Actions
{
    public class CleanUpTempFolder : IInstallerBuildAction
    {
        ILocalPath _tempFolder;
        bool _throwIfFails;
        public CleanUpTempFolder(ILocalPath tempFolder, bool throwIfFails)
        {
            _tempFolder = tempFolder;
            _throwIfFails = throwIfFails;
        }
        public void Execute(IInstallerBuildContext context)
        {
            context.Logger.Info($"Clean up temp folder {_tempFolder.AsString()}");
            try
            {
                if (context.FileSystemAdapter.FolderExists(_tempFolder))
                {
                    context.FileSystemAdapter.DeleteFolder(_tempFolder);
                }
            }
            catch(Exception ex)
            {
                if(_throwIfFails)
                {
                    context.Logger.Exception($"Failed to delete temp folder {_tempFolder.AsString()}!", ex);
                    throw;
                }

                context.Logger.Warning($"Failed to delete temp folder {_tempFolder.AsString()}! Exception details: {ex.ToString()}");
            }
        }
    }
}
