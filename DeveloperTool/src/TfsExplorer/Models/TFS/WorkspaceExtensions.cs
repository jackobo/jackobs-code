using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Spark.TfsExplorer.Models.TFS
{
    public static class WorkspaceExtensions
    {
        public static void ExecuteVoid(this Workspace workspace, Action<Workspace> action)
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

        public static TResult ExecuteReturn<TResult>(this Workspace workspace, Func<Workspace, TResult> action)
        {
            Exception exception = null;
            Failure failure = null;

            workspace.VersionControlServer.NonFatalError += (sndr, args) =>
            {
                exception = args.Exception;
                failure = args.Failure;
            };


            var result = action(workspace);

            if (exception != null)
                throw new ApplicationException("Version control exception: " + exception.Message, exception);

            if (failure != null)
                throw new ApplicationException("Version control failure: " + Environment.NewLine + failure.ToString());

            return result;
        }

        public static int CheckInWithPolicyOverride(this Workspace workspace, string comment, PendingChange[] pendingChanges, string policyOverrideMessage = "")
        {
            PolicyOverrideInfo policyOverride = null;
            if (!string.IsNullOrEmpty(policyOverrideMessage))
            {
                var evaluationResult = workspace.EvaluateCheckin(CheckinEvaluationOptions.Policies,
                                                                 pendingChanges,
                                                                 pendingChanges,
                                                                 comment,
                                                                 null,
                                                                 new WorkItemCheckinInfo[0]);

                policyOverride = new PolicyOverrideInfo(policyOverrideMessage, evaluationResult.PolicyFailures);
            }

#warning Associate workitem
            return workspace.CheckIn(pendingChanges,
                                      comment,
                                      null,
                                      null,
                                      policyOverride);
        }
    }
}
