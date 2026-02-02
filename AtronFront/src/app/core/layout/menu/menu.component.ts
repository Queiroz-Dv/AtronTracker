import { Component } from '@angular/core';
import { MenuItem } from '../models/menuItem';
import { MatListModule } from '@angular/material/list';

@Component({
  standalone: true,
  selector: 'atron-menu',
  imports: [MatListModule],
  template: `
@for (item of menuItems; track item.path) {
    <a mat-list-item [href]="item.path">{{ item.label }}</a>
  }`,
  styles: ``
})
export class MenuComponent {
  menuItems: Array<MenuItem> = [
    { path: '/', label: 'Home' },
    { path: '/departamentos', label: 'Departamentos' },
    { path: '/cargos', label: 'Cargos' },
  ]
}