import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { SelecionarWorkspaceRequest } from '../../shared/models/request/selecionar-workspace-request.model';
import { WorkspaceResponse } from '../../shared/models/response/registrar-response.model';
import { RotasApi } from '../../shared/models/rotas-api.model';

@Injectable({ providedIn: 'root' })
export class WorkspaceContextoService {
  private readonly workspaceAtualSubject = new BehaviorSubject<WorkspaceResponse | null>(null);

  readonly workspaceAtual$ = this.workspaceAtualSubject.asObservable();

  constructor(private http: HttpClient) { }

  listar(): Observable<WorkspaceResponse[]> {
    return this.http.get<WorkspaceResponse[]>(RotasApi.workspaceEndpoint).pipe(
      tap(workspaces => this.confirmarWorkspaceAtualNaLista(workspaces))
    );
  }

  selecionar(workspaceId: number): Observable<WorkspaceResponse> {
    const request: SelecionarWorkspaceRequest = { workspaceId };

    return this.http.post<WorkspaceResponse>(
      `${RotasApi.workspaceEndpoint}/selecionar`,
      request
    ).pipe(
      tap(workspace => this.definirWorkspaceAtual(workspace))
    );
  }

  limpar(): void {
    this.workspaceAtualSubject.next(null);
  }

  sincronizarComSessao(workspace: WorkspaceResponse | null): void {
    this.workspaceAtualSubject.next(workspace);
  }

  private confirmarWorkspaceAtualNaLista(workspaces: WorkspaceResponse[]): void {
    const workspaceAtual = this.workspaceAtualSubject.value;
    const workspace = workspaceAtual
      ? workspaces.find(item => item.id === workspaceAtual.id) ?? null
      : null;

    if (!workspace) {
      this.limpar();
      return;
    }

    this.workspaceAtualSubject.next(workspace);
  }

  private definirWorkspaceAtual(workspace: WorkspaceResponse): void {
    this.workspaceAtualSubject.next(workspace);
  }
}
