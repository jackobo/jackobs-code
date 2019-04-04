import { GamingComponentCategory, GameInfrastructure } from "app/entities/enums";

export class Game {
    constructor(public Id:string,
    public Name: string,
    public MainGameType: number,
    public IsExternal: boolean,
    public Category: GamingComponentCategory,
    public SupportedInfrastructures: GameInfrastructure[],
    public GameTypes: GameType[])
    {

    }


    public isGameMatch(term: string) : boolean{
        let regEx = new RegExp(term, "i");
        let match = this.Name.match(regEx);
        if(match &&  match.length > 0){
            return true;
        }

        for(let gt of this.GameTypes){
            match = gt.Id.toString().match(regEx);
            if(match &&  match.length > 0){
                return true;
            }   
        }

        return false;
    }

    static fromJson(data: any): Game{
         let supportedInfrastructures = data.SupportedInfrastructures.map(i => GameInfrastructure.fromJson(i));

        let gameTypes = data.GameTypes
                        .map(gt => GameType.fromJson(gt))
                        .sort((gt1, gt2) => gt1.OperatorName.localeCompare(gt2.OperatorName));

        return new Game(data.Id, data.Name, data.MainGameType, data.IsExternal, data.Category, supportedInfrastructures,gameTypes);
    }
}

export class GameType{
    constructor(public Id: number, public Name: string, public OperatorId: number){
        if(this.OperatorId == 0){
            this.OperatorName = "888";
        }
        else{
            this.OperatorName = "Bingo";
        }
    }

    OperatorName: string;

    static fromJson(json: any): GameType{
        return new GameType(json.Id, json.Name, json.OperatorId);
    }
}
