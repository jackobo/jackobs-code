import { GameDetailsChildNavigator } from './../game-details/game-details-child.navigator';
import { GameVersionsComponent } from './../game-versions/game-versions.component';
import { GameChangeHistoryComponent } from './game-change-history.component';

import { GameTechnology, PlatformType, GameInfrastructure } from 'app/entities/enums';
import { Route, Router, Params } from '@angular/router';

export class GameChangeHistoryNavigator implements GameDetailsChildNavigator{
  
    constructor(private parentRoutes : string[],  
                public routeParams: any, 
                private router: Router){

       if(routeParams instanceof GameInfrastructure){
            this.gameInfra = routeParams;
        }
        else{
            this.gameInfra = GameInfrastructure.fromStrings(routeParams['technology'], 
                                                            routeParams['platform']);
        }
    }

    gameInfra: GameInfrastructure;    


    navigate(): void{
        let routes = this.parentRoutes.slice(0, this.parentRoutes.length);
        routes.push('history');
        routes.push(GameTechnology[this.gameInfra.technology]);
        routes.push(PlatformType[this.gameInfra.platform]);
        this.router.navigate(routes);
    }
    

}