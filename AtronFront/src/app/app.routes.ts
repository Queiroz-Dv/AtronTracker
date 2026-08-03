import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { ModuloGuard } from './core/guards/modulo.guard';
import { DashboardComponent } from './features/navegacao/dashboard/dashboard.component';
import { HomeComponent } from './features/navegacao/home/home.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./plataforma/tracker/acesso/login/login.component').then(m => m.LoginComponent) },
  { path: 'registrar', loadComponent: () => import('./plataforma/tracker/acesso/registrar/registrar.component').then(m => m.RegistrarComponent) },
  { path: 'confirmar-email', loadComponent: () => import('./plataforma/tracker/acesso/confirmar-email/confirmar-email.component').then(m => m.ConfirmarEmailComponent) },
  { path: 'reenviar-confirmacao', loadComponent: () => import('./plataforma/tracker/acesso/reenviar-confirmacao/reenviar-confirmacao.component').then(m => m.ReenviarConfirmacaoComponent) },
  { path: 'esqueci-senha', loadComponent: () => import('./plataforma/tracker/acesso/esqueci-senha/esqueci-senha.component').then(m => m.EsqueciSenhaComponent) },
  { path: 'trocar-senha', loadComponent: () => import('./plataforma/tracker/acesso/trocar-senha/trocar-senha.component').then(m => m.TrocarSenhaComponent) },
  // Aplica o guard a tudo abaixo de 'atron'
  {
    path: 'atron',
    canActivate: [AuthGuard],
    children: [
      { path: 'home', component: HomeComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'departamentos', canActivate: [ModuloGuard], data: { moduloCodigo: 'DPT' }, loadChildren: () => import('./plataforma/tracker/departamentos/departamento-routing.module').then(m => m.DepartamentoRoutingModule) },
      { path: 'cargos', canActivate: [ModuloGuard], data: { moduloCodigo: 'CRG' }, loadChildren: () => import('./plataforma/tracker/cargos/cargo-routing.module').then(m => m.CargoRoutingModule) },
      { path: 'planejamento-custos', canActivate: [ModuloGuard], data: { moduloCodigo: 'PLC' }, loadChildren: () => import('./plataforma/tracker/planejamento-custos/planejamento-custos.module').then(m => m.PlanejamentoCustosModule) },
      { path: 'usuarios', canActivate: [ModuloGuard], data: { moduloCodigo: 'USR' }, loadChildren: () => import('./plataforma/tracker/usuarios/usuario-routing.module').then(m => m.UsuarioRoutingModule) },
      { path: 'tarefas', canActivate: [ModuloGuard], data: { moduloCodigo: 'TAR' }, loadChildren: () => import('./plataforma/tracker/tarefas/tarefa-routing.module').then(m => m.TarefaRoutingModule) },
      { path: 'notificacoes', loadComponent: () => import('./plataforma/notificacoes/components/notificacao-interna-view/notificacao-interna-view.component').then(m => m.NotificacaoInternaViewComponent) },
      { path: 'perfil-de-acesso', canActivate: [ModuloGuard], data: { moduloCodigo: 'PERF' }, loadChildren: () => import('./plataforma/tracker/perfil-de-acesso/perfil-de-acesso.module').then(m => m.PerfilModule) },
    ]
  },

  // Rota coringa se precisar
  { path: '**', redirectTo: 'login' }
];
