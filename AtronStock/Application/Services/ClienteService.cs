using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IValidador<ClienteRequest> _validador;
        private readonly IAsyncMap<ClienteRequest, Cliente> _mapService;
        private readonly IClienteRepository _repository;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IHistoricoService _historicoService;

        public ClienteService(
            IClienteRepository repository,
            IValidador<ClienteRequest> validador,
            IAsyncMap<ClienteRequest, Cliente> mapService,
            IAuditoriaService auditoriaService,
            IHistoricoService historicoService)
        {
            _repository = repository;
            _validador = validador;
            _mapService = mapService;
            _auditoriaService = auditoriaService;
            _historicoService = historicoService;
        }

        public async Task<Resultado> CriarAsync(ClienteRequest request)
        {
            var messages = _validador.Validar(request);
            if (messages.TemErros() is not true)
            {
                var cliente = await _mapService.MapToEntityAsync(request);
                var foiSalvo = await _repository.CriarClienteAsync(cliente);

                if (foiSalvo is not true)
                {
                    return Resultado.Falha("Ocorreu um erro inesperado ao salvar o cliente.");
                }

                var historicoDescricao = $"Cliente gravado com o código {cliente.Codigo} e nome {cliente.Nome} na data {DateTime.Now}";
                await _auditoriaService.RegistrarAuditoriaAsync(cliente.Codigo, null, historicoDescricao);

                var context = new NotificationBag();
                context.MensagemRegistroSalvo("Cliente");
                return Resultado.Sucesso(request, context.Messages.ToList());
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
            return Resultado.Sucesso(context.Messages.ToList());
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
                var contextNaoEncontrado = new NotificationBag();
                contextNaoEncontrado.MensagemRegistroNaoEncontrado(codigo);
                return Resultado.Falha(contextNaoEncontrado.Messages.ToList());
            }

            cliente.Status = EStatus.Inativo;
            var foiRemovido = await _repository.RemoverClienteAsync(cliente);

            if (!foiRemovido)
                return Resultado.Falha("Ocorreu um erro inesperado ao remover o cliente.");

            var historicoDescricao = $"Cliente {codigo} desativado na data {DateTime.Now}";

            await _auditoriaService.RegistrarRemocaoAsync(codigo, null, historicoDescricao);
            var contextSucesso = new NotificationBag();
            contextSucesso.MensagemRegistroRemovido(codigo);
            return Resultado.Sucesso(contextSucesso.Messages.ToList());
        }


        public async Task<Resultado<ICollection<ClienteRequest>>> ObterTodosClientesInativoServiceAsync()
        {
            var clientes = await _repository.ObterTodoClientesAsync();
            var clientesRequest = await _mapService.MapToListDTOAsync(clientes);          
            return Resultado.Sucesso<ICollection<ClienteRequest>>(clientesRequest.Where(x => x.StatusPessoa == EStatus.Inativo).ToList());
        }
    }
}
