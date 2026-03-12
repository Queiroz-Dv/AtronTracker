using Application.DTO.Request;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    /// <summary>
    /// Caso de uso responsável pela atualização de um usuário existente.
    /// </summary>
    public class AtualizarUsuario
    {
        private readonly IValidador<UsuarioRequest> _validador;
        private readonly IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> _mapService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository;

        public AtualizarUsuario(
            IValidador<UsuarioRequest> validador,
            IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> mapService,
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository,
            IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository)
        {
            _validador = validador;
            _mapService = mapService;
            _usuarioRepository = usuarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
        }

        public async Task<Resultado<UsuarioRequest>> ExecutarAsync(UsuarioRequest request)
        {
            // 1. Validação
            var mensagens = _validador.Validar(request);
            if (mensagens.Any())
                return Resultado<UsuarioRequest>.Falhas(mensagens);

            // 2. Verificação de existência
            var usuario = await _usuarioRepository
                .ObterUsuarioPorCodigoAsync(request.Codigo);

            if (usuario == null)
                return Resultado<UsuarioRequest>
                    .Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            // 3. Atualização do usuário de negócio
            await _mapService.MapToEntityAsync(request, usuario);

            var atualizado = await _usuarioRepository
                .AtualizarUsuarioAsync(usuario);

            if (!atualizado)
                return Resultado<UsuarioRequest>
                    .Falha(UsuarioResource.ErroInesperadoAtualizacao);

            // 4. Atualização da conta Identity (se aplicável)
            if (!request.Senha.IsNullOrEmpty())
            {
                var identityAtualizado = await _usuarioIdentityRepository
                    .AtualizarUserIdentityRepositoryAsync(
                        usuario.Codigo,
                        usuario.Email,
                        request.Senha);

                if (!identityAtualizado)
                    return Resultado<UsuarioRequest>
                        .Falha(UsuarioResource.ErroInesperadoAtualizacao);
            }

            // 5. Atualização da associação Cargo / Departamento (se aplicável)
            //    Decisão: Cargo/Departamento serão substituídos por RBAC em versão futura.
            if (!request.DepartamentoCodigo.IsNullOrEmpty() &&
                !request.CargoCodigo.IsNullOrEmpty())
            {
                var departamento = await _departamentoRepository
                    .ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(
                        request.DepartamentoCodigo);

                var cargo = await _cargoRepository
                    .ObterCargoPorCodigoAsync(request.CargoCodigo);

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

                    await _usuarioCargoDepartamentoRepository
                        .AtualizarRepositoryAsync(relacionamento);
                }
            }

            // 6. Retorno padronizado
            return Resultado<UsuarioRequest>
                .Sucesso(request)
                .AdicionarMensagem("Usuário atualizado com sucesso.");
        }
    }
}