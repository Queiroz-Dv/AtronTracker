using Atron.Application.DTO;
using Atron.Application.Interfaces;
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
        private readonly IModuloRepository _moduloRepository;
        private readonly IValidateModel<PerfilDeAcesso> _validateModel;
        private readonly MessageModel _messageModel;

        public PerfilDeAcessoService(
            IApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> map,
            IPerfilDeAcessoRepository perfilDeAcessoRepository,
            IModuloRepository moduloRepository,
            IValidateModel<PerfilDeAcesso> validateModel,
            MessageModel messageModel)
        {
            _map = map;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
            _validateModel = validateModel;
            _messageModel = messageModel;
            _moduloRepository = moduloRepository;
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


        public Task<bool> AtualizarPerfilServiceAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            throw new NotImplementedException();
        }

        // Para criar um perfil eu preciso pelo menos ter um módulo relacionado a ele
        public async Task<bool> CriarPerfilServiceAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            ChecarPerfil(perfilDeAcessoDTO);

            if (!_messageModel.Messages.HasErrors())
            {
                var perfilDeAcesso = _map.MapToEntity(perfilDeAcessoDTO);

                // Aqui preciso obter o id de cada módulo para relacionar ao perfil
                foreach (var moduloDTO in perfilDeAcessoDTO.Modulos)
                {
                    var modulo = await _moduloRepository.ObterPorCodigoRepository(moduloDTO.Codigo);

                    perfilDeAcesso.PerfilDeAcessoModulos.Add(new PerfilDeAcessoModulo()
                    {
                        PerfilDeAcessoId = perfilDeAcesso.Id,
                        PerfilDeAcessoCodigo = perfilDeAcesso.Codigo,                      
                        ModuloId = modulo.Id,
                        ModuloCodigo = modulo.Codigo
                    });
                }

<<<<<<< HEAD
             
=======
              

>>>>>>> 9c5d71f27b0cbf2d0952b4c244329a3ccae73954
                // Lembrar de validar o módulo de acordo com as regras de negócio
                _validateModel.Validate(perfilDeAcesso);

                if (!_messageModel.Messages.HasErrors())
                {
                    var prf = await _perfilDeAcessoRepository.CriarPerfilRepositoryAsync(perfilDeAcesso);
                    if (prf)
                    {
                        _messageModel.AddMessage($"Perfil de acesso {perfilDeAcesso.Codigo} criado com sucesso.");
                    }
                }

                return false;
            }

            return false;
        }

        public Task<bool> DeletarPerfilServiceAsync(string codigo)
        {
            throw new NotImplementedException();
        }

        public Task<PerfilDeAcessoDTO> ObterPerfilPorCodigoServiceAsync(string codigo)
        {
            throw new NotImplementedException();
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
