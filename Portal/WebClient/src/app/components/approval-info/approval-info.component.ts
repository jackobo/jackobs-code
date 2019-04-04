import { Component, OnInit, Input } from '@angular/core';

import { ApprovalInfo } from './../../entities/approval-info';
import { Optional } from './../../utils/optional';

@Component({
    moduleId: module.id,
    selector: 'approval-info',
    templateUrl: 'approval-info.component.html'
})
export class ApprovalInfoComponent implements OnInit {

    constructor() { 
        
    }

    ngOnInit() { 

    }

    @Input()
    title: string;
    @Input()
    approvalInfo: Optional<ApprovalInfo> = Optional.none<ApprovalInfo>();
}