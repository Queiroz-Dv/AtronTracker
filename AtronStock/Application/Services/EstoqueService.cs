using AtronStock.Application.Interfaces;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;

namespace AtronStock.Application.Services
{
    public class EstoqueService : IEstoqueService
    {
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IClienteRepository _clienteRepository;

        public EstoqueService(
            IEstoqueRepository estoqueRepository,
            IAuditoriaService auditoriaService,
            IFornecedorRepository fornecedorRepository,
            IProdutoRepository produtoRepository,
            IClienteRepository clienteRepository)
        {
            _estoqueRepository = estoqueRepository;
            _auditoriaService = auditoriaService;
            _fornecedorRepository = fornecedorRepository;
            _produtoRepository = produtoRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task ProcessarEntradaAsync(Entrada entrada)
        {
            if (entrada.Itens == null || !entrada.Itens.Any())
                throw new ArgumentException("A entrada deve conter pelo menos um item.");

            // 1. Fetch Fornecedor to get Code
            var fornecedor = await _fornecedorRepository.ObterPorIdAsync(entrada.FornecedorId);
            if (fornecedor == null) throw new InvalidOperationException("Fornecedor não encontrado.");
            entrada.FornecedorCodigo = fornecedor.Codigo;

            // 2. Fetch Products to get Codes and populate ItemEntrada
            foreach (var item in entrada.Itens)
            {
                var prod = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                if (prod == null) throw new InvalidOperationException($"Produto ID {item.ProdutoId} não encontrado.");

                item.ProdutoCodigo = prod.Codigo;
            }

            // 3. Registrar a Entrada
            await _estoqueRepository.RegistrarEntradaCompletaAsync(entrada);

            // 4. Audit
            await RegistrarAuditoriaAsync(entrada.FornecedorCodigo, "Entrada", $"Entrada {entrada.Id} registrada para Fornecedor {fornecedor.Nome}");

            // 5. Atualizar Estoque
            foreach (var item in entrada.Itens)
            {
                var estoque = await _estoqueRepository.ObterPorProdutoIdAsync(item.ProdutoId);

                if (estoque == null)
                {
                    estoque = new Estoque
                    {
                        ProdutoId = item.ProdutoId,
                        Quantidade = 0,
                        DataUltimaAtualizacao = DateTime.Now
                    };
                    await _estoqueRepository.AdicionarAsync(estoque);
                }

                estoque.Quantidade += item.Quantidade;
                estoque.DataUltimaAtualizacao = DateTime.Now;

                await _estoqueRepository.AtualizarAsync(estoque);

                var movimentacao = new MovimentacaoEstoque
                {
                    EstoqueId = estoque.Id,
                    TipoMovimentacao = TipoMovimentacao.Entrada,
                    Quantidade = item.Quantidade,
                    DataMovimentacao = DateTime.Now,
                    Observacao = $"Entrada ID: {entrada.Id}",
                    Origem = $"Fornecedor: {fornecedor.Nome} ({fornecedor.Codigo})",
                    TransacaoId = entrada.Id
                };

                await _estoqueRepository.AdicionarMovimentacaoAsync(movimentacao);
            }
        }

        public async Task ProcessarVendaAsync(Venda venda)
        {
            if (venda.Itens == null || !venda.Itens.Any())
                throw new ArgumentException("A venda deve conter pelo menos um item.");

            // 1. Fetch Cliente to get Code
            var cliente = await _clienteRepository.ObterPorIdAsync(venda.ClienteId);
            if (cliente == null) throw new InvalidOperationException("Cliente não encontrado.");
            venda.ClienteCodigo = cliente.Codigo;

            // 2. Validate Stock and Populate Codes
            foreach (var item in venda.Itens)
            {
                var prod = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                if (prod == null) throw new InvalidOperationException($"Produto ID {item.ProdutoId} não encontrado.");
                item.ProdutoCodigo = prod.Codigo;

                var estoque = await _estoqueRepository.ObterPorProdutoIdAsync(item.ProdutoId);
                if (estoque == null || estoque.Quantidade < item.Quantidade)
                {
                    throw new InvalidOperationException($"Estoque insuficiente para o produto {prod.Descricao} ({prod.Codigo})");
                }
            }

            // 3. Registrar a Venda
            await _estoqueRepository.RegistrarVendaCompletaAsync(venda);

            // 4. Audit
            await RegistrarAuditoriaAsync(venda.ClienteCodigo, "Venda", $"Venda {venda.Id} registrada para Cliente {cliente.Nome}");

            // 5. Update Stock
            foreach (var item in venda.Itens)
            {
                var estoque = await _estoqueRepository.ObterPorProdutoIdAsync(item.ProdutoId);
                if (estoque != null)
                {
                    estoque.Quantidade -= item.Quantidade;
                    estoque.DataUltimaAtualizacao = DateTime.Now;
                    await _estoqueRepository.AtualizarAsync(estoque);

                    var movimentacao = new MovimentacaoEstoque
                    {
                        EstoqueId = estoque.Id,
                        TipoMovimentacao = TipoMovimentacao.Saida,
                        Quantidade = item.Quantidade,
                        DataMovimentacao = DateTime.Now,
                        Observacao = $"Venda ID: {venda.Id}",
                        Origem = $"Cliente: {cliente.Nome} ({cliente.Codigo})",
                        TransacaoId = venda.Id
                    };

                    await _estoqueRepository.AdicionarMovimentacaoAsync(movimentacao);
                }
            }
        }

        private async Task RegistrarAuditoriaAsync(string codigoRegistro, string contexto, string descricao)
        {
            var auditoria = new AuditoriaDTO
            {
                CodigoRegistro = codigoRegistro,
                Contexto = contexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = codigoRegistro,
                    Contexto = contexto,
                    Descricao = descricao
                }
            };
            await _auditoriaService.RegistrarServiceAsync(auditoria);
        }
    }
}
