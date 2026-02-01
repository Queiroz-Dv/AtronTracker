using Application.DTO.Request;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validations
{
    /// <summary>
    /// Validador para UsuarioRequest seguindo o padrão CategoriaValidador.
    /// Valida os dados de entrada antes de processar a operação no serviço.
    /// </summary>
    public class UsuarioRequestValidador : IValidador<UsuarioRequest>
    {
        public IList<NotificationMessage> Validar(UsuarioRequest request)
        {
            var context = new NotificationBag();

            if (request == null)
            {
                context.AdicionarErro(UsuarioResource.ErroUsuarioNulo);
                return context.Messages.ToList();
            }

            ValidarCodigo(request.Codigo, context);
            ValidarNome(request.Nome, context);
            ValidarSobrenome(request.Sobrenome, context);
            ValidarEmail(request.Email, context);

            return context.Messages.ToList();
        }

        private static void ValidarCodigo(string codigo, NotificationBag context)
        {
            if (codigo.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroCodigoNulo);
                return;
            }

            if (codigo.Length > 10)
            {
                context.AdicionarErro(UsuarioResource.ErroCodigoLongo);
            }

            if (codigo.Length < 3)
            {
                context.AdicionarErro(UsuarioResource.ErroCodigoPequeno);
            }
        }

        private static void ValidarNome(string nome, NotificationBag context)
        {
            if (nome.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroNomeUsuarioNulo);
                return;
            }

            if (nome.Length < 3)
            {
                context.AdicionarErro(UsuarioResource.ErroNomePequeno);
            }

            if (nome.Length > 25)
            {
                context.AdicionarErro(UsuarioResource.ErroNomeLongo);
            }
        }

        private static void ValidarSobrenome(string sobrenome, NotificationBag context)
        {
            if (sobrenome.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroSobrenomeObrigatorio);
                return;
            }

            if (sobrenome.Length < 3)
            {
                context.AdicionarErro(UsuarioResource.ErroSobrenomePequeno);
            }

            if (sobrenome.Length > 50)
            {
                context.AdicionarErro(UsuarioResource.ErroSobrenomeLongo);
            }
        }

        private static void ValidarEmail(string email, NotificationBag context)
        {
            if (email.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroEmailNulo);
            }
        }
    }
}
