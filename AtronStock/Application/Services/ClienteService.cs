using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.Services
{
    public class ClienteService : IClienteService
    {
        private const string ClienteContexto = nameof(Cliente);

        private readonly IValidador<ClienteRequest> _validador;
        private readonly IAsyncMap<ClienteRequest, Cliente> _mapService;
        private readonly IClienteRepository _repository;
        private readonly IAuditoriaService _auditoriaService;

        public ClienteService(
            IClienteRepository repository,
            IValidador<ClienteRequest> validador,
            IAsyncMap<ClienteRequest, Cliente> mapService,
            IAuditoriaService auditoriaService)
        {
            _repository = repository;
            _validador = validador;
            _mapService = mapService;
            _auditoriaService = auditoriaService;
        }

        public async Task<Resultado> CriarAsync(ClienteRequest request)
        {
            var messages = _validador.Validar(request);
            if (!messages.TemErros())
            {
                var cliente = await _mapService.MapToEntityAsync(request);
                var foiSalvo = await _repository.CriarClienteAsync(cliente);

                if (!foiSalvo)
                {
                    return Resultado.Falha(ClienteResource.ErroInesperadoCliente);
                }

                IAuditoriaDTO auditoriaDTO = new AuditoriaDTO()
                {
                    CodigoRegistro = cliente.Codigo,
                    Contexto = ClienteContexto,
                    Historico = new HistoricoDTO()
                    {
                        CodigoRegistro = cliente.Codigo,
                        Contexto = ClienteContexto,
                        Descricao = string.Format(
                        ClienteResource.HistoricoClienteGravado,
                        cliente.Codigo,
                        cliente.Nome, DateTime.Now)
                    }
                };

                await _auditoriaService.RegistrarServiceAsync(auditoriaDTO);

                var context = new NotificationBag();
                context.MensagemRegistroSalvo(ClienteContexto);
                return Resultado.Sucesso(request, [.. context.Messages]);
            }

            return Resultado.Falha(messages);
        }

        public async Task<Resultado> AtualizarClienteServiceAsync(ClienteRequest request)
        {
            var messages = _validador.Validar(request);

            if (messages.HasErrors())
            {
                return Resultado.Falha(messages);
            }

            var cliente = await _repository.ObterClientePorCodigoAsync(request.Codigo);

            if (cliente == null)
            {
                return Resultado.Falha(ClienteResource.ErroClienteNaoExiste);
            }

            await _mapService.MapToEntityAsync(request, cliente);

            var foiAtualizado = await _repository.AtualizarClienteAsync(cliente);

            if (!foiAtualizado)
            {
                return Resultado.Falha(ClienteResource.ErroInesperadoCliente);
            }

            IAuditoriaDTO auditoria = new AuditoriaDTO()
            {
                CodigoRegistro = cliente.Codigo,
                Contexto = ClienteContexto,
                Historico = new HistoricoDTO()
                {
                    CodigoRegistro = cliente.Codigo,
                    Contexto = ClienteContexto,
                    Descricao = string.Format(
                    ClienteResource.ClienteAtualizadoDescricao,
                    cliente.Codigo,
                    DateTime.Now.Date)
                },
            };

            await _auditoriaService.RegistrarServiceAsync(auditoria);

            var context = new NotificationBag();
            context.MensagemRegistroAtualizado(ClienteContexto);
            return Resultado.Sucesso([.. context.Messages]);
        }

        public async Task<Resultado<ClienteRequest>> ObterClientePorCodigoServiceAsync(string codigo)
        {
            var cliente = await _repository.ObterClientePorCodigoAsync(codigo);
            if (cliente == null)
                return (Resultado<ClienteRequest>)Resultado.Falha(ClienteResource.ErroClienteNaoExiste);

            var clienteRequest = await _mapService.MapToDTOAsync(cliente);

            return Resultado.Sucesso(clienteRequest);
        }

        public async Task<Resultado<ICollection<ClienteRequest>>> ObterTodosClientesServiceAsync()
        {
            var clientes = await _repository.ObterTodoClientesAsync();
            var clientesRequest = await _mapService.MapToListDTOAsync(clientes);
            return Resultado.Sucesso<ICollection<ClienteRequest>>([.. clientesRequest]);
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            var cliente = await _repository.ObterClientePorCodigoAsync(codigo);

            if (cliente == null)
            {
                var messsageBag = new NotificationBag();
                messsageBag.MensagemRegistroNaoEncontrado(codigo);
                return Resultado.Falha([.. messsageBag.Messages]);
            }

            cliente.Status = EStatus.Removido;
            var foiRemovido = await _repository.AtualizarClienteAsync(cliente);

            if (!foiRemovido)
            {
                return Resultado.Falha(ClienteResource.ErroInesperadoAoRemover);
            }

            IAuditoriaDTO auditoria = new AuditoriaDTO()
            {
                CodigoRegistro = cliente.Codigo,
                Contexto = ClienteContexto,
                Historico = new HistoricoDTO()
                {
                    CodigoRegistro = cliente.Codigo,
                    Contexto = ClienteContexto,
                    Descricao = string.Format(ClienteResource.HistoricoClienteRemovido)
                },
            };

            await _auditoriaService.RemoverServiceAsync(auditoria);
            var notificationBag = new NotificationBag();
            notificationBag.MensagemRegistroRemovido(codigo);
            return Resultado.Sucesso([.. notificationBag.Messages]);
        }

        public async Task<Resultado<ICollection<ClienteRequest>>> ObterTodosClientesInativoServiceAsync()
        {
            var clientes = await _repository.ObterTodoClientesInativosAsync();
            var clientesRequest = await _mapService.MapToListDTOAsync(clientes);
            return Resultado.Sucesso<ICollection<ClienteRequest>>(clientesRequest);
        }

        public async Task<Resultado> AtivarInativarClienteServiceAsync(string codigo, bool ativar)
        {
            var cliente = await _repository.ObterClientePorCodigoAsync(codigo);
            if (cliente == null)
            {
                var messageBag = new NotificationBag();
                messageBag.MensagemRegistroNaoEncontrado(codigo);
                return Resultado.Falha([.. messageBag.Messages]);
            }

            cliente.Status = ativar ? EStatus.Ativo : EStatus.Inativo;
            var atualizado = await _repository.AtualizarClienteAsync(cliente);

            if (!atualizado)
            {
                return Resultado.Falha(ClienteResource.ErroInesperadoCliente);
            }

            IAuditoriaDTO auditoria = new AuditoriaDTO()
            {
                CodigoRegistro = cliente.Codigo,
                Contexto = ClienteContexto,
                Historico = new HistoricoDTO()
                {
                    CodigoRegistro = cliente.Codigo,
                    Contexto = ClienteContexto,
                    Descricao = string.Format(
                        ClienteResource.HistoricoClienteAtivado,
                        cliente.Codigo,
                        cliente.Status.GetDescription(),
                        DateTime.Now.Date)
                },
            };

            await _auditoriaService.RemoverServiceAsync(auditoria);

            var notificationBag = new NotificationBag();
            notificationBag.MensagemRegistroAtualizado(codigo);
            return Resultado.Sucesso([.. notificationBag.Messages]);
        }
    }
}
