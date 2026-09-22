using Application.DTO;
using Application.Mapping;
using Application.Services.EntitiesServices;
using Application.Services.EntitiesServices.Tenancy;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.DepartamentoCases
{
    public sealed class CriarDepartamentoCase(
        DepartamentoWorkspaceService _vincularDepartamentoWorkspaceService,
        VincularGestorDepartamentoService _vincularGestorDepartamento,
        DepartamentoMapping _mapper,
        IDepartamentoRepository _departamentoRepository,
        IValidador<DepartamentoDTO> _validador)
    {
        public async Task<Resultado> ExecutarAsync(DepartamentoDTO departamentoDTO)
        {
            var messages = _validador.Validar(departamentoDTO);
            if (messages.TemErros())
                return Resultado.Falha(messages);

            var departamentoExiste = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(departamentoDTO.Codigo);

            if (!departamentoExiste.IsNullable())
                return Resultado.Falha(DepartamentoResource.ErroCodigoDepartamentoExistente);

            var departamento = _mapper.MapToEntity(departamentoDTO);

            var resultadoGestor = await _vincularGestorDepartamento
                .ExecutarAsync(departamento, departamentoDTO.GestorDepartamentoCodigo);

            if (resultadoGestor.TeveFalha)
                return Resultado.Falha(resultadoGestor.Messages);

            var entidadeGravada = await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
            if (!entidadeGravada)
                return Resultado.Falha(DepartamentoResource.ErroGravacao);

            var resultadoTenant = await _vincularDepartamentoWorkspaceService.ExecutarAsync(departamentoDTO.Codigo);
            if (resultadoTenant.TeveFalha)
                return Resultado.Falha(resultadoTenant.Messages);


            return Resultado
                .Sucesso()
                .AdicionarMensagem(string.Format(
                    NotificacoesPadronizadas.Mensagem_RegistroSalvo,
                    departamento.Codigo));
        }
    }
}