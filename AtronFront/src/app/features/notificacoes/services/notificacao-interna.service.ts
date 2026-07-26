import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { RotasApi } from '../../../shared/models/rotas-api.model';
import { NotificacaoInterna } from '../models/notificacao-interna.model';

@Injectable({
  providedIn: 'root'
})
export class NotificacaoInternaService {
  private notificacoesSubject = new BehaviorSubject<NotificacaoInterna[]>([]);

  notificacoes$ = this.notificacoesSubject.asObservable();
  totalNaoLidas$ = this.notificacoes$.pipe(
    map(notificacoes => notificacoes.filter(notificacao => !notificacao.lida).length)
  );

  constructor(private http: HttpClient) { }

  carregar(): Observable<NotificacaoInterna[]> {
    return this.http.get<NotificacaoInterna[]>(RotasApi.notificacaoInternaEndpoint).pipe(
      tap(notificacoes => this.notificacoesSubject.next(notificacoes))
    );
  }

  marcarComoLida(id: number): Observable<NotificacaoInterna> {
    return this.http.post<NotificacaoInterna>(`${RotasApi.notificacaoInternaEndpoint}/${id}/marcar-como-lida`, {}).pipe(
      tap(notificacaoAtualizada => {
        const notificacoes = this.notificacoesSubject.value.map(notificacao =>
          notificacao.id === notificacaoAtualizada.id ? notificacaoAtualizada : notificacao
        );
        this.notificacoesSubject.next(notificacoes);
      })
    );
  }

  marcarTodasComoLidas(): Observable<NotificacaoInterna[]> {
    return this.http.post<NotificacaoInterna[]>(`${RotasApi.notificacaoInternaEndpoint}/marcar-todas-como-lidas`, {}).pipe(
      tap(notificacoes => this.notificacoesSubject.next(notificacoes))
    );
  }

  excluir(id: number): Observable<void> {
    return this.http.delete<void>(`${RotasApi.notificacaoInternaEndpoint}/${id}`).pipe(
      tap(() => this.notificacoesSubject.next(
        this.notificacoesSubject.value.filter(notificacao => notificacao.id !== id)
      ))
    );
  }

  limparCache(): void {
    this.notificacoesSubject.next([]);
  }
}
