using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public static class AntPropertyNames
    {
        public static readonly string major = "major";
        public static readonly string minor = "minor";
        public static readonly string revision = "revision";
        public static readonly string build = "build";
        public static readonly string ProjDir = "ProjDir";

        public static readonly string TestingDeployFiles = "TestingDeployFiles";
        public static readonly string ProductionDeployFiles = "ProductionDeployFiles";

        public static readonly string PublishDate = "PublishDate";

        public static readonly string FriendlyName = "FriendlyName";

        public static readonly string GGPGameServerUniqueID = "GGPGameServerUniqueID";
        public static readonly string GameCommonInterfaceUniqueID = "GameCommonInterfaceUniqueID";
        public static readonly string GamesCommonUniqueID = "GamesCommonUniqueID";
        public static readonly string GGPConfigurationEditorUniqueID = "GGPConfigurationEditorUniqueID";
        public static readonly string CoreComponentUniqueID = "CoreComponentUniqueID";
        public static readonly string GameEngineUniqueID = "GameEngineUniqueID";
        public static readonly string GameUniqueName = "GameUniqueName";
        public static readonly string GamePartUniqueID = "GamePartUniqueID";
        public static readonly string GameVersion = "GameVersion";
        public static readonly string GamePart = "GamePart";
        public static readonly string ParentGameEngineUniqueID = "ParentGameEngineUniqueID";

        //installer specific
        public static readonly string InstallerUniqueID = "InstallerUniqueID";
        public static readonly string InstallerVersion = "InstallerVersion";
        public static readonly string TriggeredBy = "TriggeredBy";
        public static readonly string CustomizedInstaller = "CustomizedInstaller";

        public static readonly string ProductionEnvironment = "ProductionEnvironment";
        public static readonly string TriggerDate = "TriggerDate";
    }
}
