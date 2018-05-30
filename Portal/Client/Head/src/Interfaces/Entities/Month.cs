using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Client.Interfaces.Entities
{
    public sealed class Month
    {

        private static Dictionary<int, Month> _months;
        static Month()
        {
            var allMonths = new List<Month>();
            _months = new Dictionary<int, Month>();

            for (int i = 1; i <= 12; i++)
            {
                var m = new Month(i, new DateTime(DateTime.Today.Year, i, 1).ToString("MMM"));
                _months.Add(m.Id, m);
                allMonths.Add(m);
            }

            All = allMonths.ToArray();
        }

        public static Month[] All { get; private set; }
        public Month(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }

        public static Month Get(int id)
        {
            return _months[id];
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as Month;
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
