using Atron.Application.DTO;
using Atron.Application.Enums;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class PermissaoService : IPermissaoService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Permissao> _repository;
        private readonly IPermissaoRepository _permissaoRepository;
        private readonly IPermissaoEstadoRepository _permissaoEstadoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;        

        public PermissaoService(IMapper mapper,
                                IRepository<Permissao> repository,
                                IPermissaoRepository permissaoRepository,
                                IPermissaoEstadoRepository permissaoEstadoRepository,
                                IUsuarioRepository usuarioRepository,
                                IUsuarioService usuarioService)
        {
            _mapper = mapper;
            _repository = repository;
            _permissaoRepository = permissaoRepository;
            _permissaoEstadoRepository = permissaoEstadoRepository;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            Messages = new List<NotificationMessage>();
        }

        public List<NotificationMessage> Messages { get; set; }

        public async Task AtualizarPermissaoServiceAsync(PermissaoDTO permissaoDTO)
        {
            var permissao = _mapper.Map<Permissao>(permissaoDTO);

            var usuarioExiste = _usuarioRepository.UsuarioExiste(permissao.UsuarioCodigo);
            if (usuarioExiste)
            {
                var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(permissao.UsuarioCodigo);
                var permissaoEstado = await _permissaoEstadoRepository.ObterTodosPermissaoEstadoRepositoryAsync();

                permissao.UsuarioId = usuario.Id;
                permissao.UsuarioCodigo = usuario.Codigo;
                permissao.PermissaoEstadoId = permissaoEstado.FirstOrDefault(pme => pme.Id == permissao.PermissaoEstadoId).Id;
            }
            else
            {
                Messages.Add(new NotificationMessage("Código de usuário não existe para atualização da permissão. Tente novamente.", Notification.Enums.ENotificationType.Error));
                return;
            }

            //_notification.Validate(permissao);

            //if (!_notification.Messages.HasErrors())
            //{
            //    await _permissaoRepository.AtualizarRepositoryAsync(permissao);
            //    Messages.Add(new NotificationMessage("Permissão atualizada com sucesso"));
            //}

            //Messages.AddRange(_notification.Messages);
        }

        public async Task CriarPermissaoServiceAsync(PermissaoDTO permissaoDTO)
        {
            var usuarioRepository = await _usuarioRepository.ObterUsuarioPorCodigoAsync(permissaoDTO.UsuarioCodigo);

            if (usuarioRepository is not null)
            {
                permissaoDTO.Usuario.Id = usuarioRepository.Id;
                permissaoDTO.Usuario.Codigo = usuarioRepository.Codigo;
            }

            var permissao = _mapper.Map<Permissao>(permissaoDTO);
            //_notification.Validate(permissao);

            //if (!_notification.Messages.HasErrors())
            //{
            //    await _permissaoRepository.CriarRepositoryAsync(permissao);
            //    Messages.Add(new NotificationMessage("Permissão foi adicionada com sucesso."));
            //}

            //Messages.AddRange(_notification.Messages);
        }

        public async Task ExcluirPermissaoServiceAsync(int id)
        {
            var permissao = await _repository.ObterPorIdRepositoryAsync(id);

            if (permissao is null)
            {
                Messages.Add(new NotificationMessage("Permissão não existe ou não foi associada a um usuário"));
                return;
            }

            if (permissao.PermissaoEstadoId is (int)EPermissaoEstados.Aprovada or
                (int)EPermissaoEstados.Desaprovada)
            {
                Messages.Add(new NotificationMessage("Não é possível excluir uma permissão aprovada ou desaprovada"));
                return;
            }


            await _repository.RemoverRepositoryAsync(permissao);
            Messages.Add(new NotificationMessage("Registro removido com sucesso"));
        }

        public async Task<List<PermissaoDTO>> ObterTodasPermissoesServiceAsync()
        {
            var usuariosDTOService = await _usuarioService.ObterTodosAsync();
            var permissoesEstadoRepository = await _permissaoEstadoRepository.ObterTodosPermissaoEstadoRepositoryAsync();
            var permissoesRepository = await _permissaoRepository.ObterTodosRepositoryAsync();

            var permissoesDTO = _mapper.Map<IEnumerable<PermissaoDTO>>(permissoesRepository);

            var permissoes = permissoesDTO
                             .Join(usuariosDTOService,
                                   permissao => permissao.UsuarioId,
                                   usuario => usuario.Id,
                                   (permissao, usuario) => new { permissao, usuario })

                             .Join(permissoesEstadoRepository,
                                   pms => pms.permissao.PermissaoEstadoId,
                                   pme => pme.Id,

                                   (pms, pme) => new PermissaoDTO()
                                   {
                                       Id = pms.permissao.Id,
                                       UsuarioId = pms.permissao.UsuarioId,
                                       UsuarioCodigo = pms.permissao.UsuarioCodigo,

                                       Usuario = new UsuarioDTO()
                                       {
                                           Codigo = pms.usuario.Codigo,
                                           Nome = pms.usuario.Nome,
                                           Sobrenome = pms.usuario.Sobrenome,
                                           DataNascimento = pms.usuario.DataNascimento,
                                           Salario = pms.usuario.Salario,

                                           Departamento = new DepartamentoDTO()
                                           {
                                               Codigo = pms.usuario.Departamento.Codigo,
                                               Descricao = pms.usuario.Departamento.Descricao
                                           },

                                           Cargo = new CargoDTO()
                                           {
                                               Codigo = pms.usuario.Cargo.Codigo,
                                               Descricao = pms.usuario.Cargo.Descricao,
                                               DepartamentoCodigo = pms.usuario.Cargo.DepartamentoCodigo,
                                               DepartamentoDescricao = pms.usuario.Cargo.DepartamentoDescricao,
                                           }
                                       },

                                       Descricao = pms.permissao.Descricao,
                                       DataInicial = pms.permissao.DataInicial,
                                       DataFinal = pms.permissao.DataFinal,
                                       PermissaoEstadoId = pme.Id,
                                       PermissaoEstadoDescricao = pme.Descricao,
                                       QuantidadeDeDias = pms.permissao.QuantidadeDeDias,
                                   }
                             ).OrderByDescending(pms => pms.DataInicial).ToList();

            return permissoes;
        }
    }
}