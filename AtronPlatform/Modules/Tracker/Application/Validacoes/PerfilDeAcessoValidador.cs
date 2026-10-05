using Application.DTO;
using Application.Resources;
using Shared.Application.Services;
using Shared.Extensions.RegraExtensions;

namespace Application.Validacoes
{
    public sealed class PerfilDeAcessoValidador : Validador<PerfilDeAcessoDTO>
    {
        public PerfilDeAcessoValidador()
        {
            RegraPara(x => x)
                .DeveSer(p => !string.IsNullOrEmpty(p.Codigo) && !string.IsNullOrEmpty(p.Descricao))
                .ComMensagem(PerfilDeAcessoResource.Erro_DadosObrigatorios);

            RegraPara(x => x.Codigo)
                .Quando(x => !string.IsNullOrEmpty(x.Codigo))
                .TamanhoMenorOuIgualA(10)
                .ComMensagem(PerfilDeAcessoResource.Erro_CodigoLongo);

            RegraPara(x => x.Codigo)
                .Quando(x => !string.IsNullOrEmpty(x.Codigo))
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(PerfilDeAcessoResource.Erro_CodigoPequeno);

            RegraPara(x => x.Descricao)
                .Quando(x => !string.IsNullOrEmpty(x.Descricao))
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(PerfilDeAcessoResource.Erro_DescricaoPequena);

            RegraPara(x => x.Descricao)
                .Quando(x => !string.IsNullOrEmpty(x.Descricao))
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(PerfilDeAcessoResource.Erro_DescricaoLonga);

            RegraPara(x => x.Modulos)
                .DeveSer(m => m != null && m.Count > 0)
                .ComMensagem(PerfilDeAcessoResource.Erro_SemModulos);
        }
    }
}
