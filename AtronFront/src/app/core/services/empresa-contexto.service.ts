import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RotasApi } from '../../shared/models/rotas-api.model';

export interface ContextoEmpresa {
  empresaId: number | null;
  codigo: string | null;
  nomeFantasia: string | null;
  acessoPermitido: boolean;
  motivoBloqueio: string | null;
}

export interface EmpresaBusca {
  id: number;
  codigo: string;
  nomeFantasia: string;
}

export interface EmpresaCadastro {
  codigo: string;
  nomeFantasia: string;
  endereco: { logradouro: string };
  numero: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class EmpresaContextoService {
  constructor(private http: HttpClient) { }

  obter(): Observable<ContextoEmpresa> {
    return this.http.get<ContextoEmpresa>(RotasApi.empresaContextoEndpoint);
  }

  buscar(termo: string): Observable<EmpresaBusca[]> {
    return this.http.get<EmpresaBusca[]>(`${RotasApi.empresaEndpoint}/Busca`, { params: { termo } });
  }

  cadastrar(request: EmpresaCadastro): Observable<unknown> {
    return this.http.post(RotasApi.empresaEndpoint, request);
  }

  solicitar(empresaId: number): Observable<unknown> {
    return this.http.post(`${RotasApi.empresaEndpoint}/Solicitacoes`, { empresaId });
  }
}
