using Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Application.Extensions
{
    public static class UsuarioTarefaExtensions
    {
        public static IReadOnlyCollection<int> ObterDepartamentoIdsParaTarefas(this Usuario usuario)
        {
            return usuario.UsuarioCargoDepartamentos?
                .Select(relacao => relacao.DepartamentoId)
                .Distinct()
                .ToList() ?? [];
        }

        public static IReadOnlyCollection<int> ObterCargoIdsParaTarefas(this Usuario usuario)
        {
            return usuario.UsuarioCargoDepartamentos?
                .Select(relacao => relacao.CargoId)
                .Distinct()
                .ToList() ?? [];
        }
    }
}
