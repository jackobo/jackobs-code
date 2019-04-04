using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces;
using GamesPortal.Client.Interfaces.Entities;

namespace GamesPortal.Client.Models
{
   


    internal class LanguageApprovalInfo : ILanguageApprovalInfo
    {
        public LanguageApprovalInfo(IApproval qaApprovalInfo, bool isMandatory, IProductionUploadInfo productionUploadInfo)
        {
            _qaApprovalInfo = qaApprovalInfo;
            this.IsMandatory = isMandatory;
            _productionUploadInfo = productionUploadInfo;
        }

        IProductionUploadInfo _productionUploadInfo;
        IApproval _qaApprovalInfo;
        
        public bool IsApproved
        {
            get
            {
                return _qaApprovalInfo.IsApproved ||
                       _productionUploadInfo.IsInProduction;
            }
        }
        
        public bool IsMandatory { get; private set; }
        public string Status
        {
            get
            {
                if (_productionUploadInfo.IsInProduction)
                    return "Production";

                if (_qaApprovalInfo.IsApproved)
                    return "Approved";

                if (this.IsMandatory)
                    return "Mandatory";

                return "InProgress";
            }
        }
    }

    internal class GameVersionApprovalInfo : IGameVersionApprovalInfo
    {
        public GameVersionApprovalInfo(IApproval qaApprovalInfo, 
                                       IApproval pmApprovalInfo, 
                                       IProductionUploadInfo productionUploadInfo)
        {
            this.QAApprovalInfo = qaApprovalInfo;
            this.PMApprovalInfo = pmApprovalInfo;
            this.ProductionUploadInfo = productionUploadInfo;
        }

        IApproval QAApprovalInfo { get; set; }

        IApproval PMApprovalInfo { get; set; }


        IProductionUploadInfo ProductionUploadInfo { get; set; }


        public string Status
        {
            get
            {
                if (ProductionUploadInfo.IsInProduction)
                    return ApprovalStatusesDescriptions.Production;

                if (QAApprovalInfo.IsApproved && PMApprovalInfo.IsApproved)
                    return ApprovalStatusesDescriptions.ApprovedForProduction;

                if (QAApprovalInfo.IsApproved)
                    return ApprovalStatusesDescriptions.QAApproved;

                if (PMApprovalInfo.IsApproved)
                    return ApprovalStatusesDescriptions.PMApproved;

                return ApprovalStatusesDescriptions.InProgress;
            }
        }

        public bool CanPMApprove
        {
            get
            {
                return !ProductionUploadInfo.IsInProduction
                    && !PMApprovalInfo.IsApproved;
            }
        }

        public bool CanQAApprove
        {
            get
            {
                return !ProductionUploadInfo.IsInProduction
                    && !QAApprovalInfo.IsApproved;
            }
        }

        public bool IsQAApproved
        {
            get { return QAApprovalInfo.IsApproved; }
        }

    }

    internal class Approved : IApproval
    {
        public Approved(DateTime approvalDate, string approvedBy)
        {
            this.ApprovalDate = approvalDate;
            this.ApprovedBy = approvedBy;
        }

        DateTime ApprovalDate { get; set; }

        string ApprovedBy { get; set; }



        public bool IsApproved
        {
            get
            {
                return true;
            }
        }

        DateTime? IApproval.ApprovalDate
        {
            get
            {
                return this.ApprovalDate;
            }
        }
    }
    
    public class NotApproved : IApproval
    {
        private NotApproved()
        {

        }

        public static readonly NotApproved Instance = new NotApproved();

        public bool IsApproved
        {
            get
            {
                return false;
            }
        }

        public string Description
        {
            get
            {
                return string.Empty;
            }
        }

        public DateTime? ApprovalDate
        {
            get
            {
                return null;
            }
        }
    }
}
