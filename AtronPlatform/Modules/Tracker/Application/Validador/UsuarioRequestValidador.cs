using Application.DTO.Request;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validador
{
    /// <summary>
    /// Valida os dados de entrada antes de processar a operação no serviço.
    /// </summary>
    public class UsuarioRequestValidador : IValidador<UsuarioRequest>
    {
        public IList<NotificationMessage> Validar(UsuarioRequest request)
        {
            var notificacoes = new NotificationBag();

            if (request == null)
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroUsuarioNulo);
                return notificacoes.Messages.ToList();
            }

            ValidarCodigo(request.Codigo, notificacoes);
            ValidarNome(request.Nome, notificacoes);
            ValidarSobrenome(request.Sobrenome, notificacoes);
            ValidarEmail(request.Email, notificacoes);

            return notificacoes.Messages.ToList();
        }

        private static void ValidarCodigo(string codigo, NotificationBag notificacoes)
        {
            if (codigo.IsNullOrEmpty())
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroCodigoNulo);
                return;
            }

            if (codigo.Length > 10)
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroCodigoLongo);
            }

            if (codigo.Length < 3)
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroCodigoPequeno);
            }
        }

        private static void ValidarNome(string nome, NotificationBag notificacoes)
        {
            if (nome.IsNullOrEmpty())
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroNomeUsuarioNulo);
                return;
            }

            if (nome.Length < 3)
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroNomePequeno);
            }

            if (nome.Length > 25)
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroNomeLongo);
            }
        }

        private static void ValidarSobrenome(string sobrenome, NotificationBag notificacoes)
        {
            if (sobrenome.IsNullOrEmpty())
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroSobrenomeObrigatorio);
                return;
            }

            if (sobrenome.Length < 3)
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroSobrenomePequeno);
            }

            if (sobrenome.Length > 50)
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroSobrenomeLongo);
            }
        }

        private static void ValidarEmail(string email, NotificationBag notificacoes)
        {
            if (email.IsNullOrEmpty())
            {
                notificacoes.AdicionarErro(UsuarioResource.ErroEmailNulo);
            }
        }
    }
}
