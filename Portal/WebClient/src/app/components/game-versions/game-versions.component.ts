import { GameVersionsNavigator } from './game-versions.navigator';

import { GameDetailsNavigator } from './../game-details/game-details.navigator';
import { AppNavigationProvider } from './../../app-navigation-provider';
import { GameChangeHistoryNavigator } from './../game-change-history/game-change-history.navigator';
import { GameVersionRegulation } from './../../entities/game-version-regulation';
import { Observable } from 'rxjs/Observable';
import { GameVersion } from './../../entities/game-version';
import { GamesProviderService } from './../../services/games-provider.service';
import {GameTechnology, PlatformType, GameInfrastructure } from './../../entities/enums';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import 'rxjs/add/operator/switchMap';



@Component({
    moduleId: module.id,
    selector: 'game-versions',
    templateUrl: 'game-versions.component.html',
    styles:[`
        .view-history-link{
            float: right;
            line-height: 40px;
        }

        .section-title{
            line-height: 40px;
            margin: 0px;
        }

        :host >>> .popover {
            max-width: none;
        }

        :host >>> .btn-group {
            display: block;
        }
    `]
})
export class GameVersionsComponent implements OnInit, OnDestroy {


    constructor(private gamesProvider: GamesProviderService,
                private route: ActivatedRoute, 
                private router: Router, 
                private appNavigationProvider: AppNavigationProvider) { }

    ngOnInit() {
        this.gameVersions = this.route.params
            .switchMap((params: Params) => {
                        
                        let gameDetailsRouter = this.appNavigationProvider.gameDetailsNavigator(this.route.snapshot.parent.params);

                        this.gameId = gameDetailsRouter.gameId;
                        this.gameInfra =  gameDetailsRouter.gameChangeHistoryNavigator(params).gameInfra;

                        return this.gamesProvider.getGameVersions(this.gameId)
                                                .map(versions => versions.filter(gv => this.gameInfra.equalsTo(gv.gameInfra))
                                                                          .sort((v1, v2) => v2.version.toNumber() - v1.version.toNumber() ));
                        });
    }

    ngOnDestroy(): void {

    }

    gameVersions: Observable<GameVersion[]>;

    gameId:string;

    gameInfra: GameInfrastructure;

     navigateToHistory() : void{
        this.appNavigationProvider.gameDetailsNavigator(this.gameId)
                               .gameChangeHistoryNavigator(this.gameInfra)
                               .navigate();
    }
}


