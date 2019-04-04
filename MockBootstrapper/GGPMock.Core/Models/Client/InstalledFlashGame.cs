using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models
{
    public class InstalledGame
    {
        public InstalledGame(int gameType, string physicalPath)
        {
            this.GameType = gameType;
            this.PhysicalPath = physicalPath;
        }
        public int GameType { get; private set; }
        public string PhysicalPath { get; private set; }

        public bool IsCustomGame { get; set; }
    }
}
