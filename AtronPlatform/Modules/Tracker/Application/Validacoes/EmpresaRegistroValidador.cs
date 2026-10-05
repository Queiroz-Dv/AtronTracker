using Application.DTO.Request;
using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Extensions.RegraExtensions;

namespace Application.Validacoes
{
    public sealed class EmpresaRegistroValidador : Validador<EmpresaRegistroRequest>
    {
        public EmpresaRegistroValidador()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nameof(EmpresaRegistroRequest.Codigo)));
            RegraPara(x => x.Codigo).TamanhoEntre(3, 25);

            RegraPara(x => x.NomeFantasia)
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nameof(EmpresaRegistroRequest.NomeFantasia)));
            RegraPara(x => x.NomeFantasia).TamanhoEntre(3, 150);

            RegraPara(x => x.Endereco)
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nameof(EmpresaRegistroRequest.Endereco)));
            RegraPara(x => x.Endereco).TamanhoEntre(3, 200);

            RegraPara(x => x.Numero)
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nameof(EmpresaRegistroRequest.Numero)));
            RegraPara(x => x.Numero).TamanhoEntre(1, 20);

            RegraPara(x => x.Email)
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nameof(EmpresaRegistroRequest.Email)));
            RegraPara(x => x.Email).TamanhoEntre(3, 254);
            RegraPara(x => x.Email).EmailValido();
        }
    }
}
