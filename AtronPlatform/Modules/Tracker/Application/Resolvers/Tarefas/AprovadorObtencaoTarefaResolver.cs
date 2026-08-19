using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Resolvers.Tarefas
{
    public sealed class AprovadorObtencaoTarefaResolver(IUsuarioRepository usuarioRepository)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Usuario> ResolverAsync(Usuario solicitante, Tarefa tarefa)
        {
            var codigosCandidatos = ObterCodigosCandidatos(solicitante, tarefa)
                .Where(codigo => !codigo.IsNullOrEmpty() &&
                !string.Equals(codigo, solicitante.Codigo))
                .Distinct();

            foreach (var codigo in codigosCandidatos)
            {
                var aprovador = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
                if (aprovador is not null)
                    return aprovador;
            }

            return null;
        }

        private static List<string> ObterCodigosCandidatos(Usuario solicitante, Tarefa tarefa)
        {
            var codigos = new List<string>
            {
                solicitante.GestorImediatoCodigo,
                tarefa.Departamento?.GestorDepartamentoCodigo
            };

            var gestoresDosDepartamentos = solicitante.UsuarioCargoDepartamentos?
                .Where(relacao => relacao.Departamento is not null)
                .OrderBy(relacao => relacao.DepartamentoCodigo)
                .Select(relacao => relacao.Departamento.GestorDepartamentoCodigo);

            if (gestoresDosDepartamentos is not null)
                codigos.AddRange(gestoresDosDepartamentos);

            return codigos;
        }
    }
}