using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
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
                context.AdicionarErro(ClienteResource.ErroNomeObrigatorio);
            }
            else if (entity.Nome.Length < 3 || entity.Nome.Length > 50)
            {
                context.AdicionarErro(ClienteResource.ErroNomeTamanho);
            }

            if (entity.Codigo.IsNullOrEmpty())
            {
                context.AdicionarErro(ClienteResource.ErroCodigoObrigatorio);
            }
            else if (entity.Codigo.Length < 3 || entity.Codigo.Length > 25)
            {
                context.AdicionarErro(ClienteResource.ErroCodigoTamanho);
            }

            if (!entity.Documento.Dado.IsNullOrEmpty())
            {
                if (entity.Documento.Dado.Length > 11)
                {
                    if (!DocumentoValidator.IsValidCpf(entity.Documento.Dado))
                    {
                        context.AdicionarErro(ClienteResource.ErroCpfInvalido);
                    }
                }

                if (entity.Documento.Dado.Length > 14)
                {
                    if (!DocumentoValidator.IsValidCnpj(entity.Documento.Dado))
                    {
                        context.AdicionarErro(ClienteResource.ErroCnpjInvalido);
                    }
                }
            }
            
            if (entity.StatusPessoa.GetDescription().IsNullOrEmpty())
            {
                context.AdicionarErro(ClienteResource.ErroStatusObrigatorio);
            }

            if (entity.Email.IsNullOrEmpty())
            {
                context.AdicionarErro(ClienteResource.ErroEmailObrigatorio);
            }
            else if (entity.Email.Length > 50)
            {
                context.AdicionarErro(ClienteResource.ErroEmailTamanho);
            }
            else
            {
                try
                {
                    var m = new MailAddress(entity.Email);
                }
                catch (FormatException)
                {
                    context.AdicionarErro(ClienteResource.ErroEmailInvalido);
                }
            }

            if (!entity.Telefone.IsNullOrEmpty())
            {
                if (entity.Telefone.Length > 15 || entity.Telefone.Length < 8)
                {
                    context.AdicionarErro(ClienteResource.ErroTelefoneTamanho);
                }
            }


            if (entity.EnderecoVO != null)
            {
                if (!entity.EnderecoVO.Logradouro.IsNullOrEmpty() && entity.EnderecoVO.Logradouro.Length > 100)
                {
                    context.AdicionarErro(ClienteResource.ErroLogradouroTamanho);
                }

                if (!entity.EnderecoVO.Numero.IsNullOrEmpty() && entity.EnderecoVO.Numero.Length > 10)
                {
                    context.AdicionarErro(ClienteResource.ErroNumeroEnderecoTamanho);
                }

                if (!entity.EnderecoVO.Cidade.IsNullOrEmpty() && entity.EnderecoVO.Cidade.Length > 50)
                {
                    context.AdicionarErro(ClienteResource.ErroCidadeTamanho);
                }

                if (!entity.EnderecoVO.UF.IsNullOrEmpty() && entity.EnderecoVO.UF.Length != 2)
                {
                    context.AdicionarErro(ClienteResource.ErroUfTamanho);
                }

                if (!entity.EnderecoVO.CEP.IsNullOrEmpty() && entity.EnderecoVO.CEP.Length != 9)
                {
                    context.AdicionarErro(ClienteResource.ErroCepTamanho);
                }
            }

            return context.Messages.ToList();
        }
    }
}
