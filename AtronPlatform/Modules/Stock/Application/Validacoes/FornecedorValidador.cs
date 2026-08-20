using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Net.Mail;

namespace AtronStock.Application.Validacoes
{
    public class FornecedorValidador : IValidador<FornecedorRequest>
    {
        public IEnumerable<NotificationMessage> Validar(FornecedorRequest entity)
        {
            var context = new NotificationBag();
            ValidaCodigo(entity, context);
            ValidaNome(entity, context);

            ValidarEmail(entity, context);
            ValidarTelefone(entity, context);

            ValidarEndereco(entity, context);

            ValidarDocumento(entity, context);

            return [.. context.Messages];
        }

        private static void ValidarDocumento(FornecedorRequest entity, NotificationBag context)
        {
            if (!entity.CNPJ.IsNullOrEmpty())
            {
                if (entity.CNPJ.Length > 14)
                {
                    if (!DocumentoValidator.IsValidCnpj(entity.CNPJ))
                    {
                        context.AdicionarErro(FornecedorResource.ErroCnpjInvalido);
                    }
                }
            }
            else
            {
                context.AdicionarErro(FornecedorResource.ErroCnpjInvalido);
            }
        }

        private static void ValidarEndereco(FornecedorRequest entity, NotificationBag context)
        {
            if (entity.EnderecoVO != null)
            {
                if (!entity.EnderecoVO.Logradouro.IsNullOrEmpty() && entity.EnderecoVO.Logradouro.Length > 100)
                {
                    context.AdicionarErro(FornecedorResource.ErroLogradouroTamanho);
                }

                if (!entity.EnderecoVO.Numero.IsNullOrEmpty() && entity.EnderecoVO.Numero.Length > 10)
                {
                    context.AdicionarErro(FornecedorResource.ErroNumeroEnderecoTamanho);
                }

                if (!entity.EnderecoVO.Cidade.IsNullOrEmpty() && entity.EnderecoVO.Cidade.Length > 50)
                {
                    context.AdicionarErro(FornecedorResource.ErroCidadeTamanho);
                }

                if (!entity.EnderecoVO.UF.IsNullOrEmpty() && entity.EnderecoVO.UF.Length != 2)
                {
                    context.AdicionarErro(FornecedorResource.ErroUfTamanho);
                }

                if (!entity.EnderecoVO.CEP.IsNullOrEmpty() && entity.EnderecoVO.CEP.Length != 9)
                {
                    context.AdicionarErro(FornecedorResource.ErroCepTamanho);
                }
            }
        }

        private static void ValidarTelefone(FornecedorRequest entity, NotificationBag context)
        {
            if (!entity.Telefone.IsNullOrEmpty())
            {
                if (entity.Telefone.Length > 15 || entity.Telefone.Length < 8)
                {
                    context.AdicionarErro(FornecedorResource.ErroTelefoneTamanho);
                }
            }
        }

        private static void ValidarEmail(FornecedorRequest entity, NotificationBag context)
        {
            if (entity.Email.IsNullOrEmpty())
            {
                context.AdicionarErro(FornecedorResource.ErroEmailObrigatorio);
            }
            else if (entity.Email.Length > 50)
            {
                context.AdicionarErro(FornecedorResource.ErroEmailTamanho);
            }
            else
            {
                try
                {
                    var m = new MailAddress(entity.Email);
                }
                catch (FormatException)
                {
                    context.AdicionarErro(FornecedorResource.ErroEmailInvalido);
                }
            }
        }

        private static void ValidaCodigo(FornecedorRequest entity, NotificationBag context)
        {
            if (entity.Codigo.IsNullOrEmpty())
            {
                context.AdicionarErro(FornecedorResource.ErroCodigoObrigatorio);
            }
            else if (entity.Codigo.Length > 20)
            {
                context.AdicionarErro(FornecedorResource.ErroCodigoLimiteMaximoDeCaractere);
            }
            else if (entity.Codigo.Length < 3)
            {
                context.AdicionarErro(FornecedorResource.ErroCodigoLimiteMinimoDeCaractere);
            }
        }
        private static void ValidaNome(FornecedorRequest entity, NotificationBag context)
        {
            if (entity.Nome.IsNullOrEmpty())
            {
                context.AdicionarErro(FornecedorResource.ErroNomeObrigatorio);
            }
            else if (entity.Nome.Length > 100)
            {
                context.AdicionarErro(FornecedorResource.ErroNomeLimiteMaximoDeCaractere);
            }
            else if (entity.Nome.Length < 5)
            {
                context.AdicionarErro(FornecedorResource.ErroNomeLimiteMinimoDeCaractere);
            }
        }
    }
}
