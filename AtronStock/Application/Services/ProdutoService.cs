using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources.AtronStock;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private const string ProdutoContexto = nameof(Produto);
        private readonly IProdutoRepository _produtoRepository;
        private readonly IEstoqueService _estoqueService;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IAsyncMap<ProdutoRequest, Produto> _map;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IValidador<ProdutoRequest> _validador;

        public ProdutoService(IProdutoRepository produtoRepository,
                              IEstoqueService estoqueService,
                              IAsyncMap<ProdutoRequest, Produto> map,
                              IAuditoriaService auditoriaService,
                              IValidador<ProdutoRequest> validador,
                              IFornecedorRepository fornecedorRepository)
        {
            _validador = validador;
            _map = map;
            _produtoRepository = produtoRepository;
            _estoqueService = estoqueService;
            _auditoriaService = auditoriaService;
            _fornecedorRepository = fornecedorRepository;
        }

        private async Task PreencherInformacoesDaEntidade()
        {

        }

        public async Task<Resultado> RegistrarProdutoAsync(ProdutoRequest request)
        {
            var messages = _validador.Validar(request);
            if (messages.Any())
            {
                return Resultado.Falha(messages);
            }

            Produto produtoExistente = await _produtoRepository.ObterPorCodigoAsync(request.Codigo);
            if (produtoExistente != null)
            {
                return Resultado.Falha(string.Format(ProdutoResource.ErroProdutoExistente, request.Codigo));
            }

            //1. Fluxo de Produto
            Produto produto = await _map.MapToEntityAsync(request);

            // A entidade de produto deve realizar todo o processo pelo EF Core
            bool foiSalvo = await _produtoRepository.AdicionarAsync(produto);
            if (!foiSalvo)
            {
                return Resultado.Falha(string.Format(ProdutoResource.ErroInesperadoDeGravacao, produto.Codigo));
            }

            IAuditoriaDTO auditoria = new AuditoriaDTO()
            {
                CodigoRegistro = produto.Codigo,
                Contexto = ProdutoContexto,
                Historico = new HistoricoDTO()
                {
                    CodigoRegistro = produto.Codigo,
                    Contexto = ProdutoContexto,                 
                    Descricao = string.Format(ProdutoResource.MensagemProdutoCriado, produto.Codigo, DateTime.Now.Date)
                }
            };

            await _auditoriaService.RegistrarServiceAsync(auditoria);
            var notificationBag = new NotificationBag();
            notificationBag.MensagemRegistroSalvo(ProdutoContexto);
            return Resultado.Sucesso(request, [.. notificationBag.Messages]);
        }

        public async Task RegistrarProdutosEmLoteAsync(Produto produtoBase, int quantidade)
        {
            for (int i = 1; i <= quantidade; i++)
            {
                var novoProduto = new Produto
                {
                    Codigo = $"{produtoBase.Codigo}{i}",
                    Descricao = produtoBase.Descricao,
                    Categorias = new List<ProdutoCategoria>()
                };

                if (produtoBase.Categorias != null)
                {
                    foreach (var cat in produtoBase.Categorias)
                    {
                        novoProduto.Categorias.Add(new ProdutoCategoria
                        {
                            CategoriaId = cat.CategoriaId,
                            CategoriaCodigo = cat.CategoriaCodigo,
                            ProdutoCodigo = novoProduto.Codigo
                        });
                    }
                }

                await _produtoRepository.AdicionarAsync(novoProduto);
            }
        }

        public async Task InativarProdutoAsync(int produtoId)
        {
            var estoque = new Estoque();
            if (estoque != null && estoque.Quantidade > 0)
            {
                throw new InvalidOperationException($"Não é possível inativar o produto ID {produtoId} pois há {estoque.Quantidade} itens em estoque.");
            }

            var produto = await _produtoRepository.ObterPorIdAsync(produtoId);
            if (produto == null) throw new InvalidOperationException($"Produto ID {produtoId} não encontrado.");

            //produto.Removido = true;
            //produto.RemovidoEm = DateTime.Now;

            await _produtoRepository.AtualizarAsync(produto);
        }

        public async Task InativarProdutosEmLoteAsync(List<int> produtoIds)
        {
            foreach (var id in produtoIds)
            {
                await InativarProdutoAsync(id);
            }
        }

        public async Task<Resultado<ICollection<Produto>>> ObterTodos()
        {
            var produtos = await _produtoRepository.ObterTodos();

            return Resultado<ICollection<Produto>>.Sucesso(produtos);
        }
    }
}
