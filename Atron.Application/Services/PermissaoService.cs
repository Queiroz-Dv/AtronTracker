using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class PermissaoService : IPermissaoService
    {
        private readonly IMapper _mapper;
        private readonly IPermissaoRepository _permissaoRepository;
        private readonly IPermissaoEstadoRepository _permissaoEstadoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;

        private readonly NotificationModel<Permissao> _notification;

        public PermissaoService(IMapper mapper,
                                IPermissaoRepository permissaoRepository,
                                IPermissaoEstadoRepository permissaoEstadoRepository,
                                IUsuarioRepository usuarioRepository,
                                IUsuarioService usuarioService,
                                NotificationModel<Permissao> notification)
        {
            _mapper = mapper;
            _permissaoRepository = permissaoRepository;
            _permissaoEstadoRepository = permissaoEstadoRepository;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _notification = notification;
            Messages = new List<NotificationMessage>();
        }

        public List<NotificationMessage> Messages { get; set; }

        public async Task CriarPermissaoServiceAsync(PermissaoDTO permissaoDTO)
        {
            var usuarioRepository = await _usuarioRepository.ObterUsuarioPorCodigoAsync(permissaoDTO.UsuarioCodigo);

            if (usuarioRepository is not null)
            {
                permissaoDTO.Usuario.Id = usuarioRepository.Id;
                permissaoDTO.Usuario.Codigo = usuarioRepository.Codigo;
            }

            var permissao = _mapper.Map<Permissao>(permissaoDTO);
            _notification.Validate(permissao);

            if (!_notification.Messages.HasErrors())
            {
                await _permissaoRepository.CriarPermissaoRepositoryAsync(permissao);
                Messages.Add(new NotificationMessage("Permissão foi adicionada com sucesso."));
            }

            Messages.AddRange(_notification.Messages);
        }

        public async Task<List<PermissaoDTO>> ObterTodasPermissoesServiceAsync()
        {
            var usuariosDTOService = await _usuarioService.ObterTodosAsync();
            var permissoesEstadoRepository = await _permissaoEstadoRepository.ObterTodosPermissaoEstadoRepositoryAsync();
            var permissoesRepository = await _permissaoRepository.ObterPermissoesRepositoryAsync();

            var permissoesDTO = _mapper.Map<IEnumerable<PermissaoDTO>>(permissoesRepository);

            var permissoes = permissoesDTO
                             .Join(usuariosDTOService,
                                   permissao => permissao.UsuarioId,
                                   usuario => usuario.Id,
                                   (permissao, usuario) => new { permissao, usuario})
                             
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