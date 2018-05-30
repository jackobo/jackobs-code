import { DownloadInfo } from './download-info';
import { UploadInfo } from './upload-info';
import { ApprovalInfo } from './approval-info';
import { Optional } from './../utils/optional';
export class GameVersionRegulation {
    constructor(public regulationName: string,
        public qaApprovalInfo: Optional<ApprovalInfo>,
        public pmApprovalInfo: Optional<ApprovalInfo>,
        public productionUploadInfo: Optional<UploadInfo>,
        public downloadInfo: DownloadInfo) {

    }

    inProgress(): boolean {
        return !this.readyForProduction() && !this.inProduction() && !this.partialyApproved();
    }

    partialyApproved() : boolean{
        return !this.readyForProduction() 
                &&  (this.qaApprovalInfo.any() || this.pmApprovalInfo.any());
    }


    readyForProduction(): boolean {
        return this.qaApprovalInfo.any() && this.pmApprovalInfo.any();
    }

    inProduction(): boolean {
        return this.productionUploadInfo.any();
    }


    static fromJson(data: any): GameVersionRegulation {
        return new GameVersionRegulation(data.Regulation,
            ApprovalInfo.fromJson(data.QAApprovalInfo),
            ApprovalInfo.fromJson(data.PMApprovalInfo),
            UploadInfo.fromJson(data.ProductionUploadInfo),
            DownloadInfo.fromJson(data.DownloadInfo))
    }
}