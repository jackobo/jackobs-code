import { Html5GamesLatestApprovedComponent } from './reports/games-latest-approved/games-latest-approved.component';
import { GameChangeHistoryComponent } from './components/game-change-history/game-change-history.component';
import { GameVersionsComponent } from './components/game-versions/game-versions.component';
import { GameDetailsComponent } from './components/game-details/game-details.component';
import { HomeComponent } from './components/home/home.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes, Route } from '@angular/router';

const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent},
    { path: 'games-latest-approved/:gameTechnology', component: Html5GamesLatestApprovedComponent},
    {path: 'gamedetails/:id',
            component: GameDetailsComponent,
            children: [
                    {
                    path: 'versions/:technology/:platform',
                    component: GameVersionsComponent
                    },
                     {
                    path: 'history/:technology/:platform',
                    component: GameChangeHistoryComponent
                    }
                ]
    },
];



@NgModule({
    imports: [RouterModule.forRoot(routes, {useHash: true})],
    exports: [RouterModule]
})
export class AppRoutingModule{

}
