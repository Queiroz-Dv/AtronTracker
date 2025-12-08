using AtronStock.Application.DTO.Request;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources.AtronStock;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Net.Mail;

namespace AtronStock.Application.Validacoes
{
    public class FornecedorValidador : IValidador<FornecedorRequest>
    {
        public IList<NotificationMessage> Validar(FornecedorRequest entity)
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
                        context.AdicionarErro("O formato do CNPJ é inválido.");
                    }
                }
            }
            else
            {
                context.AdicionarErro("O formato do CNPJ é inválido.");
            }
        }

        private static void ValidarEndereco(FornecedorRequest entity, NotificationBag context)
        {
            if (entity.EnderecoVO != null)
            {
                if (!entity.EnderecoVO.Logradouro.IsNullOrEmpty() && entity.EnderecoVO.Logradouro.Length > 100)
                {
                    context.AdicionarErro("O Logradouro excede o limite de 100 caracteres.");
                }

                if (!entity.EnderecoVO.Numero.IsNullOrEmpty() && entity.EnderecoVO.Numero.Length > 10)
                {
                    context.AdicionarErro("O Número do endereço excede o limite de 10 caracteres.");
                }

                if (!entity.EnderecoVO.Cidade.IsNullOrEmpty() && entity.EnderecoVO.Cidade.Length > 50)
                {
                    context.AdicionarErro("A Cidade excede o limite de 50 caracteres.");
                }

                if (!entity.EnderecoVO.UF.IsNullOrEmpty() && entity.EnderecoVO.UF.Length != 2)
                {
                    context.AdicionarErro("A UF deve conter exatamente 2 caracteres.");
                }

                if (!entity.EnderecoVO.CEP.IsNullOrEmpty() && entity.EnderecoVO.CEP.Length != 9)
                {
                    context.AdicionarErro("O CEP deve conter exatamente 9 caracteres (ex: 12345-678).");
                }
            }
        }

        private static void ValidarTelefone(FornecedorRequest entity, NotificationBag context)
        {
            if (!entity.Telefone.IsNullOrEmpty())
            {
                if (entity.Telefone.Length > 15 || entity.Telefone.Length < 8)
                {
                    context.AdicionarErro("O tamanho do Telefone está inválido (min 8, max 15).");
                }
            }
        }

        private static void ValidarEmail(FornecedorRequest entity, NotificationBag context)
        {
            if (entity.Email.IsNullOrEmpty())
            {
                context.AdicionarErro("O Email é obrigatório.");
            }
            else if (entity.Email.Length > 50)
            {
                context.AdicionarErro("O Email excede o limite de 50 caracteres.");
            }
            else
            {
                try
                {
                    var m = new MailAddress(entity.Email);
                }
                catch (FormatException)
                {
                    context.AdicionarErro("O formato do Email é inválido.");
                }
            }
        }

        private static void ValidaCodigo(FornecedorRequest entity, NotificationBag context)
        {
            if (entity.Codigo.IsNullOrEmpty())
            {
                context.AdicionarErro(FornecedoResource.ErroCodigoObrigatorio);
            }
            else if (entity.Codigo.Length > 20)
            {
                context.AdicionarErro(FornecedoResource.ErroCodigoLimiteMaximoDeCaractere);
            }
            else if (entity.Codigo.Length < 3)
            {
                context.AdicionarErro(FornecedoResource.ErroCodigoLimiteMinimoDeCaractere);
            }
        }
        private static void ValidaNome(FornecedorRequest entity, NotificationBag context)
        {
            if (entity.Nome.IsNullOrEmpty())
            {
                context.AdicionarErro(FornecedoResource.ErroNomeObrigatorio);
            }
            else if (entity.Nome.Length > 100)
            {
                context.AdicionarErro(FornecedoResource.ErroNomeLimiteMaximoDeCaractere);
            }
            else if (entity.Nome.Length < 5)
            {
                context.AdicionarErro(FornecedoResource.ErroNomeLimiteMinimoDeCaractere);
            }
        }
    }
}
