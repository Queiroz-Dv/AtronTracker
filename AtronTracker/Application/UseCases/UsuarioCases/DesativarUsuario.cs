using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class DesativarUsuario(
        IUsuarioRepository usuarioRepository,
        IUsuarioIdentityRepository usuarioIdentityRepository,
        IAuditoriaService auditoriaService)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository = usuarioIdentityRepository;
        private readonly IAuditoriaService _auditoriaService = auditoriaService;

        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
            if (usuario is null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (usuario.Inativo)
                return Resultado.Falha(UsuarioResource.ErroUsuarioJaInativo);

            usuario.Inativo = true;
            usuario.CodigoReativacao = GerarCodigoReativacao();

            await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            await _usuarioIdentityRepository.DesativarContaAsync(usuario.Codigo);

            await _auditoriaService.AtualizarServiceAsync(new AuditoriaDTO
            {
                CodigoRegistro = usuario.Codigo,
                Contexto = nameof(Domain.Entities.Usuario),
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = usuario.Codigo,
                    Contexto = nameof(Domain.Entities.Usuario),
                    Descricao = $"Usuário {usuario.Codigo} desativado em {DateTime.Now:dd/MM/yyyy HH:mm}."
                }
            });

            return Resultado.Sucesso().AdicionarMensagem(UsuarioResource.MensagemUsuarioDesativado);
        }

        private static string GerarCodigoReativacao()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var codigo = new char[6];
            var bytes = RandomNumberGenerator.GetBytes(6);

            for (int i = 0; i < 6; i++)
                codigo[i] = caracteres[bytes[i] % caracteres.Length];

            return new string(codigo);
        }
    }
}
