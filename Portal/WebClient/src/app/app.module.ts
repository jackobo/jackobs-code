

import { GameInfraIconsComponent } from './components/game-infra-icons/game-infra-icons.component';
import { GameChangeHistoryComponent } from './components/game-change-history/game-change-history.component';
import { AppRoutingModule } from './app-routing.module';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule }   from '@angular/router';
import { NgxBootstrapModule } from './ngx-bootstrap.module';


import { GamesProviderService } from './services/games-provider.service';
import { GameDetailsComponent } from './components/game-details/game-details.component';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { NavigationBarComponent } from "./components/navigation-bar/navigation-bar.component";
import { GameSearchComponent } from "./components/game-search/game-search.component";
import { GameTypesListComponent } from "./components/game-types/game-types-list.component";
import { ApprovalInfoComponent } from './components/approval-info/approval-info.component';
import { GameVersionRegulationsComponent } from './components/game-version-regulations/game-version-regulations.component';
import { GameVersionsComponent } from './components/game-versions/game-versions.component';
import { UploadInfoComponent } from "./components/upload-info/upload-info.component";
import { AppNavigationProvider } from './app-navigation-provider';
import { Html5GamesLatestApprovedComponent } from "./reports/games-latest-approved/games-latest-approved.component";



@NgModule({
  declarations: [
    AppComponent,
    GameSearchComponent,
    HomeComponent,
    NavigationBarComponent,
    GameDetailsComponent,
    GameVersionsComponent,
    GameTypesListComponent,
    GameVersionRegulationsComponent,
    ApprovalInfoComponent,
    UploadInfoComponent,
    GameChangeHistoryComponent,
    GameInfraIconsComponent,
    Html5GamesLatestApprovedComponent
    ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    AppRoutingModule,
    NgxBootstrapModule
  ],
  providers: [GamesProviderService, AppNavigationProvider],
  bootstrap: [AppComponent]
})
export class AppModule { }


