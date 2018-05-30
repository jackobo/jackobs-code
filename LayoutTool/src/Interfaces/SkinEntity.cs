using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public class SkinEntity
    {
        public SkinEntity(int id, string name, params LanguageEntity[] languages)
        {
            this.Id = id;
            this.Name = name;
            if (languages == null || languages.Length == 0)
                this.Languages = new LanguageEntity[] { LanguageEntity.English };
            this.Languages = languages;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        

        private LanguageEntity GetPrefferedLanguage()
        {
            if (this.Languages.Any(l => l.Id == LanguageEntity.English.Id))
                return LanguageEntity.English;

            return this.Languages.First();
        }

        public LanguageEntity[] Languages { get; private set; }

        public override string ToString()
        {
            return $"{Name} [{Id}]";
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as SkinEntity;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id && this.Name == theOther.Name;
        }
    }

    public class LanguageEntity
    {
        public LanguageEntity(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static readonly LanguageEntity English = new LanguageEntity("en", "English");

        public override bool Equals(object obj)
        {
            var theOther = obj as LanguageEntity;

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
