using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources.AtronStock;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private const string CategoriaContexto = nameof(Categoria);

        private readonly ICategoriaRepository _repository;
        private readonly IValidador<CategoriaRequest> _validador;
        private readonly IAsyncMap<CategoriaRequest, Categoria> _mapService;
        private readonly IAuditoriaService _auditoriaService;

        public CategoriaService(
            ICategoriaRepository repository,
            IValidador<CategoriaRequest> validador,
            IAsyncMap<CategoriaRequest, Categoria> mapService,
            IAuditoriaService auditoriaService)
        {
            _repository = repository;
            _validador = validador;
            _mapService = mapService;
            _auditoriaService = auditoriaService;
        }

        public async Task<Resultado> CriarAsync(CategoriaRequest dto)
        {
            var messages = _validador.Validar(dto);
            if (messages.Any()) return Resultado.Falha(messages);

            var categoriaExistente = await _repository.ObterCategoriaPorCodigoAsync(dto.Codigo);
            if (categoriaExistente != null)
            {
                return Resultado.Falha(string.Format(CategoriaResource.ErroCategoriaJaExiste, dto.Codigo));
            }

            var categoria = await _mapService.MapToEntityAsync(dto);
            await _repository.CriarCategoriaAsync(categoria);

            IAuditoriaDTO auditoria = new AuditoriaDTO
            {
                CodigoRegistro = categoria.Codigo,
                Contexto = CategoriaContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = categoria.Codigo,
                    Contexto = CategoriaContexto,
                    Descricao = string.Format(CategoriaResource.HistoricoCriacao, categoria.Codigo, DateTime.Now)
                }
            };

            await _auditoriaService.RegistrarServiceAsync(auditoria);

            var context = new NotificationBag();
            context.MensagemRegistroSalvo(CategoriaResource.SucessoCadastro);
            return Resultado.Sucesso(dto, [.. context.Messages]);
        }

        public async Task<Resultado> AtualizarAsync(CategoriaRequest dto)
        {
            var messages = _validador.Validar(dto);
            if (messages.Any()) return Resultado.Falha(messages);

            var categoria = await _repository.ObterCategoriaPorCodigoAsync(dto.Codigo);
            if (categoria == null)
            {
                return Resultado.Falha(string.Format(CategoriaResource.ErroCategoriaNaoEncontrada, dto.Codigo));
            }

            await _mapService.MapToEntityAsync(dto, categoria);

            var atualizado = await _repository.AtualizarCategoriaAsync(categoria);
            if (!atualizado)
            {
                return Resultado.Falha(CategoriaResource.ErroInesperadoAtualizar);
            }
            
            IAuditoriaDTO auditoria = new AuditoriaDTO
            {
                CodigoRegistro = categoria.Codigo,
                Contexto = CategoriaContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = categoria.Codigo,
                    Contexto = CategoriaContexto,
                    Descricao = string.Format(
                        CategoriaResource.HistoricoAtualizacao,
                        categoria.Codigo,
                        DateTime.Now,
                        dto.Descricao,
                        dto.Status.GetDescription())
                }
            };

            await _auditoriaService.AtualizarServiceAsync(auditoria);

            var context = new NotificationBag();
            context.MensagemRegistroAtualizado(CategoriaContexto);
            return Resultado.Sucesso(dto, [.. context.Messages]);
        }

        public async Task<Resultado> AtivarInativarAsync(string codigo, bool ativar)
        {
            var categoria = await _repository.ObterCategoriaPorCodigoAsync(codigo);
            if (categoria == null)
            {
                var bag = new NotificationBag();      
                bag.MensagemRegistroNaoEncontrado(codigo);
                return Resultado.Falha(bag.Messages.ToList());
            }

            categoria.Status = ativar ? EStatus.Ativo : EStatus.Inativo;
            await _repository.AtualizarCategoriaAsync(categoria);
            
            IAuditoriaDTO auditoria = new AuditoriaDTO
            {
                CodigoRegistro = categoria.Codigo,
                Contexto = CategoriaContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = categoria.Codigo,
                    Contexto = CategoriaContexto,
                    Descricao = string.Format(
                        CategoriaResource.HistoricoStatusAlterado,
                        categoria.Codigo,
                        categoria.Status.GetDescription(),
                        DateTime.Now)
                }
            };

            await _auditoriaService.AtualizarServiceAsync(auditoria);

            var context = new NotificationBag();
            context.MensagemRegistroAtualizado(codigo);
            return Resultado.Sucesso(categoria, [.. context.Messages]);
        }

        public async Task<Resultado<ICollection<CategoriaRequest>>> ObterTodasAsync()
        {
            var categorias = await _repository.ObterTodasCategoriasAsync();
            var dtos = await _mapService.MapToListDTOAsync(categorias);
            return Resultado.Sucesso<ICollection<CategoriaRequest>>(dtos);
        }

        public async Task<Resultado<ICollection<CategoriaRequest>>> ObterInativasAsync()
        {
            var categorias = await _repository.ObterTodasCategoriasInativasAsync();
            var dtos = await _mapService.MapToListDTOAsync(categorias);
            return Resultado.Sucesso<ICollection<CategoriaRequest>>(dtos);
        }

        public async Task<Resultado<CategoriaRequest>> ObterPorCodigoAsync(string codigo)
        {
            var categoria = await _repository.ObterCategoriaPorCodigoAsync(codigo);
            if (categoria == null)
            {
                var bag = new NotificationBag();
                bag.MensagemRegistroNaoEncontrado(codigo);
                return Resultado.Falha<CategoriaRequest>(bag.Messages.ToList());
            }

            var dto = await _mapService.MapToDTOAsync(categoria);
            return Resultado.Sucesso(dto);
        }
    }
}