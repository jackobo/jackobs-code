
import { Router, Params } from '@angular/router';
import { Injectable } from '@angular/core';
import { GameDetailsNavigator } from './components/game-details/game-details.navigator';
import { Html5GamesLatestApprovedNavigator } from "./reports/games-latest-approved/games-latest-approved.navigator";
import {GameInfrastructure, GameTechnology} from "./entities/enums";

@Injectable()
export class AppNavigationProvider{
    constructor(private router: Router){

    }

    gameDetailsNavigator(routeParams: any): GameDetailsNavigator{
        return new GameDetailsNavigator(routeParams, this.router);
    }

    html5GamesLatestApproved(gameTechnology: GameTechnology): Html5GamesLatestApprovedNavigator{
        return new Html5GamesLatestApprovedNavigator(this.router, gameTechnology);
    }

    html5GamesLatestApprovedFromRouteParams(params: Params) {
      return new Html5GamesLatestApprovedNavigator(this.router, params['gameTechnology']);

    }
}
