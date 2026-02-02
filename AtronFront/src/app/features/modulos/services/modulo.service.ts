import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ModuloModel } from '../interfaces/modulo.interface';
import { BaseService } from '../../../core/services/base-service';
import { RotasApi } from '../../../shared/models/rotas-api.model';

@Injectable({
  providedIn: 'root'
})
export class ModuloService extends BaseService<ModuloModel> {

  constructor(http: HttpClient) { super(http, RotasApi.moduloEndpoint) }
}