using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Types
{
    public class StringKeyValueCollection : System.Collections.ObjectModel.KeyedCollection<string, StringKeyValue>
    {
        public StringKeyValueCollection()
        {

        }

        protected override string GetKeyForItem(StringKeyValue item)
        {
            return item.Name;
        }

        public static StringKeyValueCollection Parse(string text)
        {
            return Parse(text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None));
        }

        public static StringKeyValueCollection Parse(string[] lines)
        {
            StringKeyValueCollection properties = new StringKeyValueCollection();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                line = line.Trim();


                var keyValue = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                if (keyValue.Length == 0)
                    continue;

                string propertyName = keyValue[0].Trim();
                string propertyValue = null;

                if (keyValue.Length == 2)
                    propertyValue = keyValue[1].Trim();


                if (properties.Contains(propertyName))
                    throw new ArgumentException(string.Format("Duplicate property name {0}", propertyName));
                else
                    properties.Add(new StringKeyValue(propertyName, propertyValue, i));

            }

            return properties;
        }


        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.Select(p => p.ToString()));
        }


        public string GetPropertyValueOrNull(string propertyName)
        {
            if (!this.Contains(propertyName))
                return null;

            return this[propertyName].Value;
        }

        public Optional<StringKeyValue> TryGetProperty(string propertyName, bool ignoreCase = false)
        {
            var prop = this.FirstOrDefault(p => 0 == string.Compare(p.Name, propertyName, ignoreCase));
            
            if (prop == null)
                return Optional<StringKeyValue>.None();
            else
                return Optional<StringKeyValue>.Some(prop);
            
        }

        public string[] GetMultiValue(string propertyName)
        {
            if (!this.Contains(propertyName))
                return new string[0];

            return this[propertyName].GetMultiValue();
        }
    }
}
