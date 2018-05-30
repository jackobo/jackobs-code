using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Models.Builders.Xml
{
    internal static class XmlValues
    {
        public static readonly int UserModeVip = 2;
        public static readonly string lobby = "lobby";
        public static readonly string history = "history";

        public static readonly Dictionary<int, int[]> x = new Dictionary<int, int[]>
        {
            {5, new int[] {0,169,415,673, 842} },
            {6, new int[] {6,172,338,504,670,836 } }
        };

        public static readonly int y = 156;
        public static readonly string btnArenaTexedTemplate = "btnArenaTexedTemplate";
        public static readonly string mcDynamicTextedButtonPersonalize = "mcDynamicTextedButtonPersonalize";
        public static readonly string btnDynamicTexted = "btnDynamicTexted";
        public static readonly string BIG_ICON = "BIG_ICON";
        public static readonly string copy = "copy";
    }
}
