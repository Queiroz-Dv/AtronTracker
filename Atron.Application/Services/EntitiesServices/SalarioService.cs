using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
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

namespace Atron.Application.Services.EntitiesServices
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
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(entidade.UsuarioCodigo);

            _validateModel.Validate(entidade);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                await AtualizarSalarioUsuario(entidade, usuario);
                var atualizado = await _salarioRepository.AtualizarSalarioRepositoryAsync(id, entidade);
                if (atualizado)
                {
                    _messageModel.AdicionarMensagem($"Salário do usuário {usuario.Nome} atualizado com sucesso");
                    return;
                }
            }
        }

        // Sempre que salário atual for maior do que o cadastrado em usuário ele será atualizado
        private async Task AtualizarSalarioUsuario(Salario entidade, Usuario usuario)
        {
            if (entidade.SalarioMensal > usuario.SalarioAtual || usuario.SalarioAtual == null)
            {
                await _usuarioRepository.AtualizarSalario(entidade.UsuarioId, entidade.SalarioMensal);
            }
        }

        public async Task CriarAsync(SalarioDTO salarioDTO)
        {
            var entidade = _map.MapToEntity(salarioDTO);
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(salarioDTO.UsuarioCodigo);

            _validateModel.Validate(entidade);
            if (!_messageModel.Notificacoes.HasErrors())
            {
                // Sempre que tivermos um salário informado maior do que o já cadastrado, atualizamos o salário do usuário
                // Para evitar redundância de dados
                var salarioInformado = await _salarioRepository.ObterSalarioPorCodigoUsuario(entidade.UsuarioCodigo);
                if (salarioInformado != null)
                {
                    await AtualizarSalarioUsuario(entidade, usuario);
                    var atualizado = await _salarioRepository.AtualizarSalarioRepositoryAsync(salarioInformado.Id, entidade);
                    if (atualizado)
                    {
                        _messageModel.AdicionarMensagem($"Salário do usuário {usuario.Nome} atualizado com sucesso");
                        return;
                    }
                }

                await AtualizarSalarioUsuario(entidade, usuario);
                var gravado = await _salarioRepository.CriarSalarioAsync(entidade);

                if (gravado)
                {
                    _messageModel.AdicionarMensagem($"Informação de salário criado para o usuário {usuario.Nome}");
                }
            }
        }

        public async Task ExcluirAsync(int id)
        {
            var removido = await _salarioRepository.RemoverInformacaoDeSalarioPorId(id);

            if (!removido)
            {
                _messageModel.AdicionarErro($"Não foi possível remover a informação de salário com o id {id}");
                return;
            }

            _messageModel.MensagemRegistroRemovido($"Informação de salário removida.");
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

        public async Task<string> ObterDescricaoDoMes(int mesId)
        {
          return await _salarioRepository.ObterDescricaoDoMes(mesId);            
        }
    }
}