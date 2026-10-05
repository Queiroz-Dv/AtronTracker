using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using Shared.Application.Services;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using Shared.Extensions.RegraExtensions;

namespace AtronStock.Application.Validacoes
{
    public class ClienteValidador : Validador<ClienteRequest>
    {
        public ClienteValidador()
        {
            RegraPara(x => x.Nome)
                .NaoVazio()
                .ComMensagem(ClienteResource.ErroNomeObrigatorio);
            RegraPara(x => x.Nome)
                .TamanhoEntre(3, 50)
                .ComMensagem(ClienteResource.ErroNomeTamanho);

            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(ClienteResource.ErroCodigoObrigatorio);
            RegraPara(x => x.Codigo)
                .TamanhoEntre(3, 25)
                .ComMensagem(ClienteResource.ErroCodigoTamanho);

            RegraPara(x => x.Documento.Dado)
                .DeveSer(doc => doc.IsNullOrEmpty() || doc.Length <= 11 || DocumentoValidator.IsValidCpf(doc))
                .ComMensagem(ClienteResource.ErroCpfInvalido);

            RegraPara(x => x.Documento.Dado)
                .DeveSer(doc => doc.IsNullOrEmpty() || doc.Length <= 14 || DocumentoValidator.IsValidCnpj(doc))
                .ComMensagem(ClienteResource.ErroCnpjInvalido);

            RegraPara(x => x.StatusPessoa)
                .DeveSer(status => !status.GetDescription().IsNullOrEmpty())
                .ComMensagem(ClienteResource.ErroStatusObrigatorio);

            RegraPara(x => x.Email)
                .NaoVazio()
                .ComMensagem(ClienteResource.ErroEmailObrigatorio);
            RegraPara(x => x.Email)
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(ClienteResource.ErroEmailTamanho);
            RegraPara(x => x.Email)
                .EmailValido()
                .ComMensagem(ClienteResource.ErroEmailInvalido);

            RegraPara(x => x.Telefone)
                .DeveSer(tel => tel.IsNullOrEmpty() || (tel.Length >= 8 && tel.Length <= 15))
                .ComMensagem(ClienteResource.ErroTelefoneTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.Logradouro.IsNullOrEmpty() || end.Logradouro.Length <= 100)
                .ComMensagem(ClienteResource.ErroLogradouroTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.Numero.IsNullOrEmpty() || end.Numero.Length <= 10)
                .ComMensagem(ClienteResource.ErroNumeroEnderecoTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.Cidade.IsNullOrEmpty() || end.Cidade.Length <= 50)
                .ComMensagem(ClienteResource.ErroCidadeTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.UF.IsNullOrEmpty() || end.UF.Length == 2)
                .ComMensagem(ClienteResource.ErroUfTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.CEP.IsNullOrEmpty() || end.CEP.Length == 9)
                .ComMensagem(ClienteResource.ErroCepTamanho);
        }
    }
}
