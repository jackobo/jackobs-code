import { UploadInfo } from './upload-info';
import { ApprovalInfo } from './approval-info';
import { Optional } from './../utils/optional';

export class GameVersionRegulationLanguage{
    constructor(public languageName: string,
                public isMandatory: boolean,
                public qaApprovalInfo: Optional<ApprovalInfo>, 
                public productionUploadInfo: Optional<UploadInfo>){

    }

    static fromJson(data: any){
        return new GameVersionRegulationLanguage(data.Language.Name,
                                                data.IsMandatory,
                                                ApprovalInfo.fromJson(data.QaApprovalInfo),
                                                UploadInfo.fromJson(data.ProductionUploadInfo));
    }
}
