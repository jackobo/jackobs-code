using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;


namespace LayoutTool.Models.JsonEntities
{
    internal class NDL
    {
        public List<Language> languages = new List<Language>();
        public List<Brand> brands = new List<Brand>();
        public List<Group> groups = new List<Group>();
        public List<QAClientVersion> versions = new List<QAClientVersion>();
        public List<QAEnvironment> environments = new List<QAEnvironment>();

        internal Group GetGroupForBrand(Brand brand)
        {
            foreach (var g in groups)
            {
                if (g.id == brand.group)
                    return g;
            }

            return null;
        }
    }


    public class QAClientVersion
    {
        public QAClientVersion()
        {
            this.brandsGroups = new List<BrandsGroup>();
        }
        public string id;
        public string value;
        public List<BrandsGroup> brandsGroups;
    }

    public class QAEnvironment
    {
        public QAEnvironment()
        {
            this.versions = new List<QAEnvironmentVersion>();
        }
        public string id;
        public string value;
#warning can be many !!!!
        [JsonConverter(typeof(PathJsonConverter))]
        public string path;
        public List<QAEnvironmentVersion> versions;
    }

    public class QAEnvironmentVersion
    {
        public string id;
    }
    
    public class BrandsGroup
    {
        public string id;
    }

    public class Brand
    {
        public string value;
        public string id;
        //public string site;
        public string group;
        public List<Skin> skins;

        public override bool Equals(object obj)
        {
            var theOther = obj as Brand;

            if (theOther == null)
                return false;

            return this.id == theOther.id;
        }

        public override int GetHashCode()
        {
            return (this.id ?? string.Empty).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.value, this.id);
        }
    }

    public class Skin
    {
        public string value;
        public string id;
        
        [JsonConverter(typeof(LanguagesJsonConverter))]
        public List<Language> languages;
        
        public override bool Equals(object obj)
        {
            var theOther = obj as Skin;

            if (theOther == null)
                return false;

            return this.id == theOther.id;
        }

        public override int GetHashCode()
        {
            return (this.id ?? string.Empty).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.value, this.id);
        }
        
    }


    public class Group
    {
        public string id;
        public string domain;



        public override bool Equals(object obj)
        {
            var theOther = obj as Group;

            if (theOther == null)
                return false;

            return this.id == theOther.id;
        }

        public override int GetHashCode()
        {
            return (this.id ?? string.Empty).GetHashCode();
        }

        public override string ToString()
        {
            return this.domain;
        }

    }

    public class Language
    {
        public string value;
        public string id;

        public override bool Equals(object obj)
        {
            var theOther = obj as Language;

            if (theOther == null)
                return false;

            return this.id == theOther.id;
        }

        public override int GetHashCode()
        {
            return (this.id ?? string.Empty).GetHashCode();
        }

        public override string ToString()
        {
            return this.value;
        }

    }

    public class PathJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if(token.Type == JTokenType.Object)
            {
                var p = string.Join(";", token.Values().Select(t => t.ToString()));

                return p;
                
            }

            return token.ToString();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class LanguagesJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            var languagesStr = token.ToString();

            if (languagesStr == "all")
                return null;

            return JsonConvert.DeserializeObject<List<Language>>(languagesStr);


        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
