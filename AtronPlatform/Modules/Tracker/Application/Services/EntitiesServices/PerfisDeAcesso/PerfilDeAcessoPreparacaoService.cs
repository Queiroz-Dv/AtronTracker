using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.PerfisDeAcesso
{
    public class PerfilDeAcessoPreparacaoService(
        IToEntityMapper<PerfilDeAcesso, PerfilDeAcessoDTO> map,
        IModuloRepository moduloRepository,
        IValidador<PerfilDeAcessoDTO> validador) : IPerfilDeAcessoPreparacaoService
    {
        private readonly IToEntityMapper<PerfilDeAcesso, PerfilDeAcessoDTO> _map = map;
        private readonly IModuloRepository _moduloRepository = moduloRepository;
        private readonly IValidador<PerfilDeAcessoDTO> _validador = validador;

        public async Task<Resultado<PerfilDeAcesso>> PrepararAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var mensagens = _validador.Validar(perfilDeAcessoDTO);
            if (mensagens.Any())
                return Resultado<PerfilDeAcesso>.Falhas(mensagens);

            var perfilDeAcesso = _map.MapToEntity(perfilDeAcessoDTO);
            var vinculacao = await VincularModulosAsync(perfilDeAcessoDTO, perfilDeAcesso);
            if (vinculacao.TeveFalha)
                return Resultado<PerfilDeAcesso>.Falhas(vinculacao.Messages);

            return Resultado<PerfilDeAcesso>.Sucesso(perfilDeAcesso);
        }

        private async Task<Resultado> VincularModulosAsync(
            PerfilDeAcessoDTO perfilDeAcessoDTO,
            PerfilDeAcesso perfilDeAcesso)
        {
            foreach (var moduloDTO in perfilDeAcessoDTO.Modulos)
            {
                var modulo = await _moduloRepository.ObterPorCodigoRepository(moduloDTO.Codigo);
                if (modulo is null)
                    return Resultado.Falha(ModuloResource.Erro_ModuloNaoEncontrado);

                perfilDeAcesso.PerfilDeAcessoModulos.Add(new PerfilDeAcessoModulo
                {
                    PerfilDeAcessoId = perfilDeAcesso.Id,
                    PerfilDeAcessoCodigo = perfilDeAcesso.Codigo,
                    ModuloId = modulo.Id,
                    ModuloCodigo = modulo.Codigo
                });
            }

            return Resultado.Sucesso();
        }
    }
}