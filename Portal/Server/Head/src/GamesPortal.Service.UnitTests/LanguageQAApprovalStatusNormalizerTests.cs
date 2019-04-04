using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Helpers;
using static GamesPortal.Service.Helpers.MockRecordsFactory;
using NUnit.Framework;
using NSubstitute;

namespace GamesPortal.Service.Synchronizers
{
    [TestFixture]
    public class LanguageQAApprovalStatusUpdaterTests
    {
        [SetUp]
        public void Setup()
        {
            _internalServices = Helpers.GamesPortalInternalServicesHelper.Create();
            _dataContext = _internalServices.CreateGamesPortalDBDataContext();
            _normalizer = new GamesLanguageQAApprovalStatusNormalizer(_internalServices);
        }

        IGamesPortalInternalServices _internalServices;
        IGamesPortalDataContext _dataContext;
        GamesLanguageQAApprovalStatusNormalizer _normalizer;
        
       

        [Test]
        public void NormalizeApprovalStatusForAllLanguagesWithTheSameHash_ShouldCallNormalizeMethodInTheDataContext()
        {
         
            _normalizer.NormalizeApprovalStatusForAllLanguagesWithTheSameHash();

            _dataContext.Received().NormalizeApprovalStatusForAllLanguagesWithTheSameHash();
        }
    }
}
