using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Client.Interfaces
{
    public static class ApprovalStatusesDescriptions
    {
        public static readonly string InProgress = "In Progress";
        public static readonly string QAApproved = "QA Approved";
        public static readonly string PMApproved = "PM Approved";
        public static readonly string ApprovedForProduction = "Approved for PROD";
        public static readonly string Production = "Production";
    }
}
