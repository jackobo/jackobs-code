import { LatestApprovedGameVersionForRegulation } from './../entities/latest-approved-game-version-for-each-regulation';
import { environment } from './../../environments/environment';
import { GameVersion } from './../entities/game-version';
import { Injectable } from '@angular/core';
import { Http, RequestOptionsArgs } from '@angular/http';
import { Game, GameType } from './../entities/game';
import { Observable } from "rxjs/Observable";


@Injectable()
export class GamesProviderService {

    constructor(private http: Http) { }
    
    
    private actionUrl(actionName: string): string{
        return environment.gamesPortalApi + actionName;
    }

    getAllGames(): Observable<Game[]> {
        return this.http.get(this.actionUrl('GetAllGames'))
            .map(response => {
                return response.json().Games
                    .map(g => Game.fromJson(g));
            });
    }

    getGame(id: string): Observable<Game> {
        return this.http.get(this.actionUrl(`GetGame?gameId=${id}`))
            .map(response => Game.fromJson(response.json().Game));
    }

    getGameVersions(id: string): Observable<GameVersion[]> {
        return this.http.get(this.actionUrl(`GetGameVersions?gameId=${id}`))
            .map(response => response.json().GameVersions.map(gv => GameVersion.fromJson(gv)));
    }

    getLatestApprovedGameVersionForEachRegulation() : Observable<LatestApprovedGameVersionForRegulation[]>{
         return this.http.get(this.actionUrl('GetLatestApprovedGameVersionForEachRegulation'))
            .map(response => response.json().LatestApprovedGamesVersions.map(item => LatestApprovedGameVersionForRegulation.fromJson(item)));
    }
}