using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;

namespace GamesPortal.Client.Models
{
    internal class MandatoryLanguagesPerRegulationProvider : IMandatoryLanguagesProvider
    {
        Dictionary<RegulationType, HashSet<Language>> _mandatoryLanguagesPerRegulation = new Dictionary<RegulationType, HashSet<Language>>();
        public MandatoryLanguagesPerRegulationProvider()
        {

        }


        public void AddMandatoryLanguages(RegulationType regulation, IEnumerable<Language> mandatoryLanguages)
        {
            if (!_mandatoryLanguagesPerRegulation.ContainsKey(regulation))
                _mandatoryLanguagesPerRegulation.Add(regulation, new HashSet<Language>());

            foreach (var l in mandatoryLanguages)
                _mandatoryLanguagesPerRegulation[regulation].Add(l);
        }

        public IEnumerable<Language> GetManatoryLanguages(RegulationType regulation)
        {
            if (_mandatoryLanguagesPerRegulation.ContainsKey(regulation))
                return _mandatoryLanguagesPerRegulation[regulation];
            else
                return new Language[0];
        }
    }
}
