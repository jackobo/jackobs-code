
import { DownloadInfo } from './download-info';
import { VersionNumber } from './version-number';
export class LatestVersionInfo{
    constructor(public versionId: string, public version: VersionNumber, public downloadInfo: DownloadInfo ){
    }

    static fromJson(json: any) : LatestVersionInfo{
        if(json){
            return new LatestVersionInfo(json.VersionId, 
                                         VersionNumber.parse(json.Version), 
                                         DownloadInfo.fromJson(json.DownloadInfo));
        }

        else{
            return null;
        }
    }
}