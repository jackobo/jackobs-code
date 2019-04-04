using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Entities;
using NUnit.Framework;
using NSubstitute;
using GamesPortal.Service.Helpers;
using static GamesPortal.Service.Helpers.MockRecordsFactory;
using GamesPortal.Service.Artifactory;

namespace GamesPortal.Service
{
    [TestFixture]
    public class GamesPortalApprovalService_PMApproveTests : GamesPortalApprovalService_ApproveTests<PMApproveRequest>
    {
        protected override string GetPropertyValue()
        {
            return WellKnownNamesAndValues.True;
        }

        protected override string GetPropertyName()
        {
            return WellKnownNamesAndValues.PMApproved;
        }

        protected override PMApproveRequest CreateApproveRequest()
        {
            return CreateApproveRequest(Guid.NewGuid());
        }

        protected override PMApproveRequest CreateApproveRequest(Guid gameVersionId, params string[] regulations)
        {
            if (!regulations.Any())
                regulations = new string[] { "Gibraltar" };

            return new PMApproveRequest(gameVersionId,
                                       regulations);
        }

      

        protected override void Approve(GamesPortalApprovalService service, PMApproveRequest request)
        {
            service.PMApprove(request);
        }
        
    }
}
