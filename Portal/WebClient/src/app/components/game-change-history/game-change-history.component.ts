import { GameDetailsNavigator } from './../game-details/game-details.navigator';
import { GameChangeHistoryNavigator } from './game-change-history.navigator';
import { AppNavigationProvider } from './../../app-navigation-provider';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { GameInfrastructure, GameTechnology, PlatformType } from 'app/entities/enums';

import { GameVersionChangeHistory } from './../../entities/game-version-change-history';


import { VersionNumber } from './../../entities/version-number';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { GamesProviderService } from './../../services/games-provider.service';
import { GameVersionsNavigator } from "app/components/game-versions/game-versions.navigator";


@Component({
    moduleId: module.id,
    selector: 'game-change-history',
    templateUrl: 'game-change-history.component.html',
    styles:[`
        .view-versions-link{
            float:right;
            line-height: 40px;
        }

        .section-title{
            line-height: 40px
        }
    `]
})

export class GameChangeHistoryComponent implements OnInit {

    constructor(private gamesProvider: GamesProviderService, 
                private route: ActivatedRoute, 
                private router: Router,
                private appNavigationProvider: AppNavigationProvider) { }

    ngOnInit() { 
         this.historyRecords = this.route.params
                            .switchMap((params: Params) => {
                                        
                                        
                                        let gameDetailsRouter = this.appNavigationProvider.gameDetailsNavigator(this.route.snapshot.parent.params);
                                        let gameChangeHistoryRouter = gameDetailsRouter.gameChangeHistoryNavigator(params);

                                        this.gameId = gameDetailsRouter.gameId;
                                        this.gameInfra = gameChangeHistoryRouter.gameInfra;
                                        
                                        return this.gamesProvider.getGameVersions(this.gameId)
                                                                .map(versions => {


                                                                    let records: GameVersionChangeHistoryRecord[]  = [];

                                                                    for(let gameVersion of versions.filter(gv => this.gameInfra.equalsTo(gv.gameInfra)))
                                                                    {
                                                                        for(let historyRecord of gameVersion.history){
                                                                            records.push(new GameVersionChangeHistoryRecord(gameVersion.version,
                                                                                                                            gameVersion.gameInfra,
                                                                                                                            historyRecord))
                                                                        }
                                                                    }


                                                                    return records.sort((r1, r2) => Number(r2.historyRecord.changeDate) - Number(r1.historyRecord.changeDate));
                                                                });
                                        });
    }

    historyRecords: Observable<GameVersionChangeHistoryRecord[]>;

    gameId:string;

    gameInfra: GameInfrastructure;

    navigateToVersions() : void {
     
        this.appNavigationProvider.gameDetailsNavigator(this.gameId)
                               .gameVersionsNavigator(this.gameInfra)
                               .navigate();
    }
}



export class GameVersionChangeHistoryRecord{
    constructor(public version: VersionNumber, public gameInfra: GameInfrastructure, public historyRecord: GameVersionChangeHistory)
    {

    }
}