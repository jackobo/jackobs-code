using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service.Artifactory
{
    public class ArtifactoryProperty
    {
        public ArtifactoryProperty()
        {

        }

        public ArtifactoryProperty(string key, params string[] values)
        {
            this.Key = key;
            this.Values = values;
        }

      

        public string Key { get; set; }
        public string[] Values { get; set; }

        public string Name
        {
            get
            {
                return GetPropertyNameFromKey(this.Key);
            }
        }

        public string SetName
        {
            get
            {
                return GetSetNameFromKey(this.Key);
            }
        }


        public static string GetSetNameFromKey(string key)
        {
            var propertyKeyComponents = key.Split('.');
            if (propertyKeyComponents.Length == 1)
            {
                return null;
            }
            else
            {
                return string.Join(".", propertyKeyComponents.Take(propertyKeyComponents.Length - 1));
            }
        }
        
        public static string GetPropertyNameFromKey(string key)
        {
            var propertyKeyComponents = key.Split('.');
            if (propertyKeyComponents.Length == 1)
            {
                return propertyKeyComponents[0];
            }
            else
            {
                return propertyKeyComponents[propertyKeyComponents.Length - 1];
            }
        }

        public bool HasPropertySet
        {
            get { return !string.IsNullOrEmpty(this.SetName); }
        }

      

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.Key, this.ConcatValues());
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as ArtifactoryProperty;

            if (theOther == null)
                return false;

            return this.Key == theOther.Key
                    && this.ConcatValues() == theOther.ConcatValues();
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        public string ConcatValues()
        {
            return string.Join(";", this.Values);
        }


        public string BuildUniqueID(string regulation)
        {
            return BuildUniqueID(regulation, this.Key);
        }


        public static string BuildUniqueID(string regulation, string key)
        {
            return regulation.ToLowerInvariant() + "." + key.ToLowerInvariant();
        }

        public static bool IsPropertySupportedForRegulation(string propertyKey, string regulation, Dictionary<string, List<string>> supportedRegulationPerClient)
        {
            if (string.IsNullOrEmpty(propertyKey))
                return true;

            var propertySetName = GetSetNameFromKey(propertyKey);

            if (string.IsNullOrEmpty(propertySetName))
            {
                return true;
            }

            if (!supportedRegulationPerClient.ContainsKey(propertySetName))
                return true;

            return supportedRegulationPerClient[propertySetName].Contains(regulation)
                   || supportedRegulationPerClient[propertySetName].Contains("All");
        }
        
    }   
    
    
    public class LanguageProperty : ArtifactoryProperty
    {
        private LanguageProperty(string key, string hash)
            : base(key, hash)
        {
        }


        public string Hash
        {
            get { return this.ConcatValues(); }
        }

        public string Language
        {
            get
            {
                return Key.Split('.').Last();
            }
        }

        public static bool IsLanguageHash(string propertyKey)
        {
            return propertyKey.Contains("LanguageHash.");
        }


        public static LanguageProperty FromKey(string key, string hash)
        {
            if (!IsLanguageHash(key))
                throw new ArgumentException($"Provided property key {key} is not a language property");

            return new LanguageProperty(key, hash);
        }

        public static LanguageProperty FromLanguage(string language, string hash)
        {
            return new LanguageProperty("LanguageHash." + language, hash);
        }
        

        public static ArtifactoryProperty BuildLanguageQaApprovedProperty(params string[] languages)
        {
            return new ArtifactoryProperty(Language_QAApproved, languages);
        }

        public static ArtifactoryProperty BuildLanguageProductionProperty(params string[] languages)
        {
            return new ArtifactoryProperty(Language_Production, languages);
        }

        public static readonly string Language_QAApproved = "Language.QAApproved";
        public static readonly string Language_Production = "Language.Production";
    }
     
    
    public class ArtifactoryPropertyCollection : List<ArtifactoryProperty>
    {
        public ArtifactoryPropertyCollection()
        {

        }

        public ArtifactoryPropertyCollection(IEnumerable<ArtifactoryProperty> items)
            : base(items)
        {

        }

        public bool Contains(string propertyName)
        {
            return this.Any(p => p.Name == propertyName);
        }
    }
}
