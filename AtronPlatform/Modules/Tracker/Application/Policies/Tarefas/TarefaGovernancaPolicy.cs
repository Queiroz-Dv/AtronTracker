using Application.Resources;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;

namespace Application.Policies.Tarefas
{
    public static class TarefaGovernancaPolicy
    {
        public static Resultado Validar(Usuario usuario, Departamento departamentoTarefa)
        {
            if (usuario is null || TemGestorImediato(usuario))
                return Resultado.Sucesso();

            if (TemGestorDepartamento(departamentoTarefa))
                return Resultado.Sucesso();

            var departamentoDoUsuarioComGestor = usuario.UsuarioCargoDepartamentos?
                .Any(relacionamento => TemGestorDepartamento(relacionamento.Departamento)) == true;

            return departamentoDoUsuarioComGestor
                ? Resultado.Sucesso()
                : Resultado.Falha(TarefaResource.Erro_AprovadorObrigatorio);
        }

        private static bool TemGestorImediato(Usuario usuario)
            => usuario.GestorImediatoId.HasValue && !usuario.GestorImediatoCodigo.IsNullOrEmpty();

        private static bool TemGestorDepartamento(Departamento departamento)
            => departamento is not null &&
               departamento.GestorDepartamentoId.HasValue &&
               !departamento.GestorDepartamentoCodigo.IsNullOrEmpty();
    }
}
