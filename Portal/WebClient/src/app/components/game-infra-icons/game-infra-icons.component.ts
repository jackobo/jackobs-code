import { GameInfrastructure, PlatformType, GameTechnology } from 'app/entities/enums';
import { Component, OnInit, Input } from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'game-infra-icons',
    templateUrl: 'game-infra-icons.component.html',
    styles:[`
        img{
            width: 20px;
            height: 20px;
        }
    `]
})
export class GameInfraIconsComponent implements OnInit {

    constructor() { }

    ngOnInit() { 
        
    }

    isPC(): boolean{
        if(this.infra.platform === PlatformType.PcAndMobile)
            return true;

        return this.infra.platform === PlatformType.PC;
    }

    isMobile(): boolean{
        if(this.infra.platform === PlatformType.PcAndMobile)
            return true;

        return this.infra.platform === PlatformType.Mobile;
    }

    isHtml5(): boolean{
        return this.infra.technology === GameTechnology.Html5;
    }

    isFlash(): boolean{
        return this.infra.technology === GameTechnology.Flash;
    }

    @Input()
    infra: GameInfrastructure;  
}