using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class AprovadorObtencaoTarefaResolver : IAprovadorObtencaoTarefaResolver
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AprovadorObtencaoTarefaResolver(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario> ResolverAsync(Usuario solicitante, Tarefa tarefa)
        {
            var gestorImediato = await ObterAprovadorAsync(solicitante.GestorImediatoCodigo, solicitante);
            if (gestorImediato is not null)
                return gestorImediato;

            var gestorDepartamentoTarefa = await ObterAprovadorAsync(tarefa.Departamento?.GestorDepartamentoCodigo, solicitante);
            if (gestorDepartamentoTarefa is not null)
                return gestorDepartamentoTarefa;

            var departamentoSolicitante = solicitante.UsuarioCargoDepartamentos?
                .Select(relacao => relacao.Departamento)
                .FirstOrDefault(departamento => departamento is not null);

            return await ObterAprovadorAsync(departamentoSolicitante?.GestorDepartamentoCodigo, solicitante);
        }

        private async Task<Usuario> ObterAprovadorAsync(string codigo, Usuario solicitante)
        {
            return codigo.IsNullOrEmpty() || codigo == solicitante.Codigo
                ? null
                : await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
        }
    }
}
