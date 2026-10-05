using Application.DTO;
using Application.Interfaces.ApplicationInterfaces;
using Application.UseCases.PerfilDeAcessoCases;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.UsuarioCases
{
    public sealed class CriarRelacaoPerfilModuloUsuarioCase(
        ILoginService loginService,
        CriarPerfilDeAcessoCase criarPerfilDeAcessoCase,
        RelacionarPerfilUsuarioCase relacionamentoService,
        ObterUsuarioCase usuarioService)
    {
        public async Task<Resultado> ExecutarAsync(PerfilDeAcessoDTO perfil, string usuarioCodigo)
        {
            var usuarioResultado = await usuarioService.ExecutarAsync(usuarioCodigo);

            if (usuarioResultado.TeveFalha)
                return Resultado.Falha(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados;
            var relacionamentoPerfil = new PerfilDeAcessoUsuarioDTO()
            {
                PerfilDeAcesso = perfil,
                Usuarios = new List<UsuarioDTO>() { new UsuarioDTO() { Codigo = usuario.Codigo } }
            };

            var perfilCriado = await criarPerfilDeAcessoCase.ExecutarAsync(perfil);

            if (perfilCriado.TeveFalha)
                return Resultado.Falha(perfilCriado.Messages);

            var relacionamentoPerfilResultado = await relacionamentoService.ExecutarAsync(relacionamentoPerfil);

            if (relacionamentoPerfilResultado.TeveFalha)
                return Resultado.Falha(relacionamentoPerfilResultado.Messages);

            await loginService.Logout(usuario.Codigo);

            return Resultado.Sucesso("Tudo pronto para o seu acesso");
        }
    }
}