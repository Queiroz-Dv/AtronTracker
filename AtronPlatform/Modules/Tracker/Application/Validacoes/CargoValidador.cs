using Application.DTO;
using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Extensions.RegraExtensions;

namespace Application.Validacoes
{
    /// <summary>
    /// Validador para CargoDTO seguindo o padrão IValidador
    /// </summary>
    public class CargoValidador : Validador<CargoDTO>
    {
        public CargoValidador()
        {
            RegrasParaCodigo();
            RegrasParaDescricao();
            RegrasParaDepartamento();
        }

        private void RegrasParaCodigo()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(CargoResource.ErroCodigoNulo);

            RegraPara(x => x.Codigo)
                .TamanhoMenorOuIgualA(10)
                .ComMensagem(CargoResource.ErroCodigoLongo);

            RegraPara(x => x.Codigo)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(CargoResource.ErroCodigoPequeno);
        }

        private void RegrasParaDescricao()
        {
            RegraPara(x => x.Descricao)
                .NaoVazio()
                .ComMensagem(CargoResource.ErroDescricaoNula);

            RegraPara(x => x.Descricao)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(CargoResource.ErroDescricaoPequena);

            RegraPara(x => x.Descricao)
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(CargoResource.ErroDescricaoLonga);
        }

        private void RegrasParaDepartamento()
        {
            RegraPara(x => x.DepartamentoCodigo)
                .NaoVazio()
                .ComMensagem(CargoResource.ErroDepartamentoObrigatorio);
        }
    }
}