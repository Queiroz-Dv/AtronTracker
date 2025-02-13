using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
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
    public class SalarioService : ISalarioService
    {
        private readonly IApplicationMapService<SalarioDTO, Salario> _map;
        private readonly ISalarioRepository _salarioRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IValidateModel<Salario> _validateModel;
        private readonly MessageModel _messageModel;

        public SalarioService(IApplicationMapService<SalarioDTO, Salario> map,
                              ISalarioRepository salarioRepository,
                              IUsuarioRepository usuarioRepository,
                              IValidateModel<Salario> validateModel,
                              MessageModel messageModel)

        {
            _map = map;
            _usuarioRepository = usuarioRepository;
            _salarioRepository = salarioRepository;
            _messageModel = messageModel;
            _validateModel = validateModel;
        }

        public async Task AtualizarServiceAsync(int id, SalarioDTO salarioDTO)
        {
            var entidade = _map.MapToEntity(salarioDTO);
            var usuario = await _usuarioRepository.ObterPorCodigoRepositoryAsync(entidade.UsuarioCodigo);

            _validateModel.Validate(entidade);

            if (!_messageModel.Messages.HasErrors())
            {
                await AtualizarSalarioUsuario(entidade, usuario);
                await _salarioRepository.AtualizarSalarioRepositoryAsync(id, entidade);
                _messageModel.AddUpdateMessage(nameof(Salario));
                return;
            }
        }

        // Sempre que salário atual for maior do que o cadastrado em usuário ele será atualizado
        private async Task AtualizarSalarioUsuario(Salario entidade, Usuario usuario)
        {
            if (entidade.SalarioMensal > usuario.SalarioAtual)
            {
                await _usuarioRepository.AtualizarSalario(entidade.UsuarioId, entidade.SalarioMensal);
            }
        }

        public async Task CriarAsync(SalarioDTO salarioDTO)
        {
            var entidade = _map.MapToEntity(salarioDTO);
            var usuario = await _usuarioRepository.ObterPorCodigoRepositoryAsync(salarioDTO.UsuarioCodigo);

            _validateModel.Validate(entidade);

            if (!_messageModel.Messages.HasErrors())
            {
                await AtualizarSalarioUsuario(entidade, usuario);
                await _salarioRepository.CriarSalarioAsync(entidade);
                _messageModel.AddSuccessMessage(nameof(Salario));
            }
        }

        public async Task ExcluirAsync(string id)
        {
            var salario = await _salarioRepository.ObterPorIdRepositoryAsync(Convert.ToInt32(id));

            if (salario is not null)
            {
                await _salarioRepository.RemoverRepositoryAsync(salario);
                _messageModel.AddRegisterRemovedSuccessMessage(nameof(Salario));
            }
        }        

        public async Task<SalarioDTO> ObterPorId(int id)
        {
            var entidade = await _salarioRepository.ObterSalarioPorIdAsync(id);
            return _map.MapToDTO(entidade);
        }

        public async Task<List<SalarioDTO>> ObterTodosAsync()
        {
            var salariosRepository = await _salarioRepository.ObterSalariosRepository();
            return _map.MapToListDTO(salariosRepository.ToList());
        }
    }
}