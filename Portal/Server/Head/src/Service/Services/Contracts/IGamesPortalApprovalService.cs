using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service
{
    [ServiceContract]
    public interface IGamesPortalApprovalService
    {
        [OperationContract]
        GetAvailableQAApprovalStatesResponse GetAvailableQAApprovalStates();

        [OperationContract]
        GetAvailablePMApprovalStatesResponse GetAvailablePMApprovalStates();

        [OperationContract]
        void QAApprove(QAApproveRequest request);
        
        [OperationContract]
        void PMApprove(PMApproveRequest request);

        [OperationContract]
        void LanguageApprove(LanguageApproveRequest request);

        [OperationContract]
        ForceSynchronizationResponse ForceSynchronization();

        [OperationContract]
        void ForceGameSynchronization(ImplicitForceGameSynchronizationRequest request);

        [OperationContract]
        void ForceLanguageSynchronization(ForceLanguageSynchronizationRequest request);
    }

    [DataContract]
    public class ForceLanguageSynchronizationRequest
    {
        [DataMember]
        public int GameType { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public PlatformType PlatformType { get; set; }
        [DataMember]
        public GameTechnology GameTechnology { get; set; }
    }

    [DataContract]
    public class ImplicitForceGameSynchronizationRequest
    {
        public ImplicitForceGameSynchronizationRequest()
        {

        }

        [DataMember]
        public int MainGameType { get; set; }
        [DataMember]
        public bool isExternal { get; set; }


    }

    [DataContract]
    public class LanguageApproveRequest
    {
        public LanguageApproveRequest()
        {

        }


        public LanguageApproveRequest(Guid gameVersionId, params string[] languages)
        {
            this.GameVersionId = gameVersionId;
            this.Languages = languages;
        }

        [DataMember]
        public Guid GameVersionId { get; set; }
        [DataMember]
        public string[] Languages { get; set; }
    }

    [DataContract]
    public class ForceSynchronizationResponse
    {
        [DataMember]
        public bool SynchronizationAlreadyInProgress { get; set; }
    }

    [DataContract]
    public class GetAvailableQAApprovalStatesResponse
    {
        [DataMember]
        public string[] States{get;set;}
    }


    [DataContract]
    public class GetAvailablePMApprovalStatesResponse
    {
        [DataMember]
        public string[] States { get; set; }
    }

    [DataContract]
    public abstract class ApproveRequestBase
    {
        public ApproveRequestBase()
        {

        }

        public ApproveRequestBase(Guid gameVersionId, params string[] regulations)
        {
            this.GameVersionID = gameVersionId;
            this.Regulations = regulations;
        }

        [DataMember]
        public Guid GameVersionID { get; set; }
        

        [DataMember]
        public string[] Regulations { get; set; }

        public abstract ArtifactoryProperty GetArtifactoryProperty();

        public abstract void SetApprovalInfo(GameVersion_Regulation regulationRow, DateTime approvalDate, string approvalUser);
        
    }

    [DataContract]
    public class QAApproveRequest : ApproveRequestBase
    {
        public QAApproveRequest()
        {

        }

        public QAApproveRequest(Guid gameVersionId, params string[] regulations)
            : base(gameVersionId, regulations)
        {
        }

        public override ArtifactoryProperty GetArtifactoryProperty()
        {
            return new ArtifactoryProperty(WellKnownNamesAndValues.QAApproved, WellKnownNamesAndValues.True);
        }

        public override void SetApprovalInfo(GameVersion_Regulation regulationRow, DateTime approvalDate, string approvalUser)
        {
            regulationRow.QAApprovalDate = approvalDate;
            regulationRow.QAApprovalUser = approvalUser;
        }

    }

    [DataContract]
    public class PMApproveRequest : ApproveRequestBase
    {
        public PMApproveRequest()
        {

        }

        public PMApproveRequest(Guid gameVersionId, params string[] regulations)
            : base(gameVersionId, regulations)
        {
        }

        public override void SetApprovalInfo(GameVersion_Regulation regulationRow, DateTime approvalDate, string approvalUser)
        {
            regulationRow.PMApprovalDate = approvalDate;
            regulationRow.PMApprovalUser = approvalUser;
        }

        public override ArtifactoryProperty GetArtifactoryProperty()
        {
            return new ArtifactoryProperty(WellKnownNamesAndValues.PMApproved, WellKnownNamesAndValues.True);
        }
    }
}
