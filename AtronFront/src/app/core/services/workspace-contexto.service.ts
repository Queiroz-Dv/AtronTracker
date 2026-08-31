import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { SelecionarWorkspaceRequest } from '../../shared/models/request/selecionar-workspace-request.model';
import { WorkspaceResponse } from '../../shared/models/response/registrar-response.model';
import { RotasApi } from '../../shared/models/rotas-api.model';

@Injectable({ providedIn: 'root' })
export class WorkspaceContextoService {
  private readonly workspaceAtualStorageKey = 'atron_workspace_atual_id';
  private readonly workspaceAtualSubject = new BehaviorSubject<WorkspaceResponse | null>(null);

  readonly workspaceAtual$ = this.workspaceAtualSubject.asObservable();

  constructor(private http: HttpClient) { }

  listar(): Observable<WorkspaceResponse[]> {
    return this.http.get<WorkspaceResponse[]>(RotasApi.workspaceEndpoint).pipe(
      tap(workspaces => this.restaurarReferenciaVisual(workspaces))
    );
  }

  selecionar(workspaceId: number): Observable<WorkspaceResponse> {
    const request: SelecionarWorkspaceRequest = { workspaceId };

    return this.http.post<WorkspaceResponse>(
      `${RotasApi.workspaceEndpoint}/selecionar`,
      request
    ).pipe(
      tap(workspace => this.definirReferenciaVisual(workspace))
    );
  }

  limpar(): void {
    sessionStorage.removeItem(this.workspaceAtualStorageKey);
    this.workspaceAtualSubject.next(null);
  }

  private restaurarReferenciaVisual(workspaces: WorkspaceResponse[]): void {
    const workspaceId = Number(sessionStorage.getItem(this.workspaceAtualStorageKey));
    const workspace = workspaces.find(item => item.id === workspaceId) ?? null;

    if (!workspace) {
      this.limpar();
      return;
    }

    this.workspaceAtualSubject.next(workspace);
  }

  private definirReferenciaVisual(workspace: WorkspaceResponse): void {
    sessionStorage.setItem(this.workspaceAtualStorageKey, workspace.id.toString());
    this.workspaceAtualSubject.next(workspace);
  }
}
