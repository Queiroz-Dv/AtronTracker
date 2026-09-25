using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.EntitiesServices;

namespace Application.UseCases.DepartamentoCases
{
    public sealed class AtualizarDepartamentoCase(
        VincularGestorDepartamentoService _vincularGestorDepartamento,
        DepartamentoMapping _mapper,
        IDepartamentoRepository _departamentoRepository,
        IValidador<DepartamentoDTO> _validador)
    {
        public async Task<Resultado> ExecutarAsync(string codigo, DepartamentoDTO departamentoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var erros = _validador.Validar(departamentoDTO);
            if (erros.Any())
                return Resultado.Falha(erros);

            var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (entidade == null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            entidade.MapToUpdate(departamentoDTO, _mapper);

            var resultadoGestor = await _vincularGestorDepartamento.ExecutarAsync(entidade, departamentoDTO.GestorDepartamentoCodigo);

            if (resultadoGestor.TeveFalha)
                return Resultado.Falha(resultadoGestor.Messages);

            var atualizado = await _departamentoRepository.AtualizarDepartamentoRepositoryAsync(entidade);
            if (!atualizado)
                return Resultado.Falha(string.Format(DepartamentoResource.ErroInesperadoAtualizacao, codigo));

            return Resultado
                .Sucesso()
                .AdicionarMensagem(string.Format(DepartamentoResource.MensagemAtualizacao, codigo));
        }
    }
}
