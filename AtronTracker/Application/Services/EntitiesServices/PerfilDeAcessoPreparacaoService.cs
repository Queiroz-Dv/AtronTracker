using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class PerfilDeAcessoPreparacaoService : IPerfilDeAcessoPreparacaoService
    {
        private readonly IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> _map;
        private readonly IModuloRepository _moduloRepository;
        private readonly IValidateModelService<PerfilDeAcesso> _validateModel;
        private readonly Notifiable _messageModel;

        public PerfilDeAcessoPreparacaoService(
            IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> map,
            IModuloRepository moduloRepository,
            IValidateModelService<PerfilDeAcesso> validateModel,
            Notifiable messageModel)
        {
            _map = map;
            _moduloRepository = moduloRepository;
            _validateModel = validateModel;
            _messageModel = messageModel;
        }

        public async Task<Resultado<PerfilDeAcesso>> PrepararAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            ValidarComando(perfilDeAcessoDTO);
            if (_messageModel.Notificacoes.HasErrors())
                return Resultado<PerfilDeAcesso>.Falhas(_messageModel.Notificacoes);

            var perfilDeAcesso = await _map.MapToEntityAsync(perfilDeAcessoDTO);
            await VincularModulosAsync(perfilDeAcessoDTO, perfilDeAcesso);

            _validateModel.Validate(perfilDeAcesso);

            return _messageModel.Notificacoes.HasErrors()
                ? Resultado<PerfilDeAcesso>.Falhas(_messageModel.Notificacoes)
                : Resultado<PerfilDeAcesso>.Sucesso(perfilDeAcesso);
        }

        private void ValidarComando(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            if (perfilDeAcessoDTO is null)
            {
                _messageModel.AdicionarErro(PerfilDeAcessoResource.Erro_PerfilInvalido);
                return;
            }

            if (perfilDeAcessoDTO.Modulos is null || !perfilDeAcessoDTO.Modulos.Any())
                _messageModel.AdicionarErro(PerfilDeAcessoResource.Erro_SemModulos);
        }

        private async Task VincularModulosAsync(PerfilDeAcessoDTO perfilDeAcessoDTO, PerfilDeAcesso perfilDeAcesso)
        {
            foreach (var moduloDTO in perfilDeAcessoDTO.Modulos)
            {
                var modulo = await _moduloRepository.ObterPorCodigoRepository(moduloDTO.Codigo);
                perfilDeAcesso.PerfilDeAcessoModulos.Add(new PerfilDeAcessoModulo
                {
                    PerfilDeAcessoId = perfilDeAcesso.Id,
                    PerfilDeAcessoCodigo = perfilDeAcesso.Codigo,
                    ModuloId = modulo.Id,
                    ModuloCodigo = modulo.Codigo
                });
            }
        }
    }
}
