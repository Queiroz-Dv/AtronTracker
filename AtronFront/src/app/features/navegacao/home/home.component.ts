import { Component, inject, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Router, RouterModule } from '@angular/router';
import { VisualizacaoService } from '../../../core/services/visualizacao-service';
import { Modulo } from '../modulos.model';
import { SharedModule } from '../../../shared/modules/shared.module';
import { AcessoService } from '../../../core/services/acesso.service';


@Component({
  selector: 'atron-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [
    SharedModule,
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    RouterModule
  ]
})
export class HomeComponent implements OnInit {
  private breakpointObserver = inject(BreakpointObserver);
  modulosView: Modulo[] = [];

  constructor(
    private router: Router,
    private visualizacaoService: VisualizacaoService,
    private authService: AcessoService
  ) { }

  navigate(route: string) {
    this.router.navigate([route]);
  }

  ngOnInit(): void {
    this.setModulosForView();
  }

  setModulosForView() {
    this.modulos.subscribe(modulo => {
      this.modulosView = modulo;
    });
  }

  trocarVisualizacao() {
    this.visualizacaoService.setViewMode('dashboard');
    this.router.navigate(['/atron/dashboard']);
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }

  modulos: Observable<Modulo[]> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(map(() => Modulo.getModulos()));
}
