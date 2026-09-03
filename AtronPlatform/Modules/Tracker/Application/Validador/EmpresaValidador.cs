using Application.DTO;
using Application.Resources;
using Shared.Application.Services;
using Shared.Extensions;
using Shared.Extensions.RegraExtensions;

namespace Application.Validador
{
    public sealed class EmpresaValidador : Validador<EmpresaDTO>
    {
        public EmpresaValidador()
        {
            RegrasParaCodigo();
            RegrasParaNomeFantasia();
            RegrasParaValidarEndereco();
            RegrasParaValidarNumero();
            RegrasParaValidarEmail();
            RegrasParaValidarStatus();
        }

        private void RegrasParaCodigo()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(EmpresaResource.Erro_CodigoVazioOuInvalido);

            RegraPara(x => x.Codigo)
                .TamanhoEntre(3, 25)
                .ComMensagem(EmpresaResource.Erro_TamanhoCodigo);
        }

        private void RegrasParaNomeFantasia()
        {
            RegraPara(x => x.NomeFantasia)
                .NaoVazio()
                .ComMensagem(EmpresaResource.Erro_NomeFantasiaVazioOuInvalido);

            RegraPara(x => x.NomeFantasia)
                .TamanhoEntre(20, 150)
                .ComMensagem(EmpresaResource.Erro_TamanhoNomeFantasia);
        }

        private void RegrasParaValidarEndereco()
        {
            RegraPara(x => x.Endereco)
                .NaoVazio()
                .ComMensagem(EmpresaResource.Erro_EnderecoVazioOuInvalido);

            RegraPara(x => x.Endereco)
                .TamanhoEntre(3, 200)
                .ComMensagem(EmpresaResource.Erro_TamanhoEndereco);
        }

        private void RegrasParaValidarNumero()
        {
            RegraPara(x => x.Numero)
                .NaoVazio()
                .ComMensagem(EmpresaResource.Erro_NumeroVazioOuInvalido);

            RegraPara(x => x.Numero)
                .TamanhoEntre(16, 16) // Se não funcionar terei de usar  método DeveSer
                .ComMensagem(EmpresaResource.Erro_TamanhoNumero);
        }

        private void RegrasParaValidarEmail()
        {
            RegraPara(x => x.Email)
               .NaoVazio()
                .EmailValido()
               .ComMensagem(EmpresaResource.Erro_EmailVazioOuInvalido);

            RegraPara(x => x.Email)
                .TamanhoEntre(3, 30)
                .ComMensagem(EmpresaResource.Erro_TamanhoEmail);
        }

        private void RegrasParaValidarStatus()
        {
            RegraPara(x => x.Status)
             .DeveSer(status => status.IsValidEnum());
        }
    }
}