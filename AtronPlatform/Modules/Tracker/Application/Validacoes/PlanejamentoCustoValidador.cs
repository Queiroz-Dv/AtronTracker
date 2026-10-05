using Application.DTO;
using Application.Resources;
using Shared.Application.Services;
using Shared.Extensions.RegraExtensions;
using System;

namespace Application.Validacoes
{
    public class PlanejamentoCustoValidador : Validador<PlanejamentoCustoDTO>
    {
        public PlanejamentoCustoValidador()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(PlanejamentoCustoResource.Erro_CodigoObrigatorio);

            RegraPara(x => x.Codigo)
                .TamanhoMenorOuIgualA(10)
                .ComMensagem(PlanejamentoCustoResource.Erro_CodigoTamanhoMaximo);

            RegraPara(x => x.Descricao)
                .NaoVazio()
                .ComMensagem(PlanejamentoCustoResource.Erro_DescricaoObrigatoria);

            RegraPara(x => x.Descricao)
                .TamanhoMenorOuIgualA(100)
                .ComMensagem(PlanejamentoCustoResource.Erro_DescricaoTamanhoMaximo);

            RegraPara(x => x.DepartamentoCodigo)
                .NaoVazio()
                .ComMensagem(PlanejamentoCustoResource.Erro_DepartamentoObrigatorio);

            RegraPara(x => x.Ano)
                .DeveSer(ano => ano >= DateTime.Today.Year)
                .ComMensagem(PlanejamentoCustoResource.Erro_AnoPassadoNaoPermitido);

            RegraPara(x => x.ValorMinimo)
                .DeveSer(vm => vm >= 0)
                .ComMensagem(PlanejamentoCustoResource.Erro_ValorMinimoNegativo);

            RegraPara(x => x.ValorTeto)
                .DeveSer(vt => vt > 0)
                .ComMensagem(PlanejamentoCustoResource.Erro_ValorTetoInvalido);

            RegraPara(x => x)
                .DeveSer(p => p.ValorMinimo < p.ValorTeto)
                .ComMensagem(PlanejamentoCustoResource.Erro_ValorMinimoMaiorTeto);
        }
    }
}
