import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TarefaService } from '../../services/tarefa.service';
import { TarefaFormComponent } from "../tarefa-form/tarefa-form.component";
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { TarefaRequest } from '../../models/request/tarefa-request.model';
import { Nivel, NotificacaoService } from '../../../../../core/services/notification.service';
import { converterDataParaFormulario, formatarDataParaEnvio } from '../../../../../shared/utils/data-form.utils';
import { TarefaHistoricoComponent } from '../tarefa-historico/tarefa-historico.component';

@Component({
  selector: 'c-tarefa-edit',
  imports: [ReactiveFormsModule, TarefaFormComponent, TarefaHistoricoComponent, SharedModule],
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
    private notificacaoService: NotificacaoService,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      id: [null],
      titulo: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      conteudo: ['', [Validators.required, Validators.maxLength(2500)]],
      dataInicial: [null],
      dataFinal: [null],
      destinoInicial: [1, Validators.required],
      exigeAprovacaoParaObter: [false],
      estadoId: [null, Validators.required],
      usuarioCodigo: [null],
      departamentoCodigo: [null],
      cargoCodigo: [null],
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
          dataInicial: converterDataParaFormulario(trf.dataInicial),
          dataFinal: converterDataParaFormulario(trf.dataFinal),
          usuarioNome: trf.usuario?.nome ?? '',
          cargoDescricao: trf.usuario?.cargo?.descricao ?? '',
          departamentoDescricao: trf.usuario?.departamento?.descricao ?? '',
          estadoId: trf.estadoDaTarefa.id,
          destinoInicial: trf.destinoInicial || 1,
          exigeAprovacaoParaObter: trf.exigeAprovacaoParaObter,
          departamentoCodigo: trf.departamentoCodigo,
          cargoCodigo: trf.cargoCodigo
        }
        this.form.patchValue(tarefa);
      }
      );
    }
  }

  salvar() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notificacaoService.exibirMensagem('Formulário inválido. Verifique os campos.', Nivel.Error);
      return;
    }

    const formValues = this.form.getRawValue();

    const tarefaPayload: TarefaRequest = {
      id: formValues.id || 0,
      identificador: formValues.identificador,
      destinoInicial: formValues.destinoInicial,
      exigeAprovacaoParaObter: formValues.exigeAprovacaoParaObter,
      titulo: formValues.titulo,
      conteudo: formValues.conteudo,
      dataInicial: formatarDataParaEnvio(formValues.dataInicial),
      dataFinal: formatarDataParaEnvio(formValues.dataFinal),
      estadoDaTarefa: { id: formValues.estadoId, descricao: '' },
      usuarioCodigo: formValues.usuarioCodigo,
      departamentoCodigo: formValues.departamentoCodigo,
      cargoCodigo: formValues.cargoCodigo,
    };

    const operacao = this.id
      ? this.service.atualizar(this.id, tarefaPayload)
      : this.service.gravar(tarefaPayload);

    operacao.subscribe({
      next: resposta => {
        this.notificacaoService.exibirMensagens(resposta);
        this.router.navigate(['atron/tarefas']);
      },
      error: erro => {
        if (erro?.mensagensApi) {
          this.notificacaoService.exibirMensagens(erro.mensagensApi);
          return;
        }

        this.notificacaoService.exibirMensagem('Não foi possível salvar a tarefa.', Nivel.Error);
      }
    });
  }

}
