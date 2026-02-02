import { Component, Input, } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../modules/shared.module';

@Component({
  selector: 'c-usuario-informacoes',
  imports: [ReactiveFormsModule, SharedModule],
  templateUrl: './usuario-informacoes.component.html',
  styleUrls: ['./usuario-informacoes.component.css'],
})

export class UsuarioInformacoesComponent {
  @Input() nome!: string;
  @Input() cargoDescricao!: string;
  @Input() departamentoDescricao!: string;
  @Input() totalDeTarefas!: number;
}
