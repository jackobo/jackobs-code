import { GameVersionChangeHistory } from './game-version-change-history';
import { GameVersionRegulation } from './game-version-regulation';
import { Optional } from './../utils/optional';
import { DateUtils } from './../utils/date-utils';
import { Subject } from 'rxjs/Subject';
import { GamingComponentCategory } from 'app/entities/enums';
import { VersionNumber } from './version-number';
import { GameInfrastructure } from './enums';


export class GameVersion {
    constructor(public id: string,
                version: string,
                public gameInfra: GameInfrastructure,
                public componentCategory: GamingComponentCategory,
                public triggeredBy: string,
                public createdDate: Date,
                public cretedBy: string,
                public regulations: GameVersionRegulation[],
                public history: GameVersionChangeHistory[]) {

        this.version = VersionNumber.parse(version);
        let x = regulations;
    }

    
    versionAsNumber: number;
    version: VersionNumber;

    static fromJson(data: any): GameVersion {

        return new GameVersion(data.VersionId,
                                data.Version,
                                GameInfrastructure.fromJson(data.GameInfrastructure),
                                data.ComponentCategory,
                                data.TriggeredBy,
                                DateUtils.parse(data.CreatedDate),
                                data.CreatedBy,
                                data.Regulations.map(regulation => GameVersionRegulation.fromJson(regulation)),
                                data.PropertiesChangeHistory.map(history => GameVersionChangeHistory.fromJson(history)));
    }

    getInProgressRegulations() : GameVersionRegulation[]{
        return this.regulations.filter(r => r.inProgress());
    }

    getReadyForProductionRegulations() : GameVersionRegulation[]{
        return this.regulations.filter(r => r.readyForProduction());
    }

    getInProductionRegulations() : GameVersionRegulation[]{
        return this.regulations.filter(r => r.inProduction());
    }


}
