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
            var codigosCandidatos = await ObterCodigosCandidatos(solicitante, tarefa);
            var codigosHash = codigosCandidatos.Where(x => !x.IsNullOrEmpty()).ToHashSet();
            
            foreach (var codigo in codigosHash)
            {
                var aprovador = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
                if (aprovador is not null)
                    return aprovador;
            }

            return null;
        }

        private async Task<List<string>> ObterCodigosCandidatos(Usuario solicitante, Tarefa tarefa)
        {
            var codigos = new List<string>
            {
                solicitante.GestorImediatoCodigo,
                tarefa.Departamento?.GestorDepartamentoCodigo
            };

            var gestoresDosDepartamentos = solicitante.UsuarioCargoDepartamentos?
                .Where(relacao => !relacao.Departamento.IsNullable())
                .OrderBy(relacao => relacao.DepartamentoCodigo)
                .Select(relacao => relacao.Departamento.GestorDepartamentoCodigo);           

            if (!gestoresDosDepartamentos.IsNullable())            
                codigos.AddRange(gestoresDosDepartamentos);
            
            return codigos;
        }
    }
}