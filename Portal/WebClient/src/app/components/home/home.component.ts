import { AppNavigationProvider } from './../../app-navigation-provider';
import { Component, OnInit } from '@angular/core';
import {GameTechnology} from "../../entities/enums";

@Component({
    moduleId: module.id,
    selector: 'home',
    templateUrl: 'home.component.html'
})
export class HomeComponent implements OnInit {

    constructor(private appNavigationProvider: AppNavigationProvider) { }

    ngOnInit() {}

    html5GamesLatestApproved() {
        this.appNavigationProvider.html5GamesLatestApproved(GameTechnology.Html5).navigate();
    }

}
