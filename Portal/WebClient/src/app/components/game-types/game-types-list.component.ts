import { GameType } from './../../entities/game';
import { Component, OnInit, Input } from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'game-types-list',
    templateUrl: 'game-types-list.component.html'
})
export class GameTypesListComponent implements OnInit {

    constructor() { }

    ngOnInit() {}

    @Input()
    gameTypes: GameType[] = [];

    isCollapsed :boolean = true;
}
