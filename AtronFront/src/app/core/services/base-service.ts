import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Mensagem } from './notification.service';

export abstract class BaseService<T> {
  constructor(protected http: HttpClient, protected endpoint: string) { }

  obterTodos(): Observable<T[]> {
    return this.http.get<T[]>(this.endpoint)
      .pipe(catchError(this.handleError));
  }

  obterPorCodigo(codigo: string | number): Observable<T> {
    if (typeof codigo === 'number') {
      return this.obterPorId(codigo);
    }
    return this.http.get<T>(`${this.endpoint}/${codigo}`)
      .pipe(catchError(this.handleError));
  }

  obterPorId(id: number): Observable<T> {
    return this.http.get<T>(`${this.endpoint}/${id}`)
      .pipe(catchError(this.handleError));
  }

  gravarPorRequest<R>(requestModel: R): Observable<R> {
    return this.http.post<R>(this.endpoint, requestModel)
      .pipe(catchError(this.handleError));
  }

  gravar(model: T): Observable<T> {
    return this.http.post<T>(this.endpoint, model)
      .pipe(catchError(this.handleError));
  }

  atualizar(codigo: string | number, model: T): Observable<T> {
    return this.http.put<T>(`${this.endpoint}/${codigo}`, model)
      .pipe(catchError(this.handleError));
  }

  deletar(codigo: string | number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${codigo}`)
      .pipe(catchError(this.handleError));
  }

  protected handleError(error: any): Observable<never> {
    console.error('[BaseService] Erro detectado:', error);
    return throwError(() => error);
  }
}

type RespostaApi = Mensagem[] | any;

export abstract class BaseGenericService<TRequest, TResponse = TRequest> {
  constructor(protected http: HttpClient, protected endpoint: string) { }

  obterTodos(): Observable<TResponse[]> {
    return this.http.get<TResponse[]>(this.endpoint)
      .pipe(catchError(this.tratarErro));
  }

  obterPorCodigo(codigo: string | number): Observable<TResponse> {
    if (typeof codigo === 'number') {
      return this.obterPorId(codigo);
    }
    return this.http.get<TResponse>(`${this.endpoint}/${codigo}`)
      .pipe(catchError(this.tratarErro));
  }

  obterPorId(id: number): Observable<TResponse> {
    return this.http.get<TResponse>(`${this.endpoint}/${id}`)
      .pipe(catchError(this.tratarErro));
  }

  gravar(model: TRequest): Observable<RespostaApi> {
    return this.http.post<TResponse>(this.endpoint, model)
      .pipe(catchError(this.tratarErro));
  }

  atualizar(codigo: string | number, model: TRequest): Observable<RespostaApi> {
    return this.http.put<TResponse>(`${this.endpoint}/${codigo}`, model)
      .pipe(catchError(this.tratarErro));
  }

  deletar(codigo: string | number): Observable<RespostaApi> {
    return this.http.delete<void>(`${this.endpoint}/${codigo}`)
      .pipe(catchError(this.tratarErro));
  }

  protected extrairCorpoErro(erro: any): Mensagem[] | null {
    const corpo = this.normalizarCorpoErro(erro?.error);
    const mensagens = Array.isArray(corpo)
      ? corpo
      : corpo?.mensagensApi
        ?? corpo?.mensagens
        ?? corpo?.messages
        ?? corpo?.Messages
        ?? corpo?.errors
        ?? corpo?.Errors
        ?? null;

    if (mensagens && typeof mensagens === 'object' && !Array.isArray(mensagens)) {
      const mensagensValidacao = Object.values(mensagens)
        .flatMap(valor => Array.isArray(valor) ? valor : [valor])
        .filter((valor): valor is string => typeof valor === 'string');

      return mensagensValidacao.length
        ? mensagensValidacao.map(descricao => ({ descricao, nivel: 'Error' } as Mensagem))
        : null;
    }

    if (!Array.isArray(mensagens)) {
      return null;
    }

    const mensagensNormalizadas = mensagens
      .map((mensagem: any) => {
        if (typeof mensagem === 'string') {
          return { descricao: mensagem, nivel: 'Error' } as Mensagem;
        }

        const descricao = mensagem?.descricao ?? mensagem?.Descricao ?? mensagem?.message ?? mensagem?.Message;
        const nivel = mensagem?.nivel ?? mensagem?.Nivel ?? mensagem?.level ?? mensagem?.Level ?? 'Error';

        if (!descricao || !nivel) {
          return null;
        }

        return { descricao, nivel } as Mensagem;
      })
      .filter((mensagem: Mensagem | null): mensagem is Mensagem => !!mensagem);

    return mensagensNormalizadas.length ? mensagensNormalizadas : null;
  }

  private normalizarCorpoErro(corpo: any): any {
    if (typeof corpo !== 'string') {
      return corpo;
    }

    try {
      return JSON.parse(corpo);
    } catch {
      return corpo;
    }
  }

  protected tratarErro = (erro: any): Observable<never> => {
    console.error('[BaseService] Erro detectado:', erro);
    const mensagensErro = this.extrairCorpoErro(erro);
    if (mensagensErro) {
      return throwError(() => ({ mensagensApi: mensagensErro, erroOriginal: erro }));
    }

    return throwError(() => erro);
  };
}
