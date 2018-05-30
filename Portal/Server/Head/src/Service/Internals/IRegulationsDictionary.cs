using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service
{
    public interface IRegulationsDictionary
    {
        IRegulation GetRegulation(string regulation);
        void Refresh();
    }


    public interface IRegulation
    {
        int Id { get; }
        string Name { get; }
        bool IsClientTypeSupported(string clientType);
        string[] GetSupportedClientTypes();
        bool IsLanguageMandatory(Language language);
    }


    internal class Regulation : IRegulation
    {

        public Regulation(int id, string name, 
                          IEnumerable<string> supportedClientTypes, 
                          IEnumerable<Language> mandatoryLanguages)
        {
            this.Id = id;
            this.Name = name;
            _supportedClientTypes = new HashSet<string>(supportedClientTypes);
            _mandatoryLanguages = new HashSet<Language>(mandatoryLanguages);
        }

        HashSet<string> _supportedClientTypes;
        HashSet<Language> _mandatoryLanguages;

        public int Id { get; private set; }
        
        public string Name { get; private set; }

        public bool IsClientTypeSupported(string clientType)
        {
            return _supportedClientTypes.Contains(clientType);
        }

        public string[] GetSupportedClientTypes()
        {
            return _supportedClientTypes.ToArray();
        }

        public bool IsLanguageMandatory(Language language)
        {
            return _mandatoryLanguages.Contains(language);
        }
    }


    public class RegulationsDictionary : IRegulationsDictionary
    {
        public RegulationsDictionary(IGamesPortalInternalServices services)
        {
            _services = services;
            Refresh();
        }

        public void Refresh()
        {
            this.Regulations = new Lazy<Dictionary<string, IRegulation>>(() => ReadRegulations().ToDictionary(r => r.Name));
        }

        private IEnumerable<IRegulation> ReadRegulations()
        {
            var result = new List<IRegulation>();
            using (var dbContext = _services.CreateGamesPortalDBDataContext())
            {
                var clientsThatApplyToAllRegulations = dbContext.GetTable<ClientTypeToRegulationMapping>()
                                                                .Where(row => string.Compare(row.Regulation, "All") == 0)
                                                                .Select(row => row.ClientType)
                                                                .ToArray();

                var clientsThatApplyToSpecificRegulations = dbContext.GetTable<ClientTypeToRegulationMapping>()
                                                                     .Where(row => string.Compare(row.Regulation, "All") != 0)
                                                                     .GroupBy(row => row.Regulation)
                                                                     .ToDictionary(group => group.Key, group => group.Select(item => item.ClientType).ToArray());



               foreach (var regulationTypeRow in dbContext.GetTable<RegulationType>())
               {
                    string[] specificClientTypes = null;
                    clientsThatApplyToSpecificRegulations.TryGetValue(regulationTypeRow.RegulationName, out specificClientTypes);

                    result.Add(new Regulation(regulationTypeRow.RegulationType_ID,
                                              regulationTypeRow.RegulationName,
                                              clientsThatApplyToAllRegulations.Union(specificClientTypes ?? new string[0]),
                                              regulationTypeRow.RegulationType_MandatoryLanguages.Select(mandatoryLang => _services.LanguageDictionary.FindLanguage(mandatoryLang.LanguageIso3))));
               }
            }

            return result;
        }

        IGamesPortalInternalServices _services;
        
        Lazy<Dictionary<string, IRegulation>> Regulations { get; set; }
        
        public IRegulation GetRegulation(string regulation)
        {
            return Regulations.Value[regulation];
        }
    }
}
