import { NgModule } from '@angular/core';
import { AsyncPipe, CommonModule } from '@angular/common';
import { MatTable, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { LoadingBarComponent } from './core/layout/loading-bar/loading-bar.component';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatMenuModule } from '@angular/material/menu';
import { ControlErrorComponent } from './shared/components/control-error/control-error.component';

@NgModule({
  declarations: [],
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    LoadingBarComponent,
    CommonModule,
    AsyncPipe,
    MatGridListModule,
    MatMenuModule,
    MatTable,
    MatPaginator,
    ControlErrorComponent
  ],
  exports: [
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    LoadingBarComponent,
    CommonModule,
    AsyncPipe,
    MatGridListModule,
    MatMenuModule,
    MatTable,
    MatPaginator,
    MatSort,
    ControlErrorComponent
  ]
})
export class MaterialContainerModule { }