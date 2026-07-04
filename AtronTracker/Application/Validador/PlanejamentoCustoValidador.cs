using Application.DTO;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Application.Validador
{
    public class PlanejamentoCustoValidador : IValidador<PlanejamentoCustoDTO>
    {
        public IList<NotificationMessage> Validar(PlanejamentoCustoDTO entity)
        {
            var notificacoes = new NotificationBag();

            if (entity == null)
            {
                notificacoes.AdicionarErro(NotificacoesPadronizadas.ErroRegistroNulo);
                return [.. notificacoes.Messages];
            }

            if (entity.Codigo.IsNullOrEmpty())
                notificacoes.AdicionarErro("O código do planejamento de custo é obrigatório.");

            if (!entity.Codigo.IsNullOrEmpty() && entity.Codigo.Length > 10)
                notificacoes.AdicionarErro("O código do planejamento de custo deve ter no máximo 10 caracteres.");

            if (entity.Descricao.IsNullOrEmpty())
                notificacoes.AdicionarErro("A descrição do planejamento de custo é obrigatória.");

            if (!entity.Descricao.IsNullOrEmpty() && entity.Descricao.Length > 100)
                notificacoes.AdicionarErro("A descrição do planejamento de custo deve ter no máximo 100 caracteres.");

            if (entity.DepartamentoCodigo.IsNullOrEmpty())
                notificacoes.AdicionarErro("O departamento do planejamento de custo é obrigatório.");

            if (entity.Ano < DateTime.Today.Year)
                notificacoes.AdicionarErro("Não é permitido criar ou alterar planejamento de custo para ano passado.");

            if (entity.ValorMinimo < 0)
                notificacoes.AdicionarErro("O valor mínimo do planejamento de custo não pode ser negativo.");

            if (entity.ValorTeto <= 0)
                notificacoes.AdicionarErro("O valor teto do planejamento de custo deve ser maior que zero.");

            if (entity.ValorMinimo >= entity.ValorTeto)
                notificacoes.AdicionarErro("O valor mínimo deve ser menor que o valor teto do planejamento de custo.");

            return [.. notificacoes.Messages];
        }
    }
}
