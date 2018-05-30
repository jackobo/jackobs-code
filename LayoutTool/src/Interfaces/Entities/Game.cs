using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LayoutTool.Interfaces.Entities
{
    public class Game
    {
        public Game()
        {

        }
        public Game(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        [XmlAttribute("gameType")]
        public int Id { get; set; }
        [XmlAttribute("gameName")]
        public string Name { get; set; }
        
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            var theOther = obj as Game;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id;
        }

        public override string ToString()
        {
            return this.Name;
        }

        
    }

}
