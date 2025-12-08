using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources.AtronStock;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Services
{
    public class FornecedorService : IFornecedorService
    {
        private readonly IValidador<FornecedorRequest> _validador;
        private readonly IAsyncMap<FornecedorRequest, Fornecedor> _map;
        private readonly IFornecedorRepository _fornecedorRepository;

        public FornecedorService(IValidador<FornecedorRequest> validador,
                                 IAsyncMap<FornecedorRequest, Fornecedor> map,
                                 IFornecedorRepository fornecedorRepository)
        {
            _validador = validador;
            _map = map;
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task<Resultado> RegistrarFornecedorAsync(FornecedorRequest request)
        {
            var messages = _validador.Validar(request);
            if (messages.Any())
            {
                return Resultado.Falha(messages);
            }

            Fornecedor fornecedorExistente = await _fornecedorRepository.ObterPorCodigoAsync(request.Codigo);
            if (fornecedorExistente != null)
            {
                return Resultado.Falha(FornecedoResource.ErroCodigoFornecedorExistente);
            }

            Fornecedor fornecedor = await _map.MapToEntityAsync(request);
            bool foiSalvo = await _fornecedorRepository.AdicionarAsync(fornecedor);
            if (!foiSalvo)
            {
                return Resultado.Sucesso();
            }

            var notificationBag = new NotificationBag();
            notificationBag.MensagemRegistroSalvo(nameof(Fornecedor));
            return Resultado.Sucesso(request, [.. notificationBag.Messages]);
        }

        public async Task<ICollection<Fornecedor>> ListarFornecedoresAsync()
        {
            return await _fornecedorRepository.ListarTodosAsync();
        }

        public async Task<Resultado<FornecedorRequest>> ObterFornecedorPorCodigoAsync(string codigo)
        {
            var fornecedor = await _fornecedorRepository.ObterPorCodigoAsync(codigo);
            if (fornecedor == null)
            {
                return Resultado.Falha<FornecedorRequest>("Fornecedor não encontrado.");
            }

            var fornecedorRequest = new FornecedorRequest
            {
                Codigo = fornecedor.Codigo,
                Nome = fornecedor.Nome,
                Email = fornecedor.Email,
                Telefone = fornecedor.Telefone
            };
            return Resultado.Sucesso(fornecedorRequest);
        }
    }
}
