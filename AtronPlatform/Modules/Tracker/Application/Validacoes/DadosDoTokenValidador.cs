using Shared.Application.DTOS.Auth;
using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Extensions.RegraExtensions;

namespace Application.Validacoes
{
    public class DadosDoTokenValidador : Validador<DadosDoTokenDTO>
    {
        public DadosDoTokenValidador()
        {
            RegraPara(x => x.UsuarioCodigo)
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, AuthResource.Campo_CodigoUsuario));

            RegraPara(x => x.UsuarioCodigo)
                .TamanhoMenorOuIgualA(10)
                .ComMensagem(UsuarioResource.ErroCodigoLongo);

            RegraPara(x => x.UsuarioCodigo)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(UsuarioResource.ErroCodigoPequeno);
        }
    }
}
