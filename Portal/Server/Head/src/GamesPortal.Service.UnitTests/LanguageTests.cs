using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Entities;
using NUnit.Framework;

namespace GamesPortal.Service
{
    [TestFixture]
    public class LanguageTests
    {
        [TestCase("15")]
        [TestCase("el")]
        [TestCase("gre")]
        [TestCase("ell")]
        [TestCase("ell")]
        [TestCase("Greek")]
        public void Find_ShouldReturnTheCorrectLanguage(string languageCode)
        {
            Assert.AreEqual(Language.Greek, Language.Find(languageCode));
        }
    }
}
