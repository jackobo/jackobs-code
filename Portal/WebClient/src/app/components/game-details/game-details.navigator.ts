import { Router, Params, ActivatedRoute } from '@angular/router';
import { GameInfrastructure } from './../../entities/enums';
import { GameChangeHistoryNavigator } from './../game-change-history/game-change-history.navigator';
import { GameVersionsNavigator } from './../game-versions/game-versions.navigator';
import { GameDetailsChildNavigator } from 'app/components/game-details/game-details-child.navigator';



export class GameDetailsNavigator{
    gameId: string;
    constructor(routeParams: any, private router: Router){
        if (typeof routeParams === 'string') {
            this.gameId = routeParams;
        } else {
            this.gameId = routeParams['id'];
        }
    }
    private getRouteParts(): string[] {
            return ['/gamedetails', this.gameId];
    }

    gameVersionsNavigator(routeParams: any): GameVersionsNavigator{
        return new GameVersionsNavigator(this.getRouteParts(), routeParams, this.router);
    }


    gameChangeHistoryNavigator(routeParams: any): GameChangeHistoryNavigator{
        return new GameChangeHistoryNavigator(this.getRouteParts(), routeParams, this.router);
    }
    detectChildRouter(params: Params) : GameDetailsChildNavigator{
        if (params['versions']) {
            return this.gameVersionsNavigator(params);
        } else {
            return this.gameChangeHistoryNavigator(params);
        }
    }
}

