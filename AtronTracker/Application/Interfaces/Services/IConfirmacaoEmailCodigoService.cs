using Domain.Entities;

namespace Application.Interfaces.Services
{
    public interface IConfirmacaoEmailCodigoService
    {
        string GerarIdentificador();
        string GerarHash(string valor);
        (ConfirmacaoEmail ConfirmacaoEmail, string Identificador) CriarDadosConfirmacao(string usuarioCodigo, int validadeEmHoras);
        bool ConfirmacaoValida(ConfirmacaoEmail confirmacaoEmail, string usuarioCodigo, string identificador);
    }
}
