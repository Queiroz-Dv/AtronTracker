using Application.DTO;
using Application.Resources;
using Domain.Entities;
using Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Linq;

namespace Application.UseCases.TarefaCases
{
    public sealed class DefinirDepartamentoEquipePorTarefaCase
    {
        public static Resultado Executar(TarefaDTO tarefaDTO, Usuario responsavel)
        {
            if (tarefaDTO.DestinoInicial != DestinoInicialTarefa.Equipe.GetDescription())
                return Resultado.Sucesso();

            var departamentos = responsavel.UsuarioCargoDepartamentos?
                .Select(relacionamento => relacionamento.DepartamentoCodigo)
                .Where(codigo => !string.IsNullOrWhiteSpace(codigo))
                .Distinct()
                .ToList() ?? [];

            if (departamentos.Count != 1)
                return Resultado.Falha(TarefaResource.Erro_DepartamentoEquipeIndefinido);

            tarefaDTO.UsuarioCodigo = null;
            tarefaDTO.DepartamentoCodigo = departamentos[0];
            tarefaDTO.CargoCodigo = null;

            return Resultado.Sucesso();
        }
    }
}
