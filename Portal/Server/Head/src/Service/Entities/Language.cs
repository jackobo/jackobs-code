using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace GamesPortal.Service.Entities
{
    public class Language : SmartEnum<string, Language>
    {
        public Language(string id, string name, string iso2, string iso3_1, string iso3_2 = null)
            : base(id, name)
        {
            Iso2 = iso2;
            Iso3_1 = iso3_1;
            Iso3_2 = iso3_2;

        }

        public string Iso2 { get; private set; }
        public string Iso3_1 { get; private set; }
        public string Iso3_2 { get; private set; }


        public LanguageDTO ToLanguageDTO()
        {
            return new LanguageDTO(this.Name, this.Iso2, this.Iso3_1);
        }

        public static readonly Language English = new Language("0", "English", "en", "eng", null);
        public static readonly Language ChineseTraditional = new Language("1", "Chinese Traditional", "zh", "chi", "zho");
        public static readonly Language ChineseSimplified = new Language("2", "Chinese Simplified", "zh", "chi", "zho");
        public static readonly Language Japanese = new Language("3", "Japanese", "ja", "jpn", null);
        public static readonly Language Korean = new Language("4", "Korean", "ko", "kor", null);
        public static readonly Language French = new Language("5", "French", "fr", "fre", "fra");
        public static readonly Language Italian = new Language("6", "Italian", "it", "ita", null);
        public static readonly Language German = new Language("7", "German", "de", "ger", "deu");
        public static readonly Language Spanish = new Language("8", "Spanish", "es", "spa", null);
        public static readonly Language Portuguese = new Language("9", "Portuguese", "pt", "por", null);
        public static readonly Language Dutch = new Language("10", "Dutch", "nl", "dut", "nld");
        public static readonly Language Danish = new Language("11", "Danish", "da", "dan", null);
        public static readonly Language Turkish = new Language("12", "Turkish", "tr", "tur", null);
        public static readonly Language English_UK = new Language("13", "English-UK", "en", "eng", null);
        public static readonly Language Swedish = new Language("14", "Swedish", "sv", "swe", null);
        public static readonly Language Greek = new Language("15", "Greek", "el", "gre", "ell");
        public static readonly Language Hungarian = new Language("17", "Hungarian", "hu", "hun", null);
        public static readonly Language Russian = new Language("18", "Russian", "ru", "rus", null);
        public static readonly Language Romanian = new Language("19", "Romanian", "ro", "rum", "ron");
        public static readonly Language Polish = new Language("26", "Polish", "pl", "pol", null);
        public static readonly Language Hindi = new Language("27", "Hindi", "hi", "hin", null);
        public static readonly Language Icelandic = new Language("28", "Icelandic", "is", "ice", "isl");
        public static readonly Language Slovenian = new Language("29", "Slovenian", "sl", "slv", null);
        public static readonly Language Bulgarian = new Language("34", "Bulgarian", "bg", "bul", null);
        public static readonly Language Malay = new Language("35", "Malay", "ms", "may", "msa");
        public static readonly Language Czech = new Language("36", "Czech", "cz", "cze", "ces");
        public static readonly Language Lithuanian = new Language("37", "Lithuanian", "lt", "lit", null);
        public static readonly Language Latvian = new Language("38", "Latvian", "lv", "lav", null);
        public static readonly Language Estonian = new Language("39", "Estonian", "et", "est", null);
        public static readonly Language Norwegian = new Language("44", "Norwegian", "no", "nor", null);
        public static readonly Language Finnish = new Language("45", "Finnish", "fi", "fin", null);
        public static readonly Language English_AUS = new Language("49", "English-AUS", "en", "eng", null);
        public static readonly Language Slovak = new Language("57", "Slovak", "sk", "svk", null);
        public static readonly Language Albanian = new Language("58", "Albanian", "sq", "sqi", null);
        public static readonly Language Arabic = new Language("69", "Arabic", "AR", "ARA", "ARA");
        public static readonly Language Serbian = new Language("70", "Serbian", "SR", "SRP", "SRP");

        private static object _allLanguageCodesSync = new object();
        private static ConcurrentDictionary<string, Language> _allLanguageCodes = null;

        private static ConcurrentDictionary<string, Language> AllLanguageCodes
        {
            get
            {
                if(_allLanguageCodes == null)
                {
                    lock(_allLanguageCodesSync)
                    {
                        if (_allLanguageCodes == null)
                        {
                            _allLanguageCodes = new ConcurrentDictionary<string, Language>(StringComparer.OrdinalIgnoreCase);
                            foreach (var lang in All)
                            {
                                foreach(var langCode in lang.GetCodes())
                                {
                                    _allLanguageCodes.TryAdd(langCode, lang);
                                }
                            }
                        }
                    }
                }

                return _allLanguageCodes;

            }
        }

        public IEnumerable<string> GetCodes()
        {
            return new string[]
            {
                this.Id,
                this.Iso2,
                this.Iso3_1,
                this.Iso3_2,
                this.Name
            }
            .Where(code => !string.IsNullOrEmpty(code))
            .ToArray();
        }
        
        
        public static Language Find(string anyCode)
        {
            var lang = TryFind(anyCode);
            if (lang.Any())
            {
                return lang.First();
            }

            throw new ArgumentException(string.Format("Cannot find the language with code {0}", anyCode));
        }

        public static Optional<Language> TryFind(string anyCode)
        {
            if (AllLanguageCodes.ContainsKey(anyCode))
                return Optional<Language>.Some(AllLanguageCodes[anyCode]);
            else
                return Optional<Language>.None();
        }
        
    }
}
