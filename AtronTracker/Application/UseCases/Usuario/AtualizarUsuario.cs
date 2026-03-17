using Application.DTO.Request;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    public class AtualizarUsuario
    {
        private readonly IValidador<UsuarioRequest> _validador;
        private readonly IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> _mapService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository;
        private readonly IAuditoriaService _auditoriaService;

        private const string UsuarioContexto = "Usuario";

        public AtualizarUsuario(
            IValidador<UsuarioRequest> validador,
            IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> mapService,
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository,
            IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
            IAuditoriaService auditoriaService)
        {
            _validador = validador;
            _mapService = mapService;
            _usuarioRepository = usuarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _auditoriaService = auditoriaService;
        }

        public async Task<Resultado<UsuarioRequest>> ExecutarAsync(UsuarioRequest request)
        {
            var mensagens = _validador.Validar(request);
            if (mensagens.Any())
                return Resultado<UsuarioRequest>.Falhas(mensagens);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(request.Codigo);
            if (usuario is null)
                return Resultado<UsuarioRequest>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            await _mapService.MapToEntityAsync(request, usuario);

            var atualizado = await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            if (!atualizado)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoAtualizacao);

            if (!request.Senha.IsNullOrEmpty())
            {
                var identityAtualizado = await _usuarioIdentityRepository.AtualizarUserIdentityRepositoryAsync(usuario.Codigo, usuario.Email, request.Senha);

                if (!identityAtualizado)
                    return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoAtualizacao);
            }

            if (!request.DepartamentoCodigo.IsNullOrEmpty() && !request.CargoCodigo.IsNullOrEmpty())
            {
                var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(request.DepartamentoCodigo);
                var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(request.CargoCodigo);

                if (departamento != null && cargo != null)
                {
                    var relacionamento = new UsuarioCargoDepartamento
                    {
                        UsuarioId = usuario.Id,
                        UsuarioCodigo = usuario.Codigo,
                        CargoId = cargo.Id,
                        CargoCodigo = cargo.Codigo,
                        DepartamentoId = departamento.Id,
                        DepartamentoCodigo = departamento.Codigo
                    };

                    await _usuarioCargoDepartamentoRepository.AtualizarRepositoryAsync(relacionamento);
                }
            }

            await _auditoriaService.AtualizarServiceAsync(new AuditoriaDTO
            {
                CodigoRegistro = usuario.Codigo,
                Contexto = UsuarioContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = usuario.Codigo,
                    Contexto = UsuarioContexto,
                    Descricao = $"Usuário {usuario.Codigo} atualizado em {DateTime.Now:dd/MM/yyyy HH:mm}."
                }
            });

            return Resultado<UsuarioRequest>
                .Sucesso(request)
                .AdicionarMensagem("Usuário atualizado com sucesso.");
        }
    }
}