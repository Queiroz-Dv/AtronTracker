import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { ModuloGuard } from './core/guards/modulo.guard';
import { HomeComponent } from './features/home/home.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./features/acesso/login/login.component').then(m => m.LoginComponent) },
  { path: 'registrar', loadComponent: () => import('./features/acesso/registrar/registrar.component').then(m => m.RegistrarComponent) },
  { path: 'confirmar-email', loadComponent: () => import('./features/acesso/confirmar-email/confirmar-email.component').then(m => m.ConfirmarEmailComponent) },
  { path: 'esqueci-senha', loadComponent: () => import('./features/acesso/esqueci-senha/esqueci-senha.component').then(m => m.EsqueciSenhaComponent) },
  { path: 'trocar-senha', loadComponent: () => import('./features/acesso/trocar-senha/trocar-senha.component').then(m => m.TrocarSenhaComponent) },
  // Aplica o guard a tudo abaixo de 'atron'
  {
    path: 'atron',
    canActivate: [AuthGuard],
    children: [
      { path: 'home', component: HomeComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'departamentos', canActivate: [ModuloGuard], data: { moduloCodigo: 'DPT' }, loadChildren: () => import('./features/departamentos/departamento-routing.module').then(m => m.DepartamentoRoutingModule) },
      { path: 'cargos', canActivate: [ModuloGuard], data: { moduloCodigo: 'CRG' }, loadChildren: () => import('./features/cargos/cargo-routing.module').then(m => m.CargoRoutingModule) },
      { path: 'usuarios', canActivate: [ModuloGuard], data: { moduloCodigo: 'USR' }, loadChildren: () => import('./features/usuarios/usuario-routing.module').then(m => m.UsuarioRoutingModule) },
      { path: 'tarefas', canActivate: [ModuloGuard], data: { moduloCodigo: 'TAR' }, loadChildren: () => import('./features/tarefas/tarefa-routing.module').then(m => m.TarefaRoutingModule) },
      { path: 'planejamento-custos', canActivate: [ModuloGuard], data: { moduloCodigo: 'PLC' }, loadChildren: () => import('./features/planejamento-custos/planejamento-custos.module').then(m => m.PlanejamentoCustosModule) },
      { path: 'perfil-de-acesso', canActivate: [ModuloGuard], data: { moduloCodigo: 'PERF' }, loadChildren: () => import('./features/perfil-de-acesso/perfil-de-acesso.module').then(m => m.PerfilModule) },
    ]
  },

  // Rota coringa se precisar
  { path: '**', redirectTo: 'login' }
];
