using Application.DTO;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;

namespace Application.Validador
{
    /// <summary>
    /// Validador para CargoDTO seguindo o padrão IValidador
    /// </summary>
    public class CargoValidador : IValidador<CargoDTO>
    {
        public IList<NotificationMessage> Validar(CargoDTO entity)
        {
            var notificacoes = new NotificationBag();

            if (entity == null)
            {
                notificacoes.AdicionarErro(NotificacoesPadronizadas.ErroRegistroNulo);
                return [.. notificacoes.Messages];
            }

            ValidarCodigo(entity, notificacoes);

            ValidarDescricao(entity, notificacoes);

            ValidarDepartamento(entity, notificacoes);

            return [.. notificacoes.Messages];
        }

        private static void ValidarCodigo(CargoDTO entity, NotificationBag notificacoes)
        {
            if (entity.Codigo.IsNullOrEmpty())
            {
                notificacoes.AdicionarErro(CargoResource.ErroCodigoNulo);
                return;
            }

            if (entity.Codigo.Length > 10)
            {
                notificacoes.AdicionarErro(CargoResource.ErroCodigoLongo);
            }

            if (entity.Codigo.Length < 3)
            {
                notificacoes.AdicionarErro(CargoResource.ErroCodigoPequeno);
            }
        }

        private static void ValidarDescricao(CargoDTO entity, NotificationBag notificacoes)
        {
            if (entity.Descricao.IsNullOrEmpty())
            {
                notificacoes.AdicionarErro(CargoResource.ErroDescricaoNula);
                return;
            }

            if (entity.Descricao.Length < 3)
            {
                notificacoes.AdicionarErro(CargoResource.ErroDescricaoPequena);
            }

            if (entity.Descricao.Length > 50)
            {
                notificacoes.AdicionarErro(CargoResource.ErroDescricaoLonga);
            }
        }

        private static void ValidarDepartamento(CargoDTO entity, NotificationBag notificacoes)
        {
            if (entity.DepartamentoCodigo.IsNullOrEmpty())
            {
                notificacoes.AdicionarErro(CargoResource.ErroDepartamentoObrigatorio);
            }
        }
    }
}
