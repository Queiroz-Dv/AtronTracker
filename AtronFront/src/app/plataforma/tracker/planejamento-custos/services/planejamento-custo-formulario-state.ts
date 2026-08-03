import { FormArray, FormBuilder } from '@angular/forms';
import { CargoResponse } from '../../cargos/models/response/cargo-response.model';
import { PlanejamentoCustoCargoRequest } from '../models/request/planejamento-custo-request.model';
import { PlanejamentoCustoCargoResponse } from '../models/response/planejamento-custo-response.model';

interface PlanejamentoCustoCargoFormulario {
  cargoCodigo: string;
  cargoDescricao?: string;
  detalhado: boolean;
  valorMinimo?: number | string | null;
  valorTeto?: number | string | null;
}

export interface ResultadoDetalhesCargoFormulario {
  detalhesCargo: PlanejamentoCustoCargoRequest[];
}

export class PlanejamentoCustoFormularioState {
  static sincronizarDetalhesCargo(
    fb: FormBuilder,
    detalhesCargo: FormArray,
    departamentoCodigo: string | null | undefined,
    todosCargos: CargoResponse[],
    detalhesExistentes: PlanejamentoCustoCargoResponse[] | PlanejamentoCustoCargoFormulario[] = []
  ): void {
    if (!departamentoCodigo || !todosCargos.length) {
      detalhesCargo.clear();
      return;
    }

    const detalhesAtuais = detalhesCargo.getRawValue() as PlanejamentoCustoCargoFormulario[];
    const detalhesPorCargo = [...detalhesAtuais, ...detalhesExistentes]
      .reduce((mapa, detalhe) => {
        mapa.set(detalhe.cargoCodigo, detalhe);
        return mapa;
      }, new Map<string, PlanejamentoCustoCargoFormulario | PlanejamentoCustoCargoResponse>());

    const cargosDepartamento = todosCargos
      .filter(cargo => cargo.departamentoCodigo === departamentoCodigo)
      .sort((a, b) => a.descricao.localeCompare(b.descricao));

    detalhesCargo.clear();

    cargosDepartamento.forEach(cargo => {
      const detalhe = detalhesPorCargo.get(cargo.codigo);
      detalhesCargo.push(fb.group({
        cargoCodigo: [cargo.codigo],
        cargoDescricao: [cargo.descricao],
        detalhado: [detalhe?.detalhado ?? false],
        valorMinimo: [detalhe?.valorMinimo ?? null],
        valorTeto: [detalhe?.valorTeto ?? null]
      }));
    });
  }

  static montarDetalhesCargo(dadosForm: { apenasDepartamento: boolean; detalhesCargo?: PlanejamentoCustoCargoFormulario[] }): ResultadoDetalhesCargoFormulario {
    if (dadosForm.apenasDepartamento) {
      return { detalhesCargo: [] };
    }

    const detalhes = dadosForm.detalhesCargo ?? [];

    return {
      detalhesCargo: detalhes.map(detalhe => new PlanejamentoCustoCargoRequest(
        detalhe.cargoCodigo,
        Boolean(detalhe.detalhado),
        detalhe.detalhado ? this.normalizarValorMonetario(detalhe.valorMinimo) : null,
        detalhe.detalhado ? this.normalizarValorMonetario(detalhe.valorTeto) : null
      ))
    };
  }

  private static normalizarValorMonetario(valor: number | string | null | undefined): number | null {
    if (valor === null || valor === undefined || valor === '') {
      return null;
    }

    const numero = Number(valor);
    return Number.isNaN(numero) ? null : numero;
  }
}
