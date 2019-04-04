using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    public static class Constants
    {
        public static readonly int ArenaPageSize = 10;

        public static readonly int DefaultNumberOfLobbyItems = 6;

        public static readonly string[] MyAccountItemsThatCannotBeRemoved = new string[] { "LOGOUT_ITEM", "ABOUT_ITEM" };

        public static readonly int MaxTopGamesItems = 4;

        public static readonly int LiveCasinoArenaType = 2010004;

        public static readonly int MaxNumberOfFiltesInArena = 5;
    }
}
