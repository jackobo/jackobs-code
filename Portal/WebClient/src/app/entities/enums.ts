export enum GamingComponentCategory{
    Wrapper = 0,
    Chill = 1,
    Game = 2
}

export enum GameTechnology {
    Flash = 0,
    Html5 = 1
}


export enum PlatformType {
    PC = 1,
    Mobile = 2,
    PcAndMobile = 3
}



export class GameInfrastructure {
  constructor(public technology: GameTechnology, public platform: PlatformType) {
  }

  public toString(): string {
    return GameTechnology[this.technology]
      + ' - '
      + (this.platform == PlatformType.PcAndMobile ? 'PC & Mobile' : PlatformType[this.platform]);
  }

  static fromJson(data: any): GameInfrastructure {
    return new GameInfrastructure(data.GameTechnology, data.PlatformType);
  }

  equalsTo(theOther: GameInfrastructure) {
    if (!theOther) {
      return false;
    }

    return this.technology === theOther.technology
      && this.platform === theOther.platform;
  }

  public static fromStrings(technology: string, platform: string) {
    return new GameInfrastructure(GameTechnology[technology], PlatformType[platform]);
  }
}
