import { NgModule } from '@angular/core';
//import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
//import { PopoverModule } from 'ngx-bootstrap/popover';
import { PopoverModule,TypeaheadModule, CollapseModule } from 'ngx-bootstrap';



@NgModule({
    imports: [ 
             TypeaheadModule.forRoot(),
             PopoverModule.forRoot(),
             CollapseModule.forRoot()
            ],
    exports: [
            TypeaheadModule, 
            PopoverModule,
            CollapseModule
            ]
})
export class NgxBootstrapModule { }