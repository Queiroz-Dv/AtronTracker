import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AcessoService, ConfirmarEmailRequest } from '../login/services/acesso.service';
import { SharedModule } from '../../../shared/modules/shared.module';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ControlErrorComponent } from '../../../shared/components/control-error/control-error.component';
import { Nivel, NotificacaoService } from '../../../core/services/notification.service';

@Component({
  selector: 'c-confirmar-email',
  imports: [CommonModule, SharedModule, RouterModule, ReactiveFormsModule, ControlErrorComponent],
  templateUrl: './confirmar-email.component.html',
  styleUrls: ['../login/login.component.css', './confirmar-email.component.css'],
  standalone: true,
})
export class ConfirmarEmailComponent implements OnInit { 
  sucesso = false;
  mensagem = '';
  confirmado = false;
  usuarioCodigo = '';
  identificadorControl = new FormControl('', [Validators.required]);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private acessoService: AcessoService,
    private notificacaoService: NotificacaoService
  ) { }

  ngOnInit(): void {
    const usuarioCodigo = this.route.snapshot.queryParamMap.get('usuarioCodigo');

    if (!usuarioCodigo) {
      this.sucesso = false;
      this.mensagem = 'Link de confirmação inválido ou incompleto.';
      this.confirmado = true;
      return;
    }

    this.usuarioCodigo = usuarioCodigo;
  }

  confirmarEmail(): void {
    if (this.identificadorControl.invalid) {
      this.identificadorControl.markAsTouched();
      return;
    }

    const confirmarEmailRequest = new ConfirmarEmailRequest();
    confirmarEmailRequest.usuarioCodigo = this.usuarioCodigo;
    confirmarEmailRequest.identificador = this.identificadorControl.value as string;

    this.acessoService.confirmarEmail(confirmarEmailRequest).subscribe({
      next: (resposta: any) => {
        this.sucesso = true;
        this.mensagem = this.extrairMensagem(resposta) || 'E-mail confirmado com sucesso!';
        this.confirmado = true;
        this.notificacaoService.exibirMensagem(this.mensagem, Nivel.Sucesso);
      },
      error: (error: any) => {       
        this.sucesso = false;
        this.mensagem = this.extrairMensagem(error?.error) || 'Falha ao confirmar e-mail. Tente novamente.';
        this.confirmado = false;
        this.notificacaoService.exibirMensagem(this.mensagem, Nivel.Error);
      }
    });
  }

  irParaLogin(): void {
    this.router.navigate(['/login']);
  }

  private extrairMensagem(retorno: any): string {
    if (!retorno) {
      return '';
    }

    if (typeof retorno === 'string') {
      return retorno;
    }

    if (Array.isArray(retorno)) {
      return retorno
        .map(item => typeof item === 'string' ? item : item?.descricao)
        .filter(Boolean)
        .join(' ');
    }

    return retorno.descricao || retorno.message || '';
  }
}
