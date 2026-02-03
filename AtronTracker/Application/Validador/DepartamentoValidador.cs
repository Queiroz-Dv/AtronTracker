using Application.DTO;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;

namespace Application.Validador
{
    public class DepartamentoValidador : IValidador<DepartamentoDTO>
    {
        public IList<NotificationMessage> Validar(DepartamentoDTO entity)
        {
            var notificacoes = new NotificationBag();

            if (entity == null)
            {
                notificacoes.AdicionarErro(NotificacoesPadronizadas.ErroRegistroNulo);
                return [.. notificacoes.Messages];
            }

            ValidarCodigo(entity, notificacoes);

            ValidarDescricao(entity, notificacoes);

            return [.. notificacoes.Messages];
        }

        private static void ValidarCodigo(DepartamentoDTO entity, NotificationBag notificacoes)
        {

            if (entity.Codigo.IsNullOrEmpty())
            {
                notificacoes.AdicionarErro(DepartamentoResource.ErroCodigoNulo);
            }

            if (entity.Codigo.Length > 10)
            {
                notificacoes.AdicionarErro(DepartamentoResource.ErroCodigoLongo);
            }

            if (entity.Codigo.Length < 3)
            {
                notificacoes.AdicionarErro(DepartamentoResource.ErroCodigoPequeno);
            }
        }

        private static void ValidarDescricao(DepartamentoDTO entity, NotificationBag notificacoes)
        {
            if (entity.Descricao.IsNullOrEmpty())
            {
                notificacoes.AdicionarErro(DepartamentoResource.ErroDescricaoNula);
            }

            if (entity.Descricao.Length < 3)
            {
                notificacoes.AdicionarErro(DepartamentoResource.ErroDescricaoPequena);
            }

            if (entity.Descricao.Length > 50)
            {
                notificacoes.AdicionarErro(DepartamentoResource.ErroDescricaoLonga);
            }
        }

    }
}
