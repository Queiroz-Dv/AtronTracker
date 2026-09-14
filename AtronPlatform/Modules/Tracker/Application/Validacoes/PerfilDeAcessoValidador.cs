using Application.DTO;
using Application.Resources;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;

namespace Application.Validacoes
{
    public sealed class PerfilDeAcessoValidador : IValidador<PerfilDeAcessoDTO>
    {
        public IEnumerable<NotificationMessage> Validar(PerfilDeAcessoDTO perfil)
        {
            var notificacoes = new NotificationBag();

            if (perfil is null)
            {
                notificacoes.AdicionarErro(PerfilDeAcessoResource.Erro_PerfilInvalido);
                return notificacoes.Messages;
            }

            if (string.IsNullOrEmpty(perfil.Codigo) || string.IsNullOrEmpty(perfil.Descricao))
                notificacoes.AdicionarErro(PerfilDeAcessoResource.Erro_DadosObrigatorios);

            if (!string.IsNullOrEmpty(perfil.Codigo) && perfil.Codigo.Length > 10)
                notificacoes.AdicionarErro(PerfilDeAcessoResource.Erro_CodigoLongo);

            if (!string.IsNullOrEmpty(perfil.Codigo) && perfil.Codigo.Length < 3)
                notificacoes.AdicionarErro(PerfilDeAcessoResource.Erro_CodigoPequeno);

            if (!string.IsNullOrEmpty(perfil.Descricao) && perfil.Descricao.Length < 3)
                notificacoes.AdicionarErro(PerfilDeAcessoResource.Erro_DescricaoPequena);

            if (!string.IsNullOrEmpty(perfil.Descricao) && perfil.Descricao.Length > 50)
                notificacoes.AdicionarErro(PerfilDeAcessoResource.Erro_DescricaoLonga);

            if (perfil.Modulos is null || perfil.Modulos.Count == 0)
                notificacoes.AdicionarErro(PerfilDeAcessoResource.Erro_SemModulos);

            return notificacoes.Messages;
        }
    }
}
