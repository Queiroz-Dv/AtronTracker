import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { MaterialContainerModule } from './material-container.module';
import { AcessoService } from './features/acesso/login/services/acesso.service';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, MaterialContainerModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'atron-manager-webview';
  mostrarNavbarInterna = false;
  constructor(
    private router: Router,
    private acessoService: AcessoService
  ) { }

  ngOnInit() {
    this.atualizarNavbarInterna(this.router.url);

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe(event => this.atualizarNavbarInterna(event.urlAfterRedirects));
  }

  abrirTracker() {
    this.router.navigate(['/atron/dashboard'], { queryParams: { produto: 'tracker' } });
  }

  logout() {
    this.acessoService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }

  private atualizarNavbarInterna(url: string) {
    const rotaSemQuery = url.split('?')[0];
    this.mostrarNavbarInterna = rotaSemQuery.startsWith('/atron/')
      && rotaSemQuery !== '/atron/dashboard'
      && rotaSemQuery !== '/atron/home';
  }
}
