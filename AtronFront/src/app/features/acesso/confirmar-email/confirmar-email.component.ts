import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AcessoService, ConfirmarEmailRequest } from '../login/services/acesso.service';
import { SharedModule } from '../../../shared/modules/shared.module';

@Component({
  selector: 'c-confirmar-email',
  imports: [CommonModule, SharedModule, RouterModule],
  templateUrl: './confirmar-email.component.html',
  styleUrls: ['./confirmar-email.component.css'],
  standalone: true,
})
export class ConfirmarEmailComponent implements OnInit { 
  sucesso = false;
  mensagem = '';
  confirmado = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private acessoService: AcessoService
  ) { }

  ngOnInit(): void {
    const usuarioCodigo = this.route.snapshot.queryParamMap.get('usuarioCodigo');
    const token = this.route.snapshot.queryParamMap.get('token');

    console.log(usuarioCodigo + " " + token);


    var confirmarEmailRequest =  new ConfirmarEmailRequest();
    confirmarEmailRequest.usuarioCodigo = usuarioCodigo;
    confirmarEmailRequest.token = token;

    this.acessoService.confirmarEmail(confirmarEmailRequest).subscribe({
      next: (resposta: string) => {        
        this.sucesso = true;
        this.mensagem = resposta || 'E-mail confirmado com sucesso!';
        this.confirmado = true;
      },
      error: (error: any) => {       
        this.sucesso = false;
        this.mensagem = error?.error || 'Falha ao confirmar e-mail. Tente novamente.';
        this.confirmado = true;
      }
    });
  }

  irParaLogin(): void {
    this.router.navigate(['/login']);
  }
}
