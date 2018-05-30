using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGPInstallerBuilder.Actions
{
    public class CompileInstaller : IInstallerBuildAction
    {
        public void Execute(IInstallerBuildContext buildContext)
        {
            
            var csprojFiles = buildContext.FileSystemAdapter.FindFiles(buildContext.BuildConfiguration.InstallerProjectPath, "*.csproj");
            if (csprojFiles.Length == 0)
                throw new ApplicationException($"Can't find a project file in {buildContext.BuildConfiguration.InstallerProjectPath.AsString()}");

            if (csprojFiles.Length > 1)
                throw new ApplicationException($"Too many project files detected in {buildContext.BuildConfiguration.InstallerProjectPath.AsString()}");


            var csProjFile = csprojFiles.First().AsString();
            buildContext.Logger.Info($"Compiling {csProjFile}");

            var buildRequestData = new Microsoft.Build.Execution.BuildRequestData(csProjFile,
                                             buildContext.BuildConfiguration.GlobalProperties,
                                             null,
                                             new string[] { "Build" },
                                             null);

            var buildParameters = new Microsoft.Build.Execution.BuildParameters()
            {
                Loggers = new Microsoft.Build.Framework.ILogger[] { new CompileActionLogger(buildContext.Logger) }
            };

            var buildResult = Microsoft.Build.Execution.BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);

            if (buildResult.Exception != null)
            {
                buildContext.Logger.Exception("MSBuild exception: ", buildResult.Exception);
            }

            if (buildResult.OverallResult == Microsoft.Build.Execution.BuildResultCode.Failure)
                throw new ApplicationException("Compile action failed! See previous errors for more details!");


        }
    }

    public class CompileActionLogger : Microsoft.Build.Framework.ILogger
    {
        public CompileActionLogger(Spark.Infra.Logging.ILogger logger)
        {
            this.Logger = logger;
        }

        Spark.Infra.Logging.ILogger Logger { get; set; }
        public string Parameters { get; set; }

        public Microsoft.Build.Framework.LoggerVerbosity Verbosity { get; set; }

        Microsoft.Build.Framework.IEventSource _eventSource;
        public void Initialize(Microsoft.Build.Framework.IEventSource eventSource)
        {
            _eventSource = eventSource;
            _eventSource.ErrorRaised += EventSource_ErrorRaised;
            //_eventSource.WarningRaised += EventSource_WarningRaised;
        }



        private void EventSource_WarningRaised(object sender, Microsoft.Build.Framework.BuildWarningEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append($"Warning {e.Code}");
            sb.AppendLine(e.Message);
            sb.AppendLine($"Project: {e.ProjectFile}");
            sb.AppendLine($"Line {e.LineNumber} Column {e.ColumnNumber} in file {e.File}");
            sb.Append($"{new string('*', 50)}");
            Logger.Warning(sb.ToString());
        }

        private void EventSource_ErrorRaised(object sender, Microsoft.Build.Framework.BuildErrorEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append($"Error {e.Code}");
            sb.AppendLine(e.Message);
            sb.AppendLine($"Project: {e.ProjectFile}");
            sb.AppendLine($"Line {e.LineNumber} Column {e.ColumnNumber} in file {e.File}");
            sb.Append($"{new string('*', 50)}");
            Logger.Error(sb.ToString());
        }

        public void Shutdown()
        {
            if (_eventSource != null)
            {
                _eventSource.ErrorRaised -= EventSource_ErrorRaised;
                _eventSource.WarningRaised -= EventSource_WarningRaised;
                _eventSource = null;
            }
        }
    }
}
