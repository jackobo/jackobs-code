import { DateUtils } from './../utils/date-utils';
import { Optional } from './../utils/optional';
export class UploadInfo {
    constructor(public uploadDate: Date) {
    }

    static fromJson(data: any): Optional<UploadInfo> {
        if (data) {
            return Optional.some<UploadInfo>(new UploadInfo(DateUtils.parse(data.UploadDate)));
        }
        else {
            return Optional.none<UploadInfo>();
        }

    }
}
