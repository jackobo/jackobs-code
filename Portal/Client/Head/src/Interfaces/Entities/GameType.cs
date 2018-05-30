using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Client.Interfaces.Entities
{
    public class GameType
    {
        public GameType()
        {
        }

        public GameType(int id, string name, int? operatorId)
        {
            this.Id = id;
            this.Name = name;
            if (operatorId != null)
            {
                if (operatorId.Value == 0)
                    this.OperatorName = "888";
                else
                    this.OperatorName = "Bingo";
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string OperatorName { get; set; }

        public override string ToString()
        {
            return this.Id + " - " + this.Name;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as GameType;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
