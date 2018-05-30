using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces.MainProxyDataControlService
{
    public partial class PlayerAccountAttribute
    {
       

        public PlayerAccountAttribute()
        {

        }

        public PlayerAccountAttribute(PlayerAccountAttributeType type, object value)
        {
            AttributeType = type;
            Value = value?.ToString() ?? "";
        }

        public override string ToString()
        {
            return $"{AttributeType}: {Value}";
        }
    }
}
