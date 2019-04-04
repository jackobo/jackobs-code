using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;

namespace GamesPortal.Service.Synchronizers
{
    public class GamesLanguageQAApprovalStatusNormalizer : IGamesLanguageQAApprovalStatusNormalizer
    {
        public GamesLanguageQAApprovalStatusNormalizer(IGamesPortalInternalServices services)
        {
            _services = services;
        }

        IGamesPortalInternalServices _services;

        public void NormalizeApprovalStatusForAllLanguagesWithTheSameHash()
        {
            using (var _dataContext = _services.CreateGamesPortalDBDataContext())
            {
                _dataContext.NormalizeApprovalStatusForAllLanguagesWithTheSameHash();
            }
            
        }
    }
}
