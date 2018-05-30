using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace GamesPortal.Service
{
    public static class WorkspaceExtensions
    {
        public static void ExecuteAndThrowOnNonFatalError(this Workspace workspace, Action<Workspace> action)
        {
            Exception exception = null;
            Failure failure = null;

            workspace.VersionControlServer.NonFatalError += (sndr, args) =>
            {
                exception = args.Exception;
                failure = args.Failure;
            };


            action(workspace);

            if (exception != null)
                throw new ApplicationException("Version control exception: " + exception.Message, exception);

            if (failure != null)
                throw new ApplicationException("Version control failure: " + Environment.NewLine + failure.ToString());
        }
    }
}
