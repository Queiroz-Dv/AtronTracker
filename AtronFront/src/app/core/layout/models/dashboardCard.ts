export interface DashboardCard {
  code: string; // código único para o card
  title: string;
  icon: string;        // nome do ícone Material ou classe FontAwesome
  description: string;
  route: string;       // rota Angular para navegação
  cols: number;
  rows: number;
}