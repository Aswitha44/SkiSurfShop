import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RentalRoutingModule } from './rental-routing.module';
import { RentalComponent } from './rental/rental.component';


@NgModule({
  declarations: [
    RentalComponent
  ],
  imports: [
    CommonModule,
    RentalRoutingModule
  ]
})
export class RentalModule { }
