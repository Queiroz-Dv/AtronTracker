using AtronStock.Application.DTO.Request; 
using Shared.Application.Services;
using AtronStock.Application.Resources;
using Shared.Extensions.RegraExtensions;
using Shared.Extensions;

namespace AtronStock.Application.Validacoes
{
    public class CategoriaValidador : Validador<CategoriaRequest>
    {
        public CategoriaValidador()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(CategoriaResource.ErroCodigoObrigatorio);

            RegraPara(x => x.Codigo)
                .TamanhoMenorOuIgualA(25)
                .ComMensagem(CategoriaResource.ErroCodigoTamanho);

            RegraPara(x => x.Descricao)
                .NaoVazio()
                .ComMensagem(CategoriaResource.ErroDescricaoObrigatoria);

            RegraPara(x => x.Descricao)
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(CategoriaResource.ErroDescricaoTamanho);

            RegraPara(x => x.Status)
                .DeveSer(status => !status.GetDescription().IsNullOrEmpty())
                .ComMensagem(CategoriaResource.ErroStatusObrigatorio);
        }
    }
}
