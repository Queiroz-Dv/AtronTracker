import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AcessoService } from '../../../../core/services/acesso.service';
import { NotificacaoService, Nivel } from '../../../../core/services/notification.service';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { RegistrarRequest, TipoWorkspace } from '../../../../shared/models/request/registrar-request.model';
import { ConviteWorkspaceResponse, RegistrarResponse } from '../../../../shared/models/response/registrar-response.model';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { converterDataParaFormulario, formatarDataParaEnvio } from '../../../../shared/utils/data-form.utils';

@Component({
  selector: 'c-registrar',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent, RouterModule],
  templateUrl: './registrar.component.html',
  styleUrls: ['./registrar.component.css'],
  standalone: true,
})
export class RegistrarComponent implements OnInit {
  form!: FormGroup;
  id?: number;
  readonly tipoWorkspace = TipoWorkspace;
  conviteIdentificador?: string;
  conviteWorkspace?: ConviteWorkspaceResponse;
  carregandoConvite = false;
  conviteInvalido = false;

  constructor(
    private fb: FormBuilder,
    private acessoService: AcessoService,
    private notificacaoService: NotificacaoService,
    private route: ActivatedRoute,
    public router: Router
  ) { }

  ngOnInit(): void {
    const empresa = this.fb.group({
      codigo: ['', Validators.required],
      nomeFantasia: ['', Validators.required],
      endereco: ['', Validators.required],
      numero: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
    });
    empresa.disable();

    this.form = this.fb.group({
      codigo: ['', Validators.required],
      workspaceNome: ['', Validators.required],
      workspaceTipo: [TipoWorkspace.Pessoal, Validators.required],
      empresa,
      nome: ['', Validators.required],
      sobrenome: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      dataNascimento: ['', Validators.required],
      senha: ['', Validators.required],
      confirmaSenha: ['', Validators.required]
    });

    this.form.get('workspaceTipo')?.valueChanges.subscribe(tipo => {
      // 1. Lógica da empresa (já existente)
      if (tipo === TipoWorkspace.Empresa) {
        empresa.enable();
      } else {
        empresa.reset();
        empresa.disable();
      }

      // 2. Lógica para os campos pessoais (nome, sobrenome, dataNascimento)
      const nome = this.form.get('nome');
      const sobrenome = this.form.get('sobrenome');
      const dataNasc = this.form.get('dataNascimento');

      if (tipo === TipoWorkspace.Empresa) {        
        nome?.clearValidators();
        sobrenome?.clearValidators();
        dataNasc?.clearValidators();        
        nome?.reset();
        sobrenome?.reset();
        dataNasc?.reset();
      } else {        
        nome?.setValidators(Validators.required);
        sobrenome?.setValidators(Validators.required);
        dataNasc?.setValidators(Validators.required);        
        nome?.reset();
        sobrenome?.reset();
        dataNasc?.reset();
      }
      
      nome?.updateValueAndValidity();
      sobrenome?.updateValueAndValidity();
      dataNasc?.updateValueAndValidity();
    });

    this.conviteIdentificador = this.route.snapshot.queryParamMap.get('convite') ?? undefined;
    if (this.conviteIdentificador) {
      this.form.get('workspaceNome')?.disable();
      this.form.get('workspaceTipo')?.disable();
      empresa.disable();
      this.carregarConvite(this.conviteIdentificador);
    }
  }

  registrarNovoUsuario(): void {
    if (this.form.invalid || this.carregandoConvite || this.conviteInvalido)
      return;

    const formulario = this.form.getRawValue();
    const tipoWorkspace = formulario.workspaceTipo as TipoWorkspace;
    const workspace = this.conviteIdentificador
      ? undefined
      : {
        nome: formulario.workspaceNome,
        tipo: tipoWorkspace,
        empresa: tipoWorkspace === TipoWorkspace.Empresa
          ? formulario.empresa
          : undefined,
      };

    const dataNascimentoFormatada = formatarDataParaEnvio(formulario.dataNascimento) ?? '';
    const dadosDoUsuario = new RegistrarRequest(
      formulario.codigo,
      formulario.nome,
      formulario.sobrenome,
      formulario.email,
      formulario.senha,
      formulario.confirmaSenha,
      workspace,
      dataNascimentoFormatada,
      this.conviteIdentificador);

    this.acessoService.registrar(dadosDoUsuario).subscribe({
      next: (resposta: RegistrarResponse) => {
        const mensagens = this.notificacaoService.normalizarMensagens(resposta.mensagens, Nivel.Sucesso);
        const mensagemSucesso = mensagens.length
          ? mensagens.map(mensagem => mensagem.descricao).join('\n')
          : 'Usuário registrado com sucesso! Verifique seu e-mail para confirmar.';

        this.notificacaoService.exibirMensagem(mensagemSucesso, Nivel.Sucesso, 5000);

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (error: any) => {
        console.error('Erro ao registrar usuário:', error);
        const mensagens = this.notificacaoService.normalizarMensagens(error);

        if (mensagens.length) {
          this.notificacaoService.exibirMensagens(mensagens);
          return;
        }

        this.notificacaoService.exibirMensagem('Erro ao registrar usuário. Tente novamente.', Nivel.Error);
      }
    });
  }

  private carregarConvite(identificador: string): void {
    this.carregandoConvite = true;
    this.acessoService.obterConviteWorkspace(identificador).subscribe({
      next: convite => {
        this.conviteWorkspace = convite;
        this.carregandoConvite = false;
      },
      error: error => {
        this.conviteInvalido = true;
        this.carregandoConvite = false;
        const mensagens = this.notificacaoService.normalizarMensagens(error);

        if (mensagens.length) {
          this.notificacaoService.exibirMensagens(mensagens);
          return;
        }

        this.notificacaoService.exibirMensagem(
          'Não foi possível validar o convite informado.',
          Nivel.Error);
      }
    });
  }
}
