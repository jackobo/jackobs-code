using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Spark.Infra.Logging;

//http://www.odewit.net/ArticleContent.aspx?id=MsBuildApi4&format=html

namespace Spark.TfsExplorer.Models.Build
{
    public class CompileAction : IBuildAction
    {
        public CompileAction()
        {
        }

        public void Execute(IBuildContext buildContext)
        {
            
            buildContext.Logger.Info($"Compile {buildContext.BuildConfiguration.SolutionFile}");

            var buildRequestData = new BuildRequestData(buildContext.BuildConfiguration.SolutionFile.AsString(), 
                                               buildContext.BuildConfiguration.GlobalProperties, 
                                               null, 
                                               new string[] { "Build" }, 
                                               null);


            var logger = new CompileActionLogger(buildContext.Logger);
            var buildParameters = new BuildParameters()
            {
                Loggers = new Microsoft.Build.Framework.ILogger[] { logger }
            };

            var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);

            if(buildResult.Exception != null)
            {
                buildContext.Logger.Exception("MSBuild exception: ", buildResult.Exception);
                throw new ApplicationException("Compile action failed! See previous errors for more details!");
            }

            if (buildResult.OverallResult == BuildResultCode.Failure)
            {
                if (logger.BuildErrorCodes.Any(errCode => errCode != MSBErrorCodes.MSB3202))
                {
                    throw new ApplicationException("Compile action failed! See previous errors for more details!");
                }
            }
            
        }
    }

    internal static class MSBErrorCodes
    {
        public static readonly string MSB3202 = "MSB3202"; //The project file {ProjectFile} was not found.
    }

    public class CompileActionLogger : Microsoft.Build.Framework.ILogger
    {
        public CompileActionLogger(Infra.Logging.ILogger logger)
        {
            this.Logger = logger;
        }




        HashSet<string> _buildErrorCodes = new HashSet<string>();
        public IEnumerable<string> BuildErrorCodes
        {
            get
            {
                return _buildErrorCodes;
            }
        }

        Infra.Logging.ILogger Logger { get; set; }
        public string Parameters { get; set; }

        public LoggerVerbosity Verbosity { get; set; }

        IEventSource _eventSource;
        public void Initialize(IEventSource eventSource)
        {
            _eventSource = eventSource;
            _eventSource.ErrorRaised += EventSource_ErrorRaised;
            //_eventSource.WarningRaised += EventSource_WarningRaised;
        }


        /*
        private void EventSource_WarningRaised(object sender, BuildWarningEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append($"MSBuild warning {e.Code}");
            sb.AppendLine(e.Message);
            sb.AppendLine($"Project: {e.ProjectFile}");
            sb.AppendLine($"Line {e.LineNumber} Column {e.ColumnNumber} in file {e.File}");
            sb.Append($"{new string('*', 50)}");
            Logger.Warning(sb.ToString());
        }
        */

        private void EventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append($"MSBuild error {e.Code}");
            sb.AppendLine(e.Message);
            sb.AppendLine($"Project: {e.ProjectFile}");
            sb.AppendLine($"Line {e.LineNumber} Column {e.ColumnNumber} in file {e.File}");
            sb.Append($"{new string('*', 50)}");
            
            _buildErrorCodes.Add(e.Code);
#warning I should find a way to exclude the missing projects from the build configuration. Or to remove the projects from the solution file.
            if (e.Code == MSBErrorCodes.MSB3202)
            {
                //Logger.Warning(sb.ToString());
            }
            else
            {
                Logger.Error(sb.ToString());
            }
        }

        public void Shutdown()
        {
            if(_eventSource != null)
            {
                _eventSource.ErrorRaised -= EventSource_ErrorRaised;
                //_eventSource.WarningRaised -= EventSource_WarningRaised;
                _eventSource = null;
            }
        }
    }
}
