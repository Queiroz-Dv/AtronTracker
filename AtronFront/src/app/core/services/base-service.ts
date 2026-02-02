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
    if (typeof codigo === "number") {
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
    // Aqui você pode disparar notificações, enviar logs, etc.
    return throwError(() => error);
  }
}

type RespostaApi = Mensagem[] | any; 

export abstract class BaseGenericService<TRequest, TResponse = TRequest> { 
  constructor(protected http: HttpClient, protected endpoint: string) { }

  obterTodos(): Observable<TResponse[]> { // <--- Retorna TResponse
    return this.http.get<TResponse[]>(this.endpoint)
      .pipe(catchError(this.tratarErro));
  }

  // Mantém a lógica original, mas o tipo de retorno agora é TResponse
  obterPorCodigo(codigo: string | number): Observable<TResponse> {
    if (typeof codigo === "number") {
      return this.obterPorId(codigo);
    }
    return this.http.get<TResponse>(`${this.endpoint}/${codigo}`)
      .pipe(catchError(this.tratarErro));
  }

  obterPorId(id: number): Observable<TResponse> { // <--- Retorna TResponse
    return this.http.get<TResponse>(`${this.endpoint}/${id}`)
      .pipe(catchError(this.tratarErro));
  }

  // Recebe TRequest, retorna TResponse (assumindo que a API retorna o objeto criado/atualizado)
  gravar(model: TRequest): Observable<RespostaApi> {
    return this.http.post<TResponse>(this.endpoint, model)
      .pipe(catchError(this.tratarErro));
  }

  // Recebe TRequest, retorna TResponse
  atualizar(codigo: string | number, model: TRequest): Observable<RespostaApi> {
    return this.http.put<TResponse>(`${this.endpoint}/${codigo}`, model)
      .pipe(catchError(this.tratarErro));
  }

  deletar(codigo: string | number): Observable<RespostaApi> {
    return this.http.delete<void>(`${this.endpoint}/${codigo}`)
      .pipe(catchError(this.tratarErro));
  }

// Método auxiliar para extrair o corpo da resposta em caso de erro (BadRequest)
  protected extrairCorpoErro(erro: any): Mensagem[] | null { // Nome do método alterado
      if (erro.error && Array.isArray(erro.error)) {
        // Tenta verificar se o corpo do erro é o array de mensagens
        if (erro.error.length > 0 && erro.error[0].hasOwnProperty('Descricao') && erro.error[0].hasOwnProperty('Nivel')) {
           return erro.error as Mensagem[];
        }
      }
      return null; // Retorna null se não for o formato esperado
  }

  protected tratarErro(erro: any): Observable<never> { // Nome do método alterado
    console.error('[BaseService] Erro detectado:', erro);
    // Tenta extrair as mensagens do corpo do erro (para BadRequest)
    const mensagensErro = this.extrairCorpoErro(erro); // Nome do método alterado
    if (mensagensErro) {
        // Se conseguimos extrair as mensagens, passamos elas adiante no erro
        return throwError(() => ({ mensagensApi: mensagensErro, erroOriginal: erro })); // Nome da propriedade alterado
    }
    // Caso contrário, lança o erro original
    return throwError(() => erro);
  }
}