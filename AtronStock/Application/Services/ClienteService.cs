using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using Shared.Infrastructure.Repositories;

namespace AtronStock.Application.Services
{
    public class ClienteService : IClienteService
    {
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
                    return Resultado.Falha("Ocorreu um erro inesperado ao salvar o cliente.");
                }

                var historicoDescricao = $"Cliente gravado com o código {cliente.Codigo} e nome {cliente.Nome} na data {DateTime.Now}";
                await _auditoriaService.RegistrarAuditoriaAsync(cliente.Codigo, null, historicoDescricao);

                var context = new NotificationBag();
                context.MensagemRegistroSalvo("Cliente");
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
                return Resultado.Falha("Cliente não existe.");
            }

            await _mapService.MapToEntityAsync(request, cliente);

            var foiAtualizado = await _repository.AtualizarClienteAsync(cliente);

            if (!foiAtualizado)
            {
                return Resultado.Falha("Ocorreu um erro inesperado ao atualizar o cliente.");
            }

            var historicoDescricao = $"Cliente {cliente.Codigo} atualizado na data {DateTime.Now}";
            await _auditoriaService.RegistrarAlteracaoAuditoriaAsync(cliente.Codigo, null, historicoDescricao);

            var context = new NotificationBag();
            context.MensagemRegistroAtualizado("Cliente");
            return Resultado.Sucesso([.. context.Messages]);
        }

        public async Task<Resultado<ClienteRequest>> ObterClientePorCodigoServiceAsync(string codigo)
        {
            var cliente = await _repository.ObterClientePorCodigoAsync(codigo);
            if (cliente == null)
                return (Resultado<ClienteRequest>)Resultado.Falha("Cliente não existe.");

            var clienteRequest = await _mapService.MapToDTOAsync(cliente);

            return Resultado.Sucesso(clienteRequest);
        }

        public async Task<Resultado<ICollection<ClienteRequest>>> ObterTodosClientesServiceAsync()
        {
            var clientes = await _repository.ObterTodoClientesAsync();
            var clientesRequest = await _mapService.MapToListDTOAsync(clientes);
            return Resultado.Sucesso<ICollection<ClienteRequest>>(clientesRequest.ToList());
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
                return Resultado.Falha("Ocorreu um erro inesperado ao remover o cliente.");
            }

            var historicoDescricao = $"Cliente {codigo} removido na data {DateTime.Now}";

            await _auditoriaService.RegistrarRemocaoAsync(codigo, null, historicoDescricao);
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
                return Resultado.Falha("Ocorreu um erro inesperado ao remover o cliente.");
            }

            string ativado = ativar ? "ativado" : "desativado";
            var historicoDescricao = $"Cliente {codigo} foi {ativado} na data {DateTime.Now}";

            await _auditoriaService.RegistrarAlteracaoAuditoriaAsync(codigo, null, historicoDescricao);

            var notificationBag = new NotificationBag();
            notificationBag.MensagemRegistroAtualizado(codigo);
            return Resultado.Sucesso([.. notificationBag.Messages]);
        }
    }
}
