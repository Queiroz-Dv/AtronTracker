import { Component } from '@angular/core';
import { VisualizacaoService } from '../../services/visualizacao-service';
import { Router } from '@angular/router';
import { SharedModule } from '../../../shared/modules/shared.module';

@Component({
  imports: [SharedModule,],
  standalone: true,
  selector: 'c-botao-voltar',
  templateUrl: './botao-voltar.component.html',
})
export class BotaoVoltarComponent {

  constructor(private router: Router, private visualizacaoService: VisualizacaoService) { }

  onVoltar() {
    const modo = this.visualizacaoService.getViewMode();
    return modo === 'menu' ? this.router.navigate(['/atron/home']) :
      this.router.navigate(['/atron/dashboard']);
  }
}
