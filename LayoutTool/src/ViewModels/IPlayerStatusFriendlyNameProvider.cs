using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.ViewModels
{
    public interface IPlayerStatusFriendlyNameProvider
    {
        string GetFriendlyName(PlayerStatusType playerStatusType);

        event EventHandler Changed;
    }


    
}
