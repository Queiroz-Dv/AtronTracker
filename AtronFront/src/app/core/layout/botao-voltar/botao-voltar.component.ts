import { Component, Input } from '@angular/core';
import { VisualizacaoService } from '../../services/visualizacao-service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedModule } from '../../../shared/modules/shared.module';

@Component({
  imports: [SharedModule,],
  standalone: true,
  selector: 'c-botao-voltar',
  templateUrl: './botao-voltar.component.html',
})
export class BotaoVoltarComponent {
  @Input() destino?: string | any[];

  constructor(
    private router: Router,
    private visualizacaoService: VisualizacaoService,
    private activatedRoute: ActivatedRoute
  ) { }

  onVoltar() {
    if (Array.isArray(this.destino)) {
      return this.router.navigate(this.destino);
    }

    if (this.destino) {
      return this.router.navigateByUrl(this.destino);
    }

    const modo = this.visualizacaoService.getViewMode();
    if (modo === 'menu') {
      return this.router.navigate(['/atron/home']);
    }

    const area = this.obterAreaDashboard();
    return area
      ? this.router.navigate(['/atron/dashboard'], { queryParams: { area } })
      : this.router.navigate(['/atron/dashboard']);
  }

  private obterAreaDashboard(): string | null {
    const snapshots = this.activatedRoute.snapshot.pathFromRoot;

    for (let index = snapshots.length - 1; index >= 0; index--) {
      const area = snapshots[index].data?.['area'];
      if (typeof area === 'string' && area.trim().length > 0) {
        return area.trim();
      }
    }

    return null;
  }
}
