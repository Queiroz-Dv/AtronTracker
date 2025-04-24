using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Componentes;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Extensions;
using Shared.Interfaces.Mapper;
using Shared.Interfaces.Validations;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class PerfilDeAcessoService : IPerfilDeAcessoService
    {
        private readonly IApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> _map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;
        private readonly IPropriedadeDeFluxoRepository _propriedadeDeFluxoRepository;
        private readonly IPropriedadeDeFluxoModuloRepository _propriedadeDeFluxoModuloRepository;
        private readonly IModuloRepository _moduloRepository;
        private readonly IValidateModel<PerfilDeAcesso> _validateModel;
        private readonly MessageModel _messageModel;

        public PerfilDeAcessoService(
            IApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> map,
            IPerfilDeAcessoRepository perfilDeAcessoRepository,
            IPropriedadeDeFluxoRepository propriedadeDeFluxoRepository,
            IPropriedadeDeFluxoModuloRepository propriedadeDeFluxoModuloRepository,
            IModuloRepository moduloRepository,
            IValidateModel<PerfilDeAcesso> validateModel,
            MessageModel messageModel)
        {
            _map = map;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
            _propriedadeDeFluxoRepository = propriedadeDeFluxoRepository;
            _moduloRepository = moduloRepository;
            _propriedadeDeFluxoModuloRepository = propriedadeDeFluxoModuloRepository;
            _validateModel = validateModel;
            _messageModel = messageModel;
        }

        private void ChecarPerfil(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            if (perfilDeAcessoDTO is null)
            {
                _messageModel.AddError("O perfil de acesso está inválido para gravação.");
            }

            if (!perfilDeAcessoDTO.Modulos.Any())
            {
                _messageModel.AddError("Não contém nenhum módulo para relacionar ao perfil criado.");
            }
        }


        public async Task<bool> AtualizarPerfilServiceAsync(string codigo, PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            // TODO: Utilizar o speficiation no futuro pra verificar se os códigos está válidos
            ChecarPerfil(perfilDeAcessoDTO);

            if (!_messageModel.Messages.HasErrors())
            {
                var perfilDeAcesso = _map.MapToEntity(perfilDeAcessoDTO);

                await PreencherInformacoesDaEntidade(perfilDeAcessoDTO, perfilDeAcesso);

                // Lembrar de validar o módulo de acordo com as regras de negócio
                _validateModel.Validate(perfilDeAcesso);

                if (!_messageModel.Messages.HasErrors())
                {
                    var prf = await _perfilDeAcessoRepository.AtualizarPerfilRepositoryAsync(codigo, perfilDeAcesso);
                    if (prf)
                    {
                        _messageModel.AddMessage($"Perfil de acesso {perfilDeAcesso.Codigo} atualizado com sucesso.");

                        return prf;
                    }
                }

                return false;
            }

            return false;
        }

        private async Task PreencherInformacoesDaEntidade(PerfilDeAcessoDTO perfilDeAcessoDTO, PerfilDeAcesso perfilDeAcesso)
        {
            // Aqui preciso obter o id de cada módulo para relacionar ao perfil e as propriedades
            foreach (var moduloDTO in perfilDeAcessoDTO.Modulos)
            {
                // Cria o objeto global do foreach
                var perfilDeAcessoModulo = new PerfilDeAcessoModulo();

                // Obtém o módulo por código para preencher o id
                var modulo = await _moduloRepository.ObterPorCodigoRepository(moduloDTO.Codigo);

                // Preenche a entidade principal
                perfilDeAcessoModulo.PerfilDeAcessoId = perfilDeAcesso.Id;
                perfilDeAcessoModulo.PerfilDeAcessoCodigo = perfilDeAcesso.Codigo;
                perfilDeAcessoModulo.ModuloId = modulo.Id;
                perfilDeAcessoModulo.ModuloCodigo = modulo.Codigo;

                // Inclui a entidade totalmente preenchida na lista
                perfilDeAcesso.PerfilDeAcessoModulos.Add(perfilDeAcessoModulo);
            }
        }

        // Para criar um perfil eu preciso pelo menos ter um módulo relacionado a ele
        public async Task<bool> CriarPerfilServiceAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            ChecarPerfil(perfilDeAcessoDTO);

            if (!_messageModel.Messages.HasErrors())
            {
                var perfilDeAcesso = _map.MapToEntity(perfilDeAcessoDTO);


                await PreencherInformacoesDaEntidade(perfilDeAcessoDTO, perfilDeAcesso);

                // Lembrar de validar o módulo de acordo com as regras de negócio
                _validateModel.Validate(perfilDeAcesso);

                if (!_messageModel.Messages.HasErrors())
                {
                    var prf = await _perfilDeAcessoRepository.CriarPerfilRepositoryAsync(perfilDeAcesso);
                    if (prf)
                    {                     
                        _messageModel.AddMessage($"Perfil de acesso {perfilDeAcesso.Codigo} criado com sucesso.");

                        return prf;
                    }
                }

                return false;
            }

            return false;
        }

        public async Task<bool> DeletarPerfilServiceAsync(string codigo)
        {
            var perfil = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            if (perfil is null)
            {
                _messageModel.AddRegisterNotFoundMessage("Perfil de acesso");
            }
            else
            {
                var result  = await _perfilDeAcessoRepository.DeletarPerfilRepositoryAsync(perfil);
                _messageModel.AddMessage("Perfil removido com sucesso");

                return result;
            }

            return false;
        }

        public async Task<PerfilDeAcessoDTO> ObterPerfilPorCodigoServiceAsync(string codigo)
        {
            var entidade = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            return entidade is null ? null :  _map.MapToDTO(entidade);

        }

        public Task<PerfilDeAcessoDTO> ObterPerfilPorIdServiceAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<PerfilDeAcessoDTO>> ObterTodosPerfisServiceAsync()
        {
            var entities = await _perfilDeAcessoRepository.ObterTodosPerfisRepositoryAsync();
            return _map.MapToListDTO(entities.ToList());
        }
    }
}
