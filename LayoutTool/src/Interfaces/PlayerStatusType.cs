using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace LayoutTool.Interfaces
{
    public class PlayerStatusType : SmartEnum<string, PlayerStatusType>
    {
        private PlayerStatusType(string id, string description, string actionName, int priority)
            : base(id, description)
        {
            this.ActionName = actionName;
            this.Priority = priority;
        }

        
        public static readonly PlayerStatusType Default = new PlayerStatusType(string.Empty, "Default Layout", "", 0);
        public static readonly PlayerStatusType New1 = new PlayerStatusType("new1", "Dynamic Layout: new1", "updatePlayerStatus_new1", 1);
        public static readonly PlayerStatusType New2 = new PlayerStatusType("new2", "Dynamic Layout: new2", "updatePlayerStatus_new2", 2);
        public static readonly PlayerStatusType Status4 = new PlayerStatusType("4", "Dynamic Layout: Status 4", "updatePlayerStatus_status_4", 4);
        public static readonly PlayerStatusType Status5 = new PlayerStatusType("5", "Dynamic Layout: Status 5", "updatePlayerStatus_status_5", 5);
        public static readonly PlayerStatusType Status6 = new PlayerStatusType("6", "Dynamic Layout: Status 6", "updatePlayerStatus_status_6", 6);
        



        public bool IsDynamicLayout
        {
            get { return !string.IsNullOrEmpty(this.Id); }
        }

        public int Priority { get; private set; }


        public string ActionName { get; private set; }


        public static PlayerStatusType[] DynamicLayouts
        {
            get { return All.Where(ps => ps.IsDynamicLayout).ToArray(); }
        }

    }
}
