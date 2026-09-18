using Application.DTO;
using Application.Resources;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Application.Validacoes
{
    public class PlanejamentoCustoValidador : IValidador<PlanejamentoCustoDTO>
    {
        public IEnumerable<NotificationMessage> Validar(PlanejamentoCustoDTO entity)
        {
            var notificacoes = new NotificationBag();

            if (entity == null)
            {
                notificacoes.AdicionarErro(NotificacoesPadronizadas.ErroRegistroNulo);
                return [.. notificacoes.Messages];
            }

            if (entity.Codigo.IsNullOrEmpty())
                notificacoes.AdicionarErro(PlanejamentoCustoResource.Erro_CodigoObrigatorio);

            if (!entity.Codigo.IsNullOrEmpty() && entity.Codigo.Length > 10)
                notificacoes.AdicionarErro(PlanejamentoCustoResource.Erro_CodigoTamanhoMaximo);

            if (entity.Descricao.IsNullOrEmpty())
                notificacoes.AdicionarErro(PlanejamentoCustoResource.Erro_DescricaoObrigatoria);

            if (!entity.Descricao.IsNullOrEmpty() && entity.Descricao.Length > 100)
                notificacoes.AdicionarErro(PlanejamentoCustoResource.Erro_DescricaoTamanhoMaximo);

            if (entity.DepartamentoCodigo.IsNullOrEmpty())
                notificacoes.AdicionarErro(PlanejamentoCustoResource.Erro_DepartamentoObrigatorio);

            if (entity.Ano < DateTime.Today.Year)
                notificacoes.AdicionarErro(PlanejamentoCustoResource.Erro_AnoPassadoNaoPermitido);

            if (entity.ValorMinimo < 0)
                notificacoes.AdicionarErro(PlanejamentoCustoResource.Erro_ValorMinimoNegativo);

            if (entity.ValorTeto <= 0)
                notificacoes.AdicionarErro(PlanejamentoCustoResource.Erro_ValorTetoInvalido);

            if (entity.ValorMinimo >= entity.ValorTeto)
                notificacoes.AdicionarErro(PlanejamentoCustoResource.Erro_ValorMinimoMaiorTeto);

            return [.. notificacoes.Messages];
        }
    }
}
