import { LatestVersionInfo } from './../../entities/latest-version-info';
import { VersionNumber } from './../../entities/version-number';
import {GameInfrastructure, GameTechnology, PlatformType} from 'app/entities/enums';
import { LatestApprovedGameVersionForRegulation } from './../../entities/latest-approved-game-version-for-each-regulation';
import { GamesProviderService } from './../../services/games-provider.service';
import { Observable } from 'rxjs/Observable';
import { Component, OnInit } from '@angular/core';
import * as _ from 'underscore';
import {AppNavigationProvider} from '../../app-navigation-provider';
import {ActivatedRoute} from "@angular/router";

@Component({
    moduleId: module.id,
    selector: 'html5-games-latest-approved',
    templateUrl: 'games-latest-approved.component.html'
})
export class Html5GamesLatestApprovedComponent implements OnInit {

    games: Observable<GameRecordReportItem[]>;

    constructor(private gamesProvider: GamesProviderService, private navigator: AppNavigationProvider, private route: ActivatedRoute) { }

    ngOnInit() {
      this.games = this.gamesProvider.getLatestApprovedGameVersionForEachRegulation()
                           .map(items => {
                             const gameTechnology = this.navigator.html5GamesLatestApprovedFromRouteParams(this.route.snapshot.params).gameTechnology;
                             const filteredGames = items.filter(item => GameTechnology[item.gameInfrastructure.technology] === gameTechnology.toString());
                             const allRegulations = _.keys(_.groupBy(filteredGames, item => item.regulation))
                                                      .sort((r1, r2) => r1.localeCompare(r2));
                             const groupedRecords = _.groupBy(filteredGames, item => this.getRecordKey(item));
                             const keys = _.keys(groupedRecords);
                             return keys.map(k => new GameRecordReportItem(allRegulations, groupedRecords[k]))
                               .sort((g1, g2) => g1.gameName.localeCompare(g2.gameName));
                           });
    }

    private getRecordKey(record: LatestApprovedGameVersionForRegulation): string {
        return record.gameId + record.gameInfrastructure.toString();
    }
    navigateToGameDetails(game: GameRecordReportItem): void {
      game.navigate(this.navigator);
    }
}

class GameRecordReportItem {
  private _firstRecord: LatestApprovedGameVersionForRegulation;
  regulations: RegulationRecordReportItem[];

  constructor(allRegulations: string[], records: LatestApprovedGameVersionForRegulation[]){
        this._firstRecord  = records[0];
        const groupedByRegulation = _.groupBy(records, r => r.regulation);
        this.regulations = allRegulations.map(r => new RegulationRecordReportItem(r, groupedByRegulation[r]));
    }
    get gameName(): string{
        return this._firstRecord.gameName;
    }
    get mainGameType(): number{
        return this._firstRecord.mainGameType;
    }
    get gameInfra(): GameInfrastructure {
        return this._firstRecord.gameInfrastructure;
    }

    get latestReleasedVersion(): string{
        return this._firstRecord.lastVersion;
    }

    get latestQaApprovedVersion(): string{
        if(this.getLatestQaApprovedVersion())
            return this.getLatestQaApprovedVersion().toString();
        else
            return '';
    }
    private getLatestQaApprovedVersion(): VersionNumber{
        return this._firstRecord.latestQAApprovedVersion;
    }

    get isMJC(): boolean{
        if(!this.getLatestQaApprovedVersion()) {
          return false;
        }

        for (let regulation of this.regulations){
            for (let item of regulation.records){
                const qaVersion = item.qaVersionInfo && item.qaVersionInfo.version
                if (qaVersion) {
                  if (!this.getLatestQaApprovedVersion().equals(qaVersion)) {
                    return false;
                  }
                }

            }
        }

        return true;
    }
    navigate(navigator: AppNavigationProvider) {
      navigator.gameDetailsNavigator(this._firstRecord.gameId)
        .gameVersionsNavigator(this.gameInfra)
        .navigate();
    }
}

class RegulationRecordReportItem {
    constructor(public regulationName: string, public records: LatestApprovedGameVersionForRegulation[]) {
        if (!records)
          this.records = [];
    }

    private getFirstRecord(): LatestApprovedGameVersionForRegulation {
      if(this.records.length === 0) {
        return null;
      }
      return this.records[0];
    }


    get qaApprovedVersion(): string{
        return this.extractVersionNumber(record => record.qaVersionInfo);
    }

    get pmApprovedVersion(): string{
        return this.extractVersionNumber(record => record.pmVersionInfo);
    }

    get productionVersion(): string{
        return this.extractVersionNumber(record => record.productionVersionInfo);
    }

    get isMJC(): boolean{

      if(!this.getFirstRecord()){
        return false;
      }
      if (!this.getFirstRecord().latestQAApprovedVersion) {
        return false;
      }

      return this.getFirstRecord().latestQAApprovedVersion.toString() === this.qaApprovedVersion;
    }

    private extractVersionNumber(getVersionInfo: (record) => LatestVersionInfo): string {
        if (!this.getFirstRecord()){
          return null;
        }

        const latestVersionInfo = getVersionInfo(this.getFirstRecord());

        if (latestVersionInfo && latestVersionInfo.version) {
            return latestVersionInfo.version.toString();
        }

        return null;
    }
}
