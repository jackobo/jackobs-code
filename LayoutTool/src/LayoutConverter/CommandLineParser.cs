using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.Infra.Windows;

namespace LayoutConverter
{
    public class CommandLineParser
    {
        

        private static string ExeName
        {
            get { return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location); }
        }

        public static string UsageInfo
        {
            get
            {
                

                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"{ExeName} <LayoutToolFile> [NavigationPlanFile] [{OptionNames.outputFolder}=path]");
                sb.AppendLine();
                sb.AppendLine(GetParametersUsageInfo());
                sb.AppendLine();
                sb.AppendLine("EXAMPLES:");
                sb.AppendLine($@"(1) {ExeName} C:\Storage\OriginalFiles\layoutFile.lyt");
                sb.AppendLine();
                sb.AppendLine($@"(2) {ExeName} $/GamingNDL/Develop/CasinoFlashClient/Configuration/HEAD/LayoutTool/Production/Brand_0/Skin_4/2017-01-26-16-47-43.lyt");
                sb.AppendLine();
                sb.AppendLine($@"(3) {ExeName} C:\Storage\OriginalFiles\layoutFile.lyt C:\Storage\OriginalFiles\navigation_plan_ndl.xmm");
                sb.AppendLine();
                sb.AppendLine($@"(4) {ExeName} C:\Storage\OriginalFiles\layoutFile.lyt C:\Storage\OriginalFiles\navigation_plan_ndl.xmm  {OptionNames.outputFolder}=C:\Output");
                sb.AppendLine();
                sb.AppendLine($@"(5) {ExeName} C:\Storage\OriginalFiles\layoutFile.lyt {OptionNames.outputFolder}=C:\Output");

                return sb.ToString();
            }
        }

        private static string GetParametersUsageInfo()
        {
            var sb = new StringBuilder();

            var parametersDefinitions = new List<KeyValuePair<string, string>>();
            parametersDefinitions.Add(ParameterDescription("<LayoutToolFile>", "Required. The path to the file created using the Layout Admin Tool. It can be a local disk file or a TFS file."));
            parametersDefinitions.Add(ParameterDescription("[NavigationPlanFile]", "Optional. The path to the navigation_plan.xmm file. This parameter can be ommited only if the provided <LayoutToolFile> is a production one."));
            parametersDefinitions.Add(ParameterDescription($"[{OptionNames.outputFolder}=path]", $"Optional. Provide this parameter if you want to specify the folder where the converted file to be placed. If you ommit this parameter the resulted file will be placed in the folder from where you run {ExeName}"));

            int maxParamNameLength = parametersDefinitions.Max(p => p.Key.Length) + 3;

            foreach(var param in parametersDefinitions )
            {
                sb.AppendLine();
                sb.Append(param.Key.PadRight(maxParamNameLength, '-'));
                sb.Append(param.Value);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static KeyValuePair<string, string> ParameterDescription(string name, string description)
        {
            return new KeyValuePair<string, string>(name, description);
        }

        public CommandLineArgs Parse(string[] args)
        {
            if (args.Length == 0)
                throw new MissingCommandLineArgumentsException();

            var inputFilesArgs = args.Where(a => !a.Contains("=")).ToArray();

            if(inputFilesArgs.Length == 0 || inputFilesArgs.Length > 2)
                throw new InvalidCommandLineArgumentsException();

            var commandLineArgs = new CommandLineArgs(inputFilesArgs[0]);
            
            if(inputFilesArgs.Length == 2)
            {
                commandLineArgs.NavigationPlanFile = Optional<string>.Some(inputFilesArgs[1]);
            }


            var optionArgs = StringKeyValueCollection.Parse(args.Where(a => a.Contains("=")).ToArray());

            var outpFolderOption = optionArgs.FirstOrDefault(o => 0 == string.Compare(o.Name, OptionNames.outputFolder, true));
            if (outpFolderOption != null)
            {
                if (string.IsNullOrEmpty(outpFolderOption.Value))
                    throw new InvalidCommandLineArgumentsException($"If you specify {OptionNames.outputFolder} paramters you must provide the path for the folder");

                commandLineArgs.OutputFolder = Optional<string>.Some(outpFolderOption.Value);
            }


            return commandLineArgs;
        }

        private static class OptionNames
        {
            public static readonly string outputFolder = "outputFolder";
        }
    }

    public class MissingCommandLineArgumentsException : ApplicationException
    {

    }

    public class InvalidCommandLineArgumentsException : ApplicationException
    {
        public InvalidCommandLineArgumentsException()
            : base("Invalid command line arguments")
        {

        }

        public InvalidCommandLineArgumentsException(string message)
           : base(message)
        {

        }

    }

    public class CommandLineArgs
    {
        public CommandLineArgs(string layoutFile)
        {
            this.LayoutFile = layoutFile;
        }

        
        public string LayoutFile { get; private set; }
        public Optional<string> NavigationPlanFile { get; set; } = Optional<string>.None();
        public Optional<string> OutputFolder { get; set; } = Optional<string>.None();
    }
}
