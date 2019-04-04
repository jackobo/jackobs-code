using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;

namespace GamesPortal.Client.Interfaces.Entities
{
    public class ClientType 
    {
        public ClientType(string name, params RegulationType[] regulations)
        {
            this.Name = name;
            this.Regulations = regulations;
        }

        public string Name { get; private set; }

        public RegulationType[] Regulations { get; private set; }


        public static readonly string IOS = "IOS";

        public static readonly string Android = "Android";

        public static readonly string Embedded = "Embedded";

        public static readonly string Bingo = "Bingo";

        public static readonly string NDL = "NDL";
        

        public static readonly string GamesTab = "GamesTab";
    }
}
