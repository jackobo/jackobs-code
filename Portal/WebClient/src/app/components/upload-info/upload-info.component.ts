import { Optional } from './../../utils/optional';
import { UploadInfo } from './../../entities/upload-info';
import { Component, OnInit, Input } from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'upload-info',
    templateUrl: 'upload-info.component.html'
})
export class UploadInfoComponent implements OnInit {

    constructor() { }

    ngOnInit() { 

    }

    @Input()
    title: string;

    @Input()
    uploadInfo: Optional<UploadInfo> = Optional.none<UploadInfo>();;
}