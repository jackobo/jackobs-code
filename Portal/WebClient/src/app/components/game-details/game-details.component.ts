import { GameDetailsNavigator } from './game-details.navigator';
import { AppNavigationProvider } from './../../app-navigation-provider';
import { GameVersionsNavigator } from './../game-versions/game-versions.navigator';
import { Observable } from 'rxjs/Observable';
import { PlatformType, GameTechnology, GameInfrastructure } from 'app/entities/enums';
import { Game, GameType} from './../../entities/game';
import { GamesProviderService } from './../../services/games-provider.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import 'rxjs/add/operator/switchMap';


@Component({
    moduleId: module.id,
    selector: 'game-details',
    templateUrl: 'game-details.component.html'
})
export class GameDetailsComponent implements OnInit {
       


    constructor(private gamesProvider: GamesProviderService, 
                private route: ActivatedRoute, 
                private appNavigationProvider: AppNavigationProvider) { }

    game: Game;
    
    ngOnInit() {

        let routeParamsObservable = this.route.params
                                              .map((params: Params) => this.appNavigationProvider.gameDetailsNavigator(params))
                                              .switchMap(gameDetailsRouter => this.gamesProvider.getGame(gameDetailsRouter.gameId))
                                              .map(game => {
                                                this.game = game;
                                                return game;
                                              });
                    
        if(this.route.firstChild){
            routeParamsObservable.map(game => this.appNavigationProvider.gameDetailsNavigator(game.Id))
               .switchMap(gameDetailsRouter => {
                   return this.route.firstChild.params.map(childParams =>  gameDetailsRouter.detectChildRouter(childParams).gameInfra)
               })
               .subscribe(gameInfra => this.selectedGameInfra = gameInfra);
        }
        else{
            routeParamsObservable.subscribe(game => {
                this.navigateToVersions(this.game.SupportedInfrastructures[0]);
            })
        }
    }

    navigateToVersions(gameInfra: GameInfrastructure) {
        this.appNavigationProvider.gameDetailsNavigator(this.game.Id)
                               .gameVersionsNavigator(gameInfra)
                               .navigate();
        this.selectedGameInfra = gameInfra;
    }


    selectedGameInfra: GameInfrastructure;

    isGameInfraSelected(gameInfra: GameInfrastructure ): boolean {
        if (!this.selectedGameInfra) {
            return false;
        }

        return this.selectedGameInfra.equalsTo(gameInfra);
        
    }

    getOtherGameTypes() : GameType[]{
        if(!this.game){
            return [];
        }

        return this.game.GameTypes.filter(gt => gt.Id != this.game.MainGameType);
    }
}