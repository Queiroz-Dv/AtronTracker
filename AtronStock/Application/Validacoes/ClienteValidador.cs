using AtronStock.Application.DTO.Request;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using Shared.Models;
using System.Net.Mail;

namespace AtronStock.Application.Validacoes
{
    public class ClienteValidador : IValidador<ClienteRequest>
    {
        public IList<NotificationMessage> Validar(ClienteRequest entity)
        {
            var context = new NotificationBag();

            if (entity.Nome.IsNullOrEmpty())
            {
                context.AdicionarErro("O Nome é obrigatório.");
            }
            else if (entity.Nome.Length < 3 || entity.Nome.Length > 50)
            {
                context.AdicionarErro("O tamanho do nome está inválido (min 3, max 50).");
            }

            if (entity.Codigo.IsNullOrEmpty())
            {
                context.AdicionarErro("O Código é obrigatório.");
            }
            else if (entity.Codigo.Length < 3 || entity.Codigo.Length > 25)
            {
                context.AdicionarErro("O tamanho do código está inválido (min 3, max 25).");
            }

            if (!entity.CPF.IsNullOrEmpty())
            {
                if (!DocumentoValidator.IsValidCpf(entity.CPF))
                {
                    context.AdicionarErro("O formato do CPF é inválido.");
                }
            }

            if (!entity.CNPJ.IsNullOrEmpty())
            {
                if (!DocumentoValidator.IsValidCnpj(entity.CNPJ))
                {
                    context.AdicionarErro("O formato do CNPJ é inválido.");
                }
            }

            if (entity.StatusPessoa.GetDescription().IsNullOrEmpty())
            {
                context.AdicionarErro("O status do cliente deve ser informado.");
            }

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

            if (!entity.Telefone.IsNullOrEmpty())
            {
                if (entity.Telefone.Length > 15 || entity.Telefone.Length < 8)
                {
                    context.AdicionarErro("O tamanho do Telefone está inválido (min 8, max 15).");
                }
            }


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

            return context.Messages.ToList();
        }
    }
}