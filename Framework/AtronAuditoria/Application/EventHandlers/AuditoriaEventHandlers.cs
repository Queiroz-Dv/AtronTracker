using System;
using System.Threading;
using System.Threading.Tasks;
using AtronAuditoria.Application.Interfaces.Repositories;
using AtronAuditoria.Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Messaging;
using Shared.Domain.Events.Auditoria;

namespace AtronAuditoria.Application.EventHandlers;

public class AuditoriaEventHandlers : 
    IEventHandler<AuditoriaRegistradaEvent>,
    IEventHandler<AuditoriaAtualizadaEvent>,
    IEventHandler<AuditoriaRemovidaEvent>
{
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IHistoricoRepository _historicoRepository;
    private readonly IUserAccessor _userAccessor;

    public AuditoriaEventHandlers(
        IAuditoriaRepository auditoriaRepository,
        IHistoricoRepository historicoRepository,
        IUserAccessor userAccessor)
    {
        _auditoriaRepository = auditoriaRepository;
        _historicoRepository = historicoRepository;
        _userAccessor = userAccessor;
    }

    public async Task ManipularAsync(AuditoriaRegistradaEvent @event, CancellationToken cancellationToken)
    {
        var auditoria = new Auditoria
        {
            CodigoRegistro = @event.CodigoRegistro,
            CriadoPor = _userAccessor.ObterLogadoUsuario(),
            DataCriacao = DateTime.Now,
            Contexto = @event.Contexto
        };

        await _auditoriaRepository.AdicionarAsync(auditoria);
        await RegistrarHistorico(@event.Contexto, @event.CodigoRegistro, @event.DescricaoHistorico);
    }

    public async Task ManipularAsync(AuditoriaAtualizadaEvent @event, CancellationToken cancellationToken)
    {
        var auditoria = await _auditoriaRepository.ObterPorContextoCodigoAsync(@event.Contexto, @event.CodigoRegistro);
        if (auditoria != null)
        {
            auditoria.AlteradoPor = _userAccessor.ObterLogadoUsuario();
            auditoria.DataAlteracao = DateTime.Now;

            await _auditoriaRepository.AtualizarAsync(auditoria);
            await RegistrarHistorico(@event.Contexto, @event.CodigoRegistro, @event.DescricaoHistorico);
        }
    }

    public async Task ManipularAsync(AuditoriaRemovidaEvent @event, CancellationToken cancellationToken)
    {
        var auditoria = await _auditoriaRepository.ObterPorContextoCodigoAsync(@event.Contexto, @event.CodigoRegistro);
        if (auditoria != null)
        {
            auditoria.RemovidoEm = DateTime.Now;
            auditoria.AlteradoPor = _userAccessor.ObterLogadoUsuario();
            auditoria.DataAlteracao = DateTime.Now;

            await _auditoriaRepository.AtualizarAsync(auditoria);
            await RegistrarHistorico(@event.Contexto, @event.CodigoRegistro, @event.DescricaoHistorico);
        }
    }

    private async Task RegistrarHistorico(string contexto, string codigoRegistro, string descricao)
    {
        if (!string.IsNullOrWhiteSpace(descricao))
        {
            var historico = new Historico
            {
                Contexto = contexto,
                CodigoRegistro = codigoRegistro,
                Descricao = descricao,
                DataCriacao = DateTime.Now
            };
            await _historicoRepository.AdicionarAsync(historico);
        }
    }
}
