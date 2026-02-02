import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { HomeComponent } from './features/home/home.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./features/acesso/login/login.component').then(m => m.LoginComponent) },
  { path: 'registrar', loadComponent: () => import('./features/acesso/registrar/registrar.component').then(m => m.RegistrarComponent) },
  { path: 'confirmar-email', loadComponent: () => import('./features/acesso/confirmar-email/confirmar-email.component').then(m => m.ConfirmarEmailComponent) },
  // Aplica o guard a tudo abaixo de 'atron'
  {
    path: 'atron',
    canActivate: [AuthGuard],
    children: [
      { path: 'home', component: HomeComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'departamentos', loadChildren: () => import('./features/departamentos/departamento-routing.module').then(m => m.DepartamentoRoutingModule) },
      { path: 'cargos', loadChildren: () => import('./features/cargos/cargo-routing.module').then(m => m.CargoRoutingModule) },
      { path: 'usuarios', loadChildren: () => import('./features/usuarios/usuario-routing.module').then(m => m.UsuarioRoutingModule) },
      { path: 'tarefas', loadChildren: () => import('./features/tarefas/tarefa-routing.module').then(m => m.TarefaRoutingModule) },
      { path: 'salarios', loadChildren: () => import('./features/salarios/salario-routing.module').then(m => m.SalarioRoutingModule) },
      { path: 'perfil-de-acesso', loadChildren: () => import('./features/perfil-de-acesso/perfil-de-acesso.module').then(m => m.PerfilModule) },
    ]
  },

  // Rota coringa se precisar
  { path: '**', redirectTo: 'login' }
];
