using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using Shared.Application.Services;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using Shared.Extensions.RegraExtensions;

namespace AtronStock.Application.Validacoes
{
    public class FornecedorValidador : Validador<FornecedorRequest>
    {
        public FornecedorValidador()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(FornecedorResource.ErroCodigoObrigatorio);
            RegraPara(x => x.Codigo)
                .TamanhoEntre(3, 20)
                .ComMensagem(FornecedorResource.ErroCodigoLimiteMaximoDeCaractere); // wait, old logic: if > 20 then MaximoDeCaractere, else if < 3 then MinimoDeCaractere.

            RegraPara(x => x.Codigo)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(FornecedorResource.ErroCodigoLimiteMinimoDeCaractere);

            RegraPara(x => x.Nome)
                .NaoVazio()
                .ComMensagem(FornecedorResource.ErroNomeObrigatorio);
            RegraPara(x => x.Nome)
                .TamanhoMenorOuIgualA(100)
                .ComMensagem(FornecedorResource.ErroNomeLimiteMaximoDeCaractere);
            RegraPara(x => x.Nome)
                .TamanhoMaiorOuIgualA(5)
                .ComMensagem(FornecedorResource.ErroNomeLimiteMinimoDeCaractere);

            RegraPara(x => x.Email)
                .NaoVazio()
                .ComMensagem(FornecedorResource.ErroEmailObrigatorio);
            RegraPara(x => x.Email)
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(FornecedorResource.ErroEmailTamanho);
            RegraPara(x => x.Email)
                .EmailValido()
                .ComMensagem(FornecedorResource.ErroEmailInvalido);

            RegraPara(x => x.Telefone)
                .DeveSer(tel => tel.IsNullOrEmpty() || (tel.Length >= 8 && tel.Length <= 15))
                .ComMensagem(FornecedorResource.ErroTelefoneTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.Logradouro.IsNullOrEmpty() || end.Logradouro.Length <= 100)
                .ComMensagem(FornecedorResource.ErroLogradouroTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.Numero.IsNullOrEmpty() || end.Numero.Length <= 10)
                .ComMensagem(FornecedorResource.ErroNumeroEnderecoTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.Cidade.IsNullOrEmpty() || end.Cidade.Length <= 50)
                .ComMensagem(FornecedorResource.ErroCidadeTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.UF.IsNullOrEmpty() || end.UF.Length == 2)
                .ComMensagem(FornecedorResource.ErroUfTamanho);

            RegraPara(x => x.EnderecoVO)
                .DeveSer(end => end == null || end.CEP.IsNullOrEmpty() || end.CEP.Length == 9)
                .ComMensagem(FornecedorResource.ErroCepTamanho);

            RegraPara(x => x.CNPJ)
                .NaoVazio()
                .ComMensagem(FornecedorResource.ErroCnpjInvalido);
            
            RegraPara(x => x.CNPJ)
                .DeveSer(cnpj => cnpj.IsNullOrEmpty() || cnpj.Length <= 14 || DocumentoValidator.IsValidCnpj(cnpj))
                .ComMensagem(FornecedorResource.ErroCnpjInvalido);
        }
    }
}
