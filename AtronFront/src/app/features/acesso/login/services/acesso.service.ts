import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, catchError, filter, map, Observable, shareReplay, throwError } from 'rxjs';
import { LoginRequest } from '../../../../shared/models/request/login-request.model';
import { RotasApi } from '../../../../shared/models/rotas-api.model';
import { RegistrarRequest } from '../../../../shared/models/request/registrar-request.model';
import { DadosDoUsuario } from '../models/dados-do-usuario.model';
import { ModuloModel } from '../../../modulos/interfaces/modulo.interface';
import { SessaoInfoService } from '../../../../shared/services/sessaoInfo.service';
import { UserToken } from '../models/userToken';

@Injectable({
  providedIn: 'root'
})
export class AcessoService {

  private sessionInfoSubject = new BehaviorSubject<DadosDoUsuario | null>(null);
  private sessionInfo$: Observable<DadosDoUsuario> = this.sessionInfoSubject.asObservable();

  /** Exponha apenas os módulos acessíveis ao consumidor externo */
  public modulosAcessiveis$: Observable<ModuloModel[]> = this.sessionInfo$.pipe(
    filter(info => info !== null), // Filtra valores nulos
    map((info) => info.perfisDeAcesso.flatMap((perfil) => perfil.modulos)),
  );

  constructor(private http: HttpClient, private sessaoService: SessaoInfoService) {
    //this.fetchSessionInfoFromApi();
  }

  logout(): Observable<boolean> {
    return this.http.get<{ deslogado: boolean }>(RotasApi.desconectarEndpoint).pipe(
      map(response => {
        if (response.deslogado) {
          this.sessionInfo$ = null;
          this.modulosAcessiveis$ = null;
        }
        return response.deslogado;
      })
    );
  }

  autenticar(login: LoginRequest): Observable<void> {
    return this.http.post<UserToken>(RotasApi.logarEndpoint, login).pipe(
      map(response => {
        this.sessaoService.setUsuarioInfo(response.token, response.expires, login.codigoDoUsuario);
        this.fetchSessionInfoFromApi(); // Fetch session info after login  
        // Se necessário, armazene o token em outro lugar
      }),
      catchError(error => {
        // Trate o erro conforme necessário
        return throwError(() => error);
      })
    );
  }

  private fetchSessionInfoFromApi(): void {
    const token = this.sessaoService.getToken();
    if (!token) return;

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.get<DadosDoUsuario>(RotasApi.sessionInfoEndpoint, { headers }).subscribe({
      next: (info) => this.sessionInfoSubject.next(info),
      error: (err) => {
        console.error('Erro ao buscar sessão:', err);
        this.sessionInfoSubject.next(null);
      }
    });
  }


  registrar(dadosDoUsuario: RegistrarRequest): Observable<string[]> {
    return this.http.post<string[]>(RotasApi.registrarEndpoint, dadosDoUsuario).pipe(
      map((response) => {
        // O backend retorna um array de mensagens/notificações
        // Se chegou aqui (HTTP 200), o registro foi bem-sucedido
        return response || [];
      }),
      catchError((error) => {
        console.error('Erro ao registrar usuário:', error);
        return throwError(() => error); // Propague o erro para o consumidor
      })
    );
  }

  confirmarEmail(confirmarEmailRequest: ConfirmarEmailRequest): Observable<string> {
    
    return this.http.post<string>(RotasApi.confirmarEmailEndpoint, confirmarEmailRequest).pipe(
      map(response => {
        console.log(response);
        return response;
      }),
      catchError((error) => {
        console.error('Erro ao confirmar email:', error);
        return throwError(() => error);
      })
    );
  }
}

export class ConfirmarEmailRequest{
  public usuarioCodigo: string;
  public token: string
}