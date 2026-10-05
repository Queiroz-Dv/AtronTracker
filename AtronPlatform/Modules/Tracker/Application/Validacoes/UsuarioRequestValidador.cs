using Application.DTO.Request;
using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Extensions.RegraExtensions;

namespace Application.Validacoes
{
    public class UsuarioRequestValidador : Validador<UsuarioRequest>
    {
        public UsuarioRequestValidador()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroCodigoNulo);
            RegraPara(x => x.Codigo)
                .TamanhoMenorOuIgualA(10)
                .ComMensagem(UsuarioResource.ErroCodigoLongo);
            RegraPara(x => x.Codigo)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(UsuarioResource.ErroCodigoPequeno);

            RegraPara(x => x.Nome)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroNomeUsuarioNulo);
            RegraPara(x => x.Nome)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(UsuarioResource.ErroNomePequeno);
            RegraPara(x => x.Nome)
                .TamanhoMenorOuIgualA(25)
                .ComMensagem(UsuarioResource.ErroNomeLongo);

            RegraPara(x => x.Sobrenome)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroSobrenomeObrigatorio);
            RegraPara(x => x.Sobrenome)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(UsuarioResource.ErroSobrenomePequeno);
            RegraPara(x => x.Sobrenome)
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(UsuarioResource.ErroSobrenomeLongo);

            RegraPara(x => x.Email)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroEmailNulo);
        }
    }
}
