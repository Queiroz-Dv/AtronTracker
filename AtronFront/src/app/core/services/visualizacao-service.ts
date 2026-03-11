import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class VisualizacaoService {
  private readonly VIEW_MODE_KEY = 'view-mode';

  setViewMode(mode: 'dashboard' | 'menu') {
    localStorage.setItem(this.VIEW_MODE_KEY, mode);
  }

  getViewMode(): 'dashboard' | 'menu' {
    return (localStorage.getItem(this.VIEW_MODE_KEY) as 'dashboard' | 'menu') ?? 'dashboard';
  }
}
