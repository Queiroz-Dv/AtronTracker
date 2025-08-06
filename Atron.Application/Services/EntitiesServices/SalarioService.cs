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
        private readonly IAsyncApplicationMapService<SalarioDTO, Salario> _map;
        private readonly ISalarioRepository _salarioRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IValidateModel<Salario> _validateModel;
        private readonly MessageModel _messageModel;

        public SalarioService(IAsyncApplicationMapService<SalarioDTO, Salario> map,
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
            var entidade = await _map.MapToEntityAsync(salarioDTO);
            var usuarioIdentity = await _usuarioRepository.ObterUsuarioPorCodigoAsync(entidade.UsuarioCodigo);

            _validateModel.Validate(entidade);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                var usuario = new Usuario()
                {
                    Codigo = usuarioIdentity.Codigo,
                    Nome = usuarioIdentity.Nome,
                    Sobrenome = usuarioIdentity.Sobrenome,
                    DataNascimento = usuarioIdentity.DataNascimento,
                    Email = usuarioIdentity.Email,
                    Salario = usuarioIdentity.Salario,
                };

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
            if (entidade.SalarioMensal > usuario.SalarioAtual || usuario.Salario == null)
            {
                await _usuarioRepository.AtualizarSalario(entidade.UsuarioId, entidade.SalarioMensal);
            }
        }

        public async Task CriarAsync(SalarioDTO salarioDTO)
        {
            var entidade = await _map.MapToEntityAsync(salarioDTO);
            var usuarioIdentity = await _usuarioRepository.ObterUsuarioPorCodigoAsync(salarioDTO.UsuarioCodigo);
            entidade.UsuarioId = usuarioIdentity.Id;

            _validateModel.Validate(entidade);
            if (!_messageModel.Notificacoes.HasErrors())
            {
                var usuario = new Usuario()
                {
                    Codigo = usuarioIdentity.Codigo,
                    Nome = usuarioIdentity.Nome,
                    Sobrenome = usuarioIdentity.Sobrenome,
                    DataNascimento = usuarioIdentity.DataNascimento,
                    Email = usuarioIdentity.Email,
                    Salario = usuarioIdentity.Salario,
                };

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
            var salario = await _salarioRepository.ObterPorIdRepositoryAsync(Convert.ToInt32(id));

            if (salario is not null)
            {
                await _salarioRepository.RemoverRepositoryAsync(salario);
                _messageModel.MensagemRegistroRemovido();
            }
        }

        public async Task<SalarioDTO> ObterPorId(int id)
        {
            var entidade = await _salarioRepository.ObterSalarioPorIdAsync(id);
            return await _map.MapToDTOAsync(entidade);
        }

        public async Task<List<SalarioDTO>> ObterTodosAsync()
        {
            var salariosRepository = await _salarioRepository.ObterSalariosRepository();
            return await _map.MapToListDTOAsync(salariosRepository.ToList());
        }       
    }
}