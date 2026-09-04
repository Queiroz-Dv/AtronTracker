using Application.DTO;
using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Extensions.RegraExtensions;

namespace Application.Validacoes
{
    public class DepartamentoValidacoes : Validador<DepartamentoDTO>
    {
        public DepartamentoValidacoes()
        {
            RegrasParaCodigo();
            RegraParaDescricao();
        }

        private void RegrasParaCodigo()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(DepartamentoResource.ErroCodigoNulo);

            RegraPara(x => x.Codigo)
                .TamanhoEntre(3, 10)
                .ComMensagem(DepartamentoResource.Erro_TamanhoCodigo);
        }

        private void RegraParaDescricao()
        {
            RegraPara(x => x.Descricao)
                .NaoVazio()
                .ComMensagem(DepartamentoResource.ErroDescricaoNula);

            RegraPara(x => x.Descricao)
                .TamanhoEntre(3, 50)
                .ComMensagem(DepartamentoResource.Erro_TamanhoDescricao);
        }
    }
}