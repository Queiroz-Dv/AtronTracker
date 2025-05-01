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
    public class PerfilDeAcessoService : IPerfilDeAcessoService
    {
        private readonly IApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> _map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoUsuarioRepository _perfilDeAcessoUsuarioRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IModuloRepository _moduloRepository;
        private readonly IValidateModel<PerfilDeAcesso> _validateModel;
        private readonly MessageModel _messageModel;

        public PerfilDeAcessoService(
            IPerfilDeAcessoUsuarioRepository perfilDeAcessoUsuarioRepository,
            IUsuarioRepository usuarioRepository,
            IApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> map,
            IPerfilDeAcessoRepository perfilDeAcessoRepository,
            IPropriedadeDeFluxoRepository propriedadeDeFluxoRepository,
            IPropriedadeDeFluxoModuloRepository propriedadeDeFluxoModuloRepository,
            IModuloRepository moduloRepository,
            IValidateModel<PerfilDeAcesso> validateModel,
            MessageModel messageModel)
        {
            _map = map;
            _usuarioRepository = usuarioRepository;
            _perfilDeAcessoUsuarioRepository = perfilDeAcessoUsuarioRepository;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
            _moduloRepository = moduloRepository;
            _validateModel = validateModel;
            _messageModel = messageModel;
        }

        private void ChecarPerfilModulo(PerfilDeAcessoDTO perfilDeAcessoDTO)
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
            ChecarPerfilModulo(perfilDeAcessoDTO);

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
            ChecarPerfilModulo(perfilDeAcessoDTO);

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
                var result = await _perfilDeAcessoRepository.DeletarPerfilRepositoryAsync(perfil);
                _messageModel.AddMessage("Perfil removido com sucesso");

                return result;
            }

            return false;
        }

        public async Task<PerfilDeAcessoDTO> ObterPerfilPorCodigoServiceAsync(string codigo)
        {
            var entidade = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            return entidade is null ? null : _map.MapToDTO(entidade);
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

        public async Task<bool> RelacionarPerfilDeAcessoUsuarioServiceAsync(PerfilDeAcessoUsuarioDTO dto)
        {
            ChecarPerfilDeAcessoUsuario(dto);

            if (!_messageModel.Messages.HasErrors())
            {
                // Preciso remover antes pois não terei o Update diretamente, basta remover os registros e refazer a gravação
                var perfilRelacionado = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(dto.PerfilDeAcesso.Codigo);

                if (perfilRelacionado != null)
                {
                    if (perfilRelacionado.PerfisDeAcessoUsuario.Any())
                    {
                        foreach (var item in perfilRelacionado.PerfisDeAcessoUsuario)
                        {
                            await _perfilDeAcessoUsuarioRepository.DeletarRelacionamento(item);
                        }
                    }
                }

                var perfilDeAcesso = _map.MapToEntity(dto.PerfilDeAcesso);
                perfilDeAcesso.PerfisDeAcessoUsuario = new List<PerfilDeAcessoUsuario>();
                foreach (var usuarioDTO in dto.Usuarios)
                {
                    var perfilDeAcessoUsuario = new PerfilDeAcessoUsuario();

                    var usuarioRepo = await _usuarioRepository.ObterPorCodigoRepositoryAsync(usuarioDTO.Codigo);
                    perfilDeAcessoUsuario.UsuarioId = usuarioRepo.Id;
                    perfilDeAcessoUsuario.UsuarioCodigo = usuarioRepo.Codigo;

                    var perfilRepo = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(perfilDeAcesso.Codigo);
                    perfilDeAcessoUsuario.PerfilDeAcessoId = perfilRepo.Id;
                    perfilDeAcessoUsuario.PerfilDeAcessoCodigo = perfilRepo.Codigo;

                    perfilDeAcesso.PerfisDeAcessoUsuario.Add(perfilDeAcessoUsuario);
                }


                // Só pra testar a gravação a partir desse ponto
                int salvos = 0;
                foreach (var perfil in perfilDeAcesso.PerfisDeAcessoUsuario)
                {
                    var result = await _perfilDeAcessoUsuarioRepository.CriarPerfilRepositoryAsync(perfil);
                    if (result)
                    {
                        salvos++;
                    }
                }

                return salvos > 0;
            }

            return false;
        }

        private void ChecarPerfilDeAcessoUsuario(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            if (perfilDeAcessoUsuario.PerfilDeAcesso is null)
            {
                _messageModel.AddError("O perfil de acesso está inválido para gravação.");
            }

            if (!perfilDeAcessoUsuario.Usuarios.Any())
            {
                _messageModel.AddError("Não contém nenhum usuário para relacionar ao perfil criado.");
            }
        }

        public async Task<PerfilDeAcessoUsuarioDTO> ObterRelacionamentoDePerfilUsuarioPorCodigoServiceAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
            {
                return await Task.FromResult(new PerfilDeAcessoUsuarioDTO());
            }

            var perfilDeAcesso = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);

            var perfilDeAcessoDTO = _map.MapToDTO(perfilDeAcesso);


            var dto = new PerfilDeAcessoUsuarioDTO();

            dto.PerfilDeAcesso.Codigo = perfilDeAcessoDTO.Codigo;
            dto.PerfilDeAcesso.Descricao = perfilDeAcessoDTO.Descricao;

            foreach (var relacionamento in perfilDeAcesso.PerfisDeAcessoUsuario)
            {
                var usuarioDto = new UsuarioDTO
                {
                    Codigo = relacionamento.Usuario.Codigo,
                    Nome = relacionamento.Usuario.Nome,
                    Sobrenome = relacionamento.Usuario.Sobrenome
                };

                dto.Usuarios.Add(usuarioDto);
            }

            foreach (var modulo in perfilDeAcessoDTO.Modulos)
            {
                dto.PerfilDeAcesso.Modulos.Add(modulo);
            }

            return dto;

        }

        public async Task<List<PerfilDeAcessoDTO>> ObterPerfisPorCodigoUsuarioServiceAsync(string usuarioCodigo)
        {
            var perfis = await _perfilDeAcessoRepository.ObterPerfisPorCodigoDeUsuarioRepositoryAsync(usuarioCodigo);

            return perfis != null ? _map.MapToListDTO(perfis) : null;
        }
    }
}
