using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models
{
    public interface IInstalationContext
    {
        GGPGameServer.ApprovalSystem.Common.IEnvironmentServices EnvironmentServices { get; }
        InstallationParameters Parameters { get; }
        GGPGameServer.ApprovalSystem.Common.Logger.ILogNotifier Logger { get; }
        
    }


    public class InstallationParameters
    {
        public string GGPMockFolder
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(@"%SystemDrive%\GGPMock");
            }
        }

        public int GameServerId
        {
            get { return 123456; }
        }

        public int GGPSimulatorPort
        {
            get { return 5445; }
        }

        public string AppDataFolder
        {
            get
            {
                return ApplicationFolders.AppData;
            }
        }
    }
}
