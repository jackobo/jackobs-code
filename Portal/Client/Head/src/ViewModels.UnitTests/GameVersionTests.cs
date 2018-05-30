using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using NSubstitute;
using NUnit.Framework;

namespace GamesPortal.Client.ViewModels
{
    [TestFixture]
    public class GameVersionTests
    {
        [Test]
        public void GetMissingMandatoryLanguagesPerRegulations_IfNoLanguageIsMissing_ReturnsEmptyArray()
        {
            var regulationType = RegulationType.GetRegulation("888Italy");
            var italianLang = new Language("Italian", "it", "ita");
            GameVersion gameVersion = Helpers.GameVersionBuilder.GameVersion()
                                             .WithRegulation(Helpers.GameVersionRegulationBuilder.GameVersionRegulation()
                                                                                                 .WithRegulation(regulationType)
                                                                                                 .WithClientType("NDL")
                                                                                                 .WithLanguage(italianLang)
                                                                                                 .Build())
                                             .Build();

            var mandatoryLanguagesProvider = Substitute.For<IMandatoryLanguagesProvider>();

            
            mandatoryLanguagesProvider.GetManatoryLanguages(regulationType).Returns(new Language[] { italianLang });

            var missingMandatoryLanguages = gameVersion.GetMissingMandatoryLanguagesPerRegulations(mandatoryLanguagesProvider);
            Assert.AreEqual(0, missingMandatoryLanguages.Length);
            
        }

        [Test]
        public void GetMissingMandatoryLanguagesPerRegulations_IfALanguageIsMissing_ReturnsOneRegulationWithOneLanguage()
        {
            var regulationType = RegulationType.GetRegulation("888Italy");
            GameVersion gameVersion = Helpers.GameVersionBuilder.GameVersion()
                                             .WithRegulation(Helpers.GameVersionRegulationBuilder.GameVersionRegulation()
                                                                                                 .WithRegulation(regulationType)
                                                                                                 .WithClientType("NDL")
                                                                                                 .Build())
                                             .Build();

            var mandatoryLanguagesProvider = Substitute.For<IMandatoryLanguagesProvider>();

            var italianLang = new Language("Italian", "it", "ita");
            mandatoryLanguagesProvider.GetManatoryLanguages(regulationType).Returns(new Language[] { italianLang });

            var missingMandatoryLanguages = gameVersion.GetMissingMandatoryLanguagesPerRegulations(mandatoryLanguagesProvider);
            Assert.AreEqual(1, missingMandatoryLanguages.Length);
            Assert.AreEqual(1, missingMandatoryLanguages[0].Languages.Length);
            Assert.AreEqual(regulationType , missingMandatoryLanguages[0].RegulationType);
            Assert.AreEqual(italianLang, missingMandatoryLanguages[0].Languages.First());

        }

        [Test]
        public void GetMissingMandatoryLanguagesPerRegulations_IfTwoLanguagesAreMissingForTheSameRegulation_ReturnsOneRegulationWithTwoLanguages()
        {
            var regulationType = RegulationType.GetRegulation("888Italy");
            GameVersion gameVersion = Helpers.GameVersionBuilder.GameVersion()
                                             .WithRegulation(Helpers.GameVersionRegulationBuilder.GameVersionRegulation()
                                                                                                 .WithRegulation(regulationType)
                                                                                                 .WithClientType("NDL")
                                                                                                 .Build())
                                             .Build();

            var mandatoryLanguagesProvider = Substitute.For<IMandatoryLanguagesProvider>();

            var italianLang = new Language("Italian", "it", "ita");
            var englishLang = new Language("English", "en", "eng");
            mandatoryLanguagesProvider.GetManatoryLanguages(regulationType).Returns(new Language[] { italianLang, englishLang });

            var missingMandatoryLanguages = gameVersion.GetMissingMandatoryLanguagesPerRegulations(mandatoryLanguagesProvider);
            Assert.AreEqual(1, missingMandatoryLanguages.Length);
            Assert.AreEqual(2, missingMandatoryLanguages[0].Languages.Length);
        }

        [Test]
        public void GetMissingMandatoryLanguagesPerRegulations_IfOneLanguagesIsMissingForTwoRegulations_ReturnsTowRegulationsEachOneWithOneLanguage()
        {
            var italyRegulation = RegulationType.GetRegulation("888Italy");
            var gibraltarRegulation = RegulationType.GetRegulation("Gibraltar");
            GameVersion gameVersion = Helpers.GameVersionBuilder.GameVersion()
                                             .WithRegulation(Helpers.GameVersionRegulationBuilder.GameVersionRegulation()
                                                                                                 .WithRegulation(italyRegulation)
                                                                                                 .WithClientType("NDL")
                                                                                                 .Build())
                                             .WithRegulation(Helpers.GameVersionRegulationBuilder.GameVersionRegulation()
                                                                                                 .WithRegulation(gibraltarRegulation)
                                                                                                 .WithClientType("NDL")
                                                                                                 .Build())
                                             .Build();

            var mandatoryLanguagesProvider = Substitute.For<IMandatoryLanguagesProvider>();

            var italianLang = new Language("Italian", "it", "ita");
            var englishLang = new Language("English", "en", "eng");
            mandatoryLanguagesProvider.GetManatoryLanguages(italyRegulation).Returns(new Language[] { italianLang});
            mandatoryLanguagesProvider.GetManatoryLanguages(gibraltarRegulation).Returns(new Language[] { englishLang });

            var missingMandatoryLanguages = gameVersion.GetMissingMandatoryLanguagesPerRegulations(mandatoryLanguagesProvider);
            Assert.AreEqual(2, missingMandatoryLanguages.Length);
            Assert.AreEqual(1, missingMandatoryLanguages[0].Languages.Length);
            Assert.AreEqual(1, missingMandatoryLanguages[1].Languages.Length);
        }
    }
}
