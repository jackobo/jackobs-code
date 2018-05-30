import { DateUtils } from './../utils/date-utils';
export class GameVersionChangeHistory{
    constructor(public changeDate: Date, 
                public changeType: number, 
                public changedBy: string, 
                public oldValue: string,
                public newValue: string,
                public propertyKey: string,
                public regulation: string)
    {
        
    }

     static fromJson(data: any): GameVersionChangeHistory {

        return new GameVersionChangeHistory(DateUtils.parse(data.ChangeDate),
                                            data.ChangeType,
                                            data.ChangedBy,
                                            data.OldValue,
                                            data.NewValue,
                                            data.PropertyKey,
                                            data.Regulation);
    }
}