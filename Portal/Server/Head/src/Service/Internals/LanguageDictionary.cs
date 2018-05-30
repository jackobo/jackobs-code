using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Entities;
using Spark.Infra.Types;

namespace GamesPortal.Service
{
    public interface ILanguageDictionary
    {
        Language FindLanguage(string id);
        Optional<Language> TryFindLanguage(string id);
    }


    public class InMemoryLanguageDictionary : ILanguageDictionary
    {
        public Language FindLanguage(string id)
        {
            return Language.Find(id);
        }

        public Optional<Language> TryFindLanguage(string id)
        {
            return Language.TryFind(id);
        }
    }


}
