import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RotasApi } from '../../../shared/models/rotas-api.model';
import { BaseGenericService } from '../../../core/services/base-service';
import { CargoRequest } from '../models/request/cargo-request.model';
import { CargoResponse } from '../models/response/cargo-response.model';

@Injectable({ providedIn: 'root' })

export class CargoService extends BaseGenericService<CargoRequest, CargoResponse> {

  constructor(http: HttpClient) {
    super(http, RotasApi.cargoEndpoint)
  }
}