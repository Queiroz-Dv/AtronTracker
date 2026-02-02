import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatPaginatorModule,
    MatTableModule, 
    MatIconModule, 
    MatCardModule, 
    MatButtonModule, 
    ReactiveFormsModule
  ],
  exports: [
    CommonModule,
    MatPaginatorModule,
    MatTableModule, 
    MatIconModule, 
    MatCardModule, 
    MatButtonModule, 
    ReactiveFormsModule
  ]
})
export class FormModule { }