import { Router } from '@angular/router';
import {GameTechnology} from "../../entities/enums";

export class Html5GamesLatestApprovedNavigator {
    constructor(private router: Router, public gameTechnology: GameTechnology) {

    }

    navigate() {
        this.router.navigate(['/games-latest-approved', GameTechnology[this.gameTechnology]]);
    }

}
