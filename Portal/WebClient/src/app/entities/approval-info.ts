import { Optional } from './../utils/optional';
import { DateUtils } from "app/utils/date-utils";
export class ApprovalInfo {
    constructor(public approvalDate: Date, public approvedBy: string) {

    }

 static fromJson(data: any): Optional<ApprovalInfo> {
        if (data) {
                return Optional.some<ApprovalInfo>(new ApprovalInfo(DateUtils.parse(data.ApprovalDate), data.ApprovedBy));
        }
        else {
            return Optional.none<ApprovalInfo>();
        }

    }
}

