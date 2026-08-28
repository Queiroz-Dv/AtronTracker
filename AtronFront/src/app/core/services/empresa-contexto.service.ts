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

@Injectable({ providedIn: 'root' })
export class EmpresaContextoService {
  constructor(private http: HttpClient) { }

  obter(): Observable<ContextoEmpresa> {
    return this.http.get<ContextoEmpresa>(RotasApi.empresaContextoEndpoint);
  }
}
