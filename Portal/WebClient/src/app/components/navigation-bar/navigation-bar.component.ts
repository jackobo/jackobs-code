import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'navigation-bar',
    templateUrl: 'navigation-bar.component.html'
})
export class NavigationBarComponent implements OnInit {

    constructor(private router: Router) { }

    ngOnInit() { 

    }

    goHome(){
        this.router.navigate(['/home']);
    }

}