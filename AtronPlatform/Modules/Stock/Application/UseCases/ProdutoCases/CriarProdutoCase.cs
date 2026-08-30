using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Application.Resources;
using AtronStock.Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.UseCases.ProdutoCases
{
    public sealed class CriarProdutoCase(
        IProdutoRepository repository,
        IValidador<ProdutoRequest> validador,
        ProdutoMapping mapper,
        SelecionarCategoriasProdutoCase selecionarCategorias,
        AuditoriaProdutoCase auditoriaProduto)
    {
        private readonly IProdutoRepository _repository = repository;
        private readonly IValidador<ProdutoRequest> _validador = validador;
        private readonly ProdutoMapping _mapper = mapper;
        private readonly SelecionarCategoriasProdutoCase _selecionarCategorias = selecionarCategorias;
        private readonly AuditoriaProdutoCase _auditoriaProduto = auditoriaProduto;

        public async Task<Resultado> ExecutarAsync(ProdutoRequest request)
        {
            var mensagens = _validador.Validar(request).ToList();
            if (mensagens.TemErros())
                return Resultado.Falha(mensagens);

            if (await _repository.ObterPorCodigoAsync(request.Codigo) is not null)
            {
                return Resultado.Falha(string.Format(
                    ProdutoResource.ErroProdutoExistente,
                    request.Codigo));
            }

            var categorias = await _selecionarCategorias.ExecutarAsync(request.CategoriaCodigos);
            if (categorias.TeveFalha)
                return Resultado.Falha(categorias.Messages);

            var produto = _mapper.MapToEntity(new(request, categorias.Dados!));

            if (!await _repository.AdicionarAsync(produto))
            {
                return Resultado.Falha(string.Format(
                    ProdutoResource.ErroInesperadoDeGravacao,
                    request.Codigo));
            }

            await _auditoriaProduto.RegistrarCriacaoAsync(produto);
            
            return Resultado.Sucesso().CommMensagemRegistroSalvo(ProdutoResource.SucessoCadastro);
        }
    }
}
