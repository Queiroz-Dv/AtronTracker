import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AcessoService } from '../../../core/services/acesso.service';
import { ContextoEmpresa, EmpresaContextoService } from '../../../core/services/empresa-contexto.service';

@Component({
  standalone: true,
  selector: 'c-acesso-empresa',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './acesso-empresa.component.html',
  styleUrl: './acesso-empresa.component.css'
})
export class AcessoEmpresaComponent implements OnInit {
  contexto: ContextoEmpresa | null = null;
  carregando = false;
  saindo = false;
  erro = '';

  constructor(
    private empresa: EmpresaContextoService,
    private acesso: AcessoService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.verificarAcesso();
  }

  verificarAcesso(): void {
    this.carregando = true;
    this.erro = '';
    this.contexto = null;
    this.empresa.obter().pipe(finalize(() => this.carregando = false)).subscribe({
      next: contexto => {
        this.contexto = contexto;
        if (contexto?.acessoPermitido === true) {
          void this.router.navigate(['/atron/dashboard']);
        }
      },
      error: () => {
        this.erro = 'Não foi possível consultar sua associação. Tente novamente.';
      }
    });
  }

  sair(): void {
    this.saindo = true;
    this.acesso.logout().pipe(finalize(() => this.saindo = false)).subscribe(() => {
      void this.router.navigate(['/login']);
    });
  }
}
