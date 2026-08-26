using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Application.Resources;
using AtronStock.Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.UseCases.ProdutoCases
{
    public sealed class AtualizarProdutoCase(
        IProdutoRepository repository,
        IValidador<ProdutoAtualizacaoRequest> validador,
        ProdutoMapping mapper,
        SelecionarCategoriasProdutoCase selecionarCategorias,
        AuditoriaProdutoCase auditoriaProduto)
    {
        private readonly IProdutoRepository _repository = repository;
        private readonly IValidador<ProdutoAtualizacaoRequest> _validador = validador;
        private readonly ProdutoMapping _mapper = mapper;
        private readonly SelecionarCategoriasProdutoCase _selecionarCategorias = selecionarCategorias;
        private readonly AuditoriaProdutoCase _auditoriaProduto = auditoriaProduto;

        public async Task<Resultado> ExecutarAsync(string codigo, ProdutoAtualizacaoRequest request)
        {
            var mensagens = _validador.Validar(request).ToList();
            if (mensagens.TemErros())
                return Resultado.Falha(mensagens);

            var produto = await _repository.ObterPorCodigoAsync(codigo.NormalizarCodigo());
            if (produto is null)
            {
                return Resultado.Falha(string.Format(ProdutoResource.ErroProdutoNaoEncontrado, codigo));
            }

            var categorias = await _selecionarCategorias.ExecutarAsync(request.CategoriaCodigos);
            if (categorias.TeveFalha)
                return Resultado.Falha(categorias.Messages);

            produto.MapToUpdate(new ProdutoAtualizacaoMappingInput(request, categorias.Dados!), _mapper);

            if (!await _repository.AtualizarAsync(produto))
                return Resultado.Falha(ProdutoResource.ErroInesperadoAtualizar);

            await _auditoriaProduto.RegistrarAtualizacaoAsync(produto);
            return Resultado.Sucesso().ComMensagemRegistroAtualizado(produto.Codigo);
        }
    }
}