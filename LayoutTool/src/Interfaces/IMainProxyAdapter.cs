using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.MainProxyDataControlService;

namespace LayoutTool.Interfaces
{
    public interface IMainProxyAdapter
    {
        int Port { get; }
        PlayerData GetPlayerData();
        void SetPlayerData(PlayerData playerData);
    }
}
