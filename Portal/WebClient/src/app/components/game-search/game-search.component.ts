import { Router } from '@angular/router';
import { GamesProviderService } from './../../services/games-provider.service';
import { Game } from './../../entities/game';
import {Component, OnInit} from '@angular/core';





import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';


// Observable class extensions
import 'rxjs/add/observable/of';
import 'rxjs/add/observable/from';

// Observable operators
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/filter';


@Component({
    selector: 'game-search',
    templateUrl: 'game-search.component.html'
})
export class GameSearchComponent implements OnInit {

    constructor(private gamesProvider : GamesProviderService, private router: Router){}
 
    
    allGames: Observable<SearchableGame[]>;
     
    
    ngOnInit(): void {
         
        this.allGames = this.gamesProvider.getAllGames()
                        .map(games =>  {
                            if(this.searchText)
                                return games.filter((g: Game) => g.isGameMatch(this.searchText))
                                            .map((g: Game) => new SearchableGame(g));
                            else
                                return [];
                        });
    }

    public searchText: string;

  
    selectGame(game: SearchableGame){
        this.router.navigate(['/gamedetails', game.Id])
        this.searchText = '';
    }
 
}

class SearchableGame{
    constructor(game: Game){
        this.Id = game.Id;
        this.fullName = game.Name + ' - ' + game.MainGameType;
    }

     fullName:string;
     Id: string;
}