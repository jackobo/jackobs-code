using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using GamesPortal.Service.Helpers;
using Spark.Infra.Types;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using static GamesPortal.Service.Helpers.MockRecordsFactory;
using NSubstitute;

namespace GamesPortal.Service
{
    [TestFixture]
    public class GamesPortalApprovalService_QAApproveTests : GamesPortalApprovalService_ApproveTests<QAApproveRequest>
    {
        protected override string GetPropertyValue()
        {
            return WellKnownNamesAndValues.True;
        }

        protected override string GetPropertyName()
        {
            return WellKnownNamesAndValues.QAApproved;
        }

        protected override QAApproveRequest CreateApproveRequest()
        {
            return CreateApproveRequest(Guid.NewGuid());
        }
        

        protected override void Approve(GamesPortalApprovalService service, QAApproveRequest request)
        {
            service.QAApprove(request);
        }

        protected override QAApproveRequest CreateApproveRequest(Guid gameVersionId, params string[] regulations)
        {
            return new QAApproveRequest(gameVersionId, regulations);
        }


        protected QAApproveRequest CreateApproveRequest(Guid versionID, string regulation = "Gibraltar", params string[] clientTypes)
        {
            return new QAApproveRequest(versionID,
                                        regulation);
        }
    }
}
