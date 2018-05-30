using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IGameEngineComponent : ILogicalComponent
    {
        GameEngineName EngineName { get; }
    }

    public sealed class GameEngineName
    {
        public GameEngineName(string engineName)
        {
            if (string.IsNullOrEmpty(engineName))
                throw new ArgumentNullException(nameof(engineName));

           

            _engineName = engineName;
        }

        string _engineName;

        public override string ToString()
        {
            return _engineName;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as GameEngineName;
            if (theOther == null)
                return false;

            return string.Compare(this._engineName, theOther._engineName, true) == 0;
        }

        public override int GetHashCode()
        {
            return _engineName.ToLowerInvariant().GetHashCode();
        }

        public static bool operator ==(GameEngineName g1, GameEngineName g2)
        {
            if (!object.ReferenceEquals(g1, null))
                return g1.Equals(g2);
            else if (!object.ReferenceEquals(g2, null))
                return g2.Equals(g1);
            return true;

        }

        public static bool operator !=(GameEngineName g1, GameEngineName g2)
        {
            return !(g1 == g2);
        }
    }


    public class GameKey
    {
        public GameKey(GameEngineName engineName, string gameName)
        {
            this.EngineName = engineName;
            this.GameName = gameName;

        }

        public string GameName { get; set; }

        public GameEngineName EngineName { get; set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as GameKey;
            if (theOther == null)
                return false;

            return this.EngineName == theOther.EngineName
                    && string.Compare(this.GameName, theOther.GameName, true) == 0;

        }

        public override int GetHashCode()
        {
            return this.EngineName.GetHashCode() ^ this.GameName.ToLowerInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return this.GameName + " - " + this.EngineName.ToString();
        }
    }
}
