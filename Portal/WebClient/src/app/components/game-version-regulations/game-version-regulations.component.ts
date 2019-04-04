import { GameVersionRegulation } from './../../entities/game-version-regulation';
import { Component, OnInit, Input } from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'game-version-regulations',
    templateUrl: 'game-version-regulations.component.html'
})
export class GameVersionRegulationsComponent implements OnInit {

    constructor() { }

    ngOnInit() { 

    }

    @Input() 
    regulations: GameVersionRegulation[] = [];

}