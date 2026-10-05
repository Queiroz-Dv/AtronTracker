#nullable enable

using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using Shared.Application.Services;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using Shared.Extensions.RegraExtensions;

namespace AtronStock.Application.Validacoes
{
    public sealed class ProdutoRequestValidador : Validador<ProdutoRequest>
    {
        public ProdutoRequestValidador()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(ProdutoResource.ErroCodigoObrigatorio);
            RegraPara(x => x.Codigo)
                .TamanhoMenorOuIgualA(25)
                .ComMensagem(ProdutoResource.ErroCodigoLimiteMaximoDeCaractere);
            RegraPara(x => x.Codigo)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(ProdutoResource.ErroCodigoLimiteMinimoDeCaractere);

            RegraPara(x => x.Descricao)
                .NaoVazio()
                .ComMensagem(ProdutoResource.ErroDescricaoObrigatoria);
            RegraPara(x => x.Descricao)
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(ProdutoResource.ErroDescricaoLimiteMaximoCaractere);
            RegraPara(x => x.Descricao)
                .TamanhoMaiorOuIgualA(5)
                .ComMensagem(ProdutoResource.ErroDescricaoLimiteMinimoCaractere);

            RegraPara(x => x.DataAquisicao)
                .DeveSer(d => d != default)
                .ComMensagem(ProdutoResource.ErroDataAquisicaoObrigatoria);

            RegraPara(x => x.PrecoUnitario)
                .DeveSer(p => p > 0)
                .ComMensagem(ProdutoResource.ErroPrecoProduto);
        }
    }

    public sealed class ProdutoAtualizacaoRequestValidador : Validador<ProdutoAtualizacaoRequest>
    {
        public ProdutoAtualizacaoRequestValidador()
        {
            RegraPara(x => x.Descricao)
                .NaoVazio()
                .ComMensagem(ProdutoResource.ErroDescricaoObrigatoria);
            RegraPara(x => x.Descricao)
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(ProdutoResource.ErroDescricaoLimiteMaximoCaractere);
            RegraPara(x => x.Descricao)
                .TamanhoMaiorOuIgualA(5)
                .ComMensagem(ProdutoResource.ErroDescricaoLimiteMinimoCaractere);

            RegraPara(x => x.DataAquisicao)
                .DeveSer(d => d != default)
                .ComMensagem(ProdutoResource.ErroDataAquisicaoObrigatoria);

            RegraPara(x => x.PrecoUnitario)
                .DeveSer(p => p > 0)
                .ComMensagem(ProdutoResource.ErroPrecoProduto);
        }
    }
}
