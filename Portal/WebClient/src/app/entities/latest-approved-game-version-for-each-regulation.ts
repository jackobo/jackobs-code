import { GameVersionRegulation } from './game-version-regulation';
import { LatestVersionInfo } from './latest-version-info';
import { GameInfrastructure } from './enums';
import { Optional } from './../utils/optional';
import { VersionNumber } from './version-number';

export class LatestApprovedGameVersionForRegulation {

    static fromJson(json: any): LatestApprovedGameVersionForRegulation{
      return new LatestApprovedGameVersionForRegulation(json.GameId,
        json.GameName,
        json.LastVersion,
        json.MainGameType,
        json.Regulation,
        json.IsExternal,
        GameInfrastructure.fromJson(json.GameInfrastructure),
        LatestVersionInfo.fromJson(json.QAVersionInfo),
        LatestVersionInfo.fromJson(json.PMVersionInfo),
        LatestVersionInfo.fromJson(json.ProductionVersionInfo),
        VersionNumber.tryParse(json.LatestQAApprovedVersion)
      );
    }
    constructor(public gameId: string,
                public gameName: string,
                public lastVersion: string,
                public mainGameType: number,
                public regulation: string,
                public isExternal: boolean,
                public gameInfrastructure: GameInfrastructure,
                public qaVersionInfo: LatestVersionInfo,
                public pmVersionInfo: LatestVersionInfo,
                public productionVersionInfo: LatestVersionInfo ,
                public latestQAApprovedVersion?: VersionNumber) {

    }
}
