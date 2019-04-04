import { Route, Router, Params } from "@angular/router";
import { GameVersionsComponent } from './game-versions.component';
import { GameTechnology, PlatformType } from 'app/entities/enums';
import { GameInfrastructure } from 'app/entities/enums';
import { GameDetailsChildNavigator } from "app/components/game-details/game-details-child.navigator";




export class GameVersionsNavigator implements GameDetailsChildNavigator {

    gameInfra: GameInfrastructure;

    constructor(private parentRoutes: string[], routeParams: any, private router: Router)
    {
        if(routeParams instanceof GameInfrastructure) {
            this.gameInfra = routeParams;
        }
        else{
            this.gameInfra = GameInfrastructure.fromStrings(routeParams['technology'],
                                                            routeParams['platform']);
        }
    }


    navigate(): void{
           let routes = this.parentRoutes.slice(0, this.parentRoutes.length);
           routes.push('versions');
           routes.push(GameTechnology[this.gameInfra.technology]);
           routes.push(PlatformType[this.gameInfra.platform]);
           this.router.navigate(routes);
    }
}
