using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.DepartamentoCases
{
    public sealed class VincularGestorDepartamentoCase(IUsuarioRepository usuarioRepository)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Resultado> ExecutarAsync(Departamento departamento, string gestorCodigo)
        {
            if (gestorCodigo.IsNullOrEmpty())
            {
                departamento.GestorDepartamentoId = null;
                departamento.GestorDepartamentoCodigo = null;
                return Resultado.Sucesso();
            }

            var gestor = await _usuarioRepository.ObterUsuarioPorCodigoAsync(gestorCodigo.ToUpper());
            if (gestor is null)
                return Resultado.Falha(DepartamentoResource.ErroGestorNaoEncontrado);

            departamento.GestorDepartamentoId = gestor.Id;
            departamento.GestorDepartamentoCodigo = gestor.Codigo;

            return Resultado.Sucesso();
        }
    }
}
