using Application.DTO.Request;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Application.Interfaces.Services;
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
        private readonly ICacheUsuarioService _cacheUsuarioService;

        private const string UsuarioContexto = "Usuario";

        public AtualizarUsuario(
            IValidador<UsuarioRequest> validador,
            IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> mapService,
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository,
            IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
            IAuditoriaService auditoriaService,
            ICacheUsuarioService cacheUsuarioService)
        {
            _validador = validador;
            _mapService = mapService;
            _usuarioRepository = usuarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _auditoriaService = auditoriaService;
            _cacheUsuarioService = cacheUsuarioService;
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

            var resultadoGestor = await VincularGestorImediatoAsync(usuario, request.GestorImediatoCodigo);
            if (resultadoGestor.TeveFalha)
                return Resultado<UsuarioRequest>.Falhas(resultadoGestor.Messages);

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
                    var relacionamentoAtualizado = await AtualizarRelacionamentoCargoDepartamentoAsync(usuario, cargo, departamento);
                    if (!relacionamentoAtualizado)
                        return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoAtualizacao);
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

            _cacheUsuarioService.RemoverCacheDeAcessoTokenInfo(usuario.Codigo);

            return Resultado<UsuarioRequest>
                .Sucesso(request)
                .AdicionarMensagem("Usuário atualizado com sucesso.");
        }

        private async Task<bool> AtualizarRelacionamentoCargoDepartamentoAsync(
            Domain.Entities.Usuario usuario,
            Cargo cargo,
            Departamento departamento)
        {
            var relacionamentoExistente = await _usuarioCargoDepartamentoRepository
                .ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo);

            if (relacionamentoExistente == null)
            {
                return await _usuarioCargoDepartamentoRepository
                    .GravarAssociacaoUsuarioCargoDepartamento(usuario, cargo, departamento);
            }

            var relacionamentoJaAtualizado =
                relacionamentoExistente.CargoId == cargo.Id &&
                relacionamentoExistente.CargoCodigo == cargo.Codigo &&
                relacionamentoExistente.DepartamentoId == departamento.Id &&
                relacionamentoExistente.DepartamentoCodigo == departamento.Codigo;

            if (relacionamentoJaAtualizado)
                return true;

            var removido = await _usuarioCargoDepartamentoRepository.RemoverRepositoryAsync(relacionamentoExistente);
            if (!removido)
                return false;

            return await _usuarioCargoDepartamentoRepository
                .GravarAssociacaoUsuarioCargoDepartamento(usuario, cargo, departamento);
        }

        private async Task<Resultado> VincularGestorImediatoAsync(Domain.Entities.Usuario usuario, string gestorCodigo)
        {
            if (gestorCodigo.IsNullOrEmpty())
            {
                usuario.GestorImediatoId = null;
                usuario.GestorImediatoCodigo = null;
                return Resultado.Sucesso();
            }

            var codigoGestor = gestorCodigo.ToUpper();
            if (codigoGestor == usuario.Codigo)
                return Resultado.Falha("O usuario nao pode ser gestor imediato dele mesmo.");

            var gestor = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoGestor);
            if (gestor is null)
                return Resultado.Falha("Gestor imediato nao encontrado.");

            usuario.GestorImediatoId = gestor.Id;
            usuario.GestorImediatoCodigo = gestor.Codigo;

            return Resultado.Sucesso();
        }
    }
}
