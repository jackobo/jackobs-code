using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    internal static class ExternalProcessRunner
    {
        public static void Run(string fileName, string arguments)
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo(fileName, arguments);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                process.Start();
                string errorMessage = process.StandardError.ReadToEnd();
                process.WaitForExit(5 * 60 * 1000); //5 minutes timeout
                if (0 != process.ExitCode && !string.IsNullOrEmpty(errorMessage))
                {
                    throw new ApplicationException(string.Format("Process '{0}' failed to execute: ExitCode = {1}; Message = {2}", fileName + " " + arguments, process.ExitCode, errorMessage));
                }
            }


        }


        public static void RunWithShellExecute(string fileName, string arguments)
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo(fileName, arguments);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = true;
            processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            processInfo.Verb = "runas";
            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                process.Start();
                process.WaitForExit(5 * 60 * 1000); //5 minutes timeout
            }


        }

        public static string RunAndGetTheOutput(string fileName, string arguments)
        {
            string result = string.Empty;
            var processInfo = new System.Diagnostics.ProcessStartInfo(fileName, arguments);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                process.Start();
                result = process.StandardOutput.ReadToEnd();
                process.WaitForExit(5 * 60 * 1000); //5 minutes timeout

            }

            return result;
        }
    }
}
