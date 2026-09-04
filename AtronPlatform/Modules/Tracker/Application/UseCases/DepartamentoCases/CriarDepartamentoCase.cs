using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.DepartamentoCases
{
    public sealed class CriarDepartamentoCase(
        VincularGestorDepartamentoCase vincularGestorDepartamento,
        DepartamentoMapping mapper,
        IDepartamentoRepository repository,
        IValidador<DepartamentoDTO> validador)
    {
        private readonly VincularGestorDepartamentoCase _vincularGestorDepartamento = vincularGestorDepartamento;
        private readonly DepartamentoMapping _mapper = mapper;
        private readonly IDepartamentoRepository _departamentoRepository = repository;
        private readonly IValidador<DepartamentoDTO> _validador = validador;

        public async Task<Resultado> ExecutarAsync(DepartamentoDTO departamentoDTO)
        {
            var erros = _validador.Validar(departamentoDTO);
            if (erros.TemErros())
                return Resultado.Falha(erros);

            var departamentoExiste = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(departamentoDTO.Codigo);
            if (departamentoExiste != null)
                return Resultado.Falha(DepartamentoResource.ErroCodigoDepartamentoExistente);

            var departamento = _mapper.MapToEntity(departamentoDTO);

            var resultadoGestor = await _vincularGestorDepartamento
                .ExecutarAsync(departamento, departamentoDTO.GestorDepartamentoCodigo);

            if (resultadoGestor.TeveFalha)
                return Resultado.Falha(resultadoGestor.Messages);

            var foiCriado = await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
            if (!foiCriado)
                return Resultado.Falha(DepartamentoResource.ErroGravacao);

            return Resultado
                .Sucesso()
                .AdicionarMensagem(string.Format(
                    NotificacoesPadronizadas.ResourceManager.GetString("Mensagem_RegistroSalvo")!,
                    departamento.Codigo));
        }
    }
}
