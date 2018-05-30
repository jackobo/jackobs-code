export class DownloadInfo {
    constructor(public url: string) {

    }

    static fromJson(data: any): DownloadInfo {
        return new DownloadInfo(data.Uri);
    }
}
