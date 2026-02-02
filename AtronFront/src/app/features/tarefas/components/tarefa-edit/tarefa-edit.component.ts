import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TarefaService } from '../../services/tarefa.service';
import { TarefaFormComponent } from "../tarefa-form/tarefa-form.component";
import { SharedModule } from '../../../../shared/modules/shared.module';
import { TarefaRequest } from '../../models/request/tarefa-request.model';

@Component({
  selector: 'c-tarefa-edit',
  imports: [ReactiveFormsModule, TarefaFormComponent, SharedModule],
  templateUrl: './tarefa-edit.component.html',
  styleUrl: '../../tarefa.component.css'
})

export class TarefaEditComponent implements OnInit {
  form!: FormGroup;
  id?: number;

  constructor(
    private fb: FormBuilder,
    private service: TarefaService,
    private route: ActivatedRoute,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      id: [null],
      titulo: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      conteudo: ['', [Validators.required, Validators.maxLength(2500)]],
      dataInicial: [null],
      dataFinal: [null],
      estadoId: [null, Validators.required],
      usuarioCodigo: [null, Validators.required],
      usuarioNome: [''],
      cargoDescricao: [''],
      departamentoDescricao: ['']
    });

    this.id = +(this.route.snapshot.paramMap.get('id') || 0);
    if (this.id) {
      this.service.obterTarefaPorIdService(this.id).subscribe(trf => {
        const tarefa = {
          ...trf,
          id: trf.id,
          titulo: trf.titulo,
          conteudo: trf.conteudo,
          dataInicial: this.formatDate(trf.dataInicial),
          dataFinal: this.formatDate(trf.dataFinal),
          usuarioNome: trf.usuario?.nome ?? '',
          cargoDescricao: trf.usuario?.cargo?.descricao ?? '',
          departamentoDescricao: trf.usuario?.departamento?.descricao ?? '',
          estadoId: trf.estadoDaTarefa.id
        }
        this.form.patchValue(tarefa);
      }
      );
    }
  }

  salvar() {
    const formValues = this.form.getRawValue();

    const tarefaPayload: TarefaRequest = {
      id: formValues.id || 0,
      titulo: formValues.titulo,
      conteudo: formValues.conteudo,
      dataInicial: formValues.dataInicial,
      dataFinal: formValues.dataFinal,
      estadoDaTarefa: { id: formValues.estadoId, descricao: '' },
      usuarioCodigo: formValues.usuarioCodigo,
    };

    const operacao = this.id
      ? this.service.atualizar(this.id, tarefaPayload)
      : this.service.gravar(tarefaPayload);

    operacao.subscribe(() => this.router.navigate(['atron/tarefas']));
  }

  // Transforma data ISO para yyyy-MM-dd
  private formatDate(dateString?: any): string | null {
    if (!dateString) return null;
    return dateString.split('T')[0];
  }
}
