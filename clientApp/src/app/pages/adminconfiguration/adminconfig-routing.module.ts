import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AdminConfigComponent } from './adminconfig.component';


const routes: Routes = [
    {
        path: '',
        component: AdminConfigComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AdminConfigRoutingModule { }
