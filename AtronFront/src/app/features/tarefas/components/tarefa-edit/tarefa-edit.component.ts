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
      dataInicial: this.formatarDataParaEnvio(formValues.dataInicial),
      dataFinal: this.formatarDataParaEnvio(formValues.dataFinal),
      estadoDaTarefa: { id: formValues.estadoId, descricao: '' },
      usuarioCodigo: formValues.usuarioCodigo,
    };

    const operacao = this.id
      ? this.service.atualizar(this.id, tarefaPayload)
      : this.service.gravar(tarefaPayload);

    operacao.subscribe(() => this.router.navigate(['atron/tarefas']));
  }

  private formatDate(dateString?: any): string | null {
    if (!dateString) return null;

    const valor = dateString.toString();
    if (/^\d{2}\/\d{2}\/\d{4}$/.test(valor)) return valor;
    if (/^\d{4}-\d{2}-\d{2}/.test(valor)) {
      const [ano, mes, dia] = valor.substring(0, 10).split('-');
      return `${dia}/${mes}/${ano}`;
    }

    return valor;
  }

  private formatarDataParaEnvio(data?: Date | string | null): string | null {
    if (!data) return null;

    if (data instanceof Date) {
      return data.toISOString().substring(0, 10);
    }

    const valor = data.toString().trim();
    if (/^\d{4}-\d{2}-\d{2}$/.test(valor)) return valor;

    const partes = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(valor);
    if (!partes) return valor;

    const [, dia, mes, ano] = partes;
    return `${ano}-${mes}-${dia}`;
  }
}
