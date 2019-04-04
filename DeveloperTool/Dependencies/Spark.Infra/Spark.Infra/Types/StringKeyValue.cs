using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Types
{

    public class StringKeyValue
    {
        public StringKeyValue(string name, string value)
        {
            Init(name, value, null);
        }

        public StringKeyValue(string name, string value, int lineNumber)
        {
            Init(name, value, lineNumber);
        }

        private void Init(string name, string value, int? lineNumber)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.Name = name;
            this.Value = value ?? string.Empty;
            this.LineNumber = lineNumber;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }

        public int? LineNumber { get; private set; }


        public string[] GetMultiValue(string separator = ",")
        {
            return this.Value.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        }

        public override string ToString()
        {
            return this.Name + " = " + this.Value;
        }

        public override bool Equals(object obj)
        {
            StringKeyValue theOther = obj as StringKeyValue;

            if (theOther == null)
                return false;

            return this.Name == theOther.Name && this.Value == theOther.Value;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Value.GetHashCode();
        }

        public static void AppendTo(StringBuilder stringBuilder, string key, string value)
        {
            stringBuilder.Append(key);
            stringBuilder.Append(" = ");
            stringBuilder.Append(value);
            stringBuilder.AppendLine();
        }
    }
}
