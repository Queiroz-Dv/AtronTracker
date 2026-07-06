using Application.DTO;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class TarefaService : ITarefaService
    {
        private readonly IAsyncApplicationMapService<TarefaDTO, Tarefa> _map;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ISolicitacaoObtencaoTarefaRepository _solicitacaoObtencaoTarefaRepository;
        private readonly INotificacaoInternaService _notificacaoInternaService;
        private readonly ITarefaPreparacaoService _tarefaPreparacaoService;
        private readonly ITarefaNotificacaoService _tarefaNotificacaoService;
        private readonly Domain.Interfaces.UsuarioInterfaces.IUsuarioRepository _usuarioRepository;
        private readonly IUserAccessor _userAccessor;
        private const int EstadoFinalizadaId = 4;

        public TarefaService(
            IAsyncApplicationMapService<TarefaDTO, Tarefa> map,
            ITarefaRepository tarefaRepository,
            ISolicitacaoObtencaoTarefaRepository solicitacaoObtencaoTarefaRepository,
            INotificacaoInternaService notificacaoInternaService,
            ITarefaPreparacaoService tarefaPreparacaoService,
            ITarefaNotificacaoService tarefaNotificacaoService,
            Domain.Interfaces.UsuarioInterfaces.IUsuarioRepository usuarioRepository,
            IUserAccessor userAccessor)
        {
            _map = map;
            _tarefaRepository = tarefaRepository;
            _solicitacaoObtencaoTarefaRepository = solicitacaoObtencaoTarefaRepository;
            _notificacaoInternaService = notificacaoInternaService;
            _tarefaPreparacaoService = tarefaPreparacaoService;
            _tarefaNotificacaoService = tarefaNotificacaoService;
            _usuarioRepository = usuarioRepository;
            _userAccessor = userAccessor;
        }

        public async Task<Resultado<TarefaDTO>> CriarAsync(TarefaDTO tarefaDTO)
        {
            var preparacao = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacao.Messages);

            var gravado = await _tarefaRepository.CriarTarefaAsync(preparacao.Dados.Entidade);
            if (!gravado)
                return Resultado<TarefaDTO>.Falha("Erro ao gravar a tarefa.");

            tarefaDTO.Id = preparacao.Dados.Entidade.Id;
            tarefaDTO.Identificador = preparacao.Dados.Entidade.Identificador;
            var resultado = Resultado<TarefaDTO>
                .Sucesso(tarefaDTO)
                .AdicionarMensagem("Tarefa gravada com sucesso.");

            var envioEmail = await _tarefaNotificacaoService.NotificarAtribuicaoAsync(tarefaDTO, preparacao.Dados.Usuario);
            if (envioEmail.TeveFalha)
            {
                resultado.AdicionarAviso("Tarefa criada, mas nao foi possivel enviar o e-mail de notificacao.");
            }

            return resultado;
        }

        public async Task<Resultado<List<TarefaDTO>>> ObterTodosAsync()
        {
            var tarefas = await _tarefaRepository.ObterTodasTarefas();
            var dtos = await _map.MapToListDTOAsync(tarefas);
            return Resultado<List<TarefaDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<List<TarefaDTO>>> ObterMeuQuadroAsync()
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<List<TarefaDTO>>.Falhas(usuario.Messages);

            var tarefas = await _tarefaRepository.ObterTarefasAtivasPorUsuarioAsync(
                usuario.Dados.Id,
                usuario.Dados.Codigo);

            var dtos = await _map.MapToListDTOAsync([.. tarefas]);
            return Resultado<List<TarefaDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<List<TarefaDTO>>> ObterEquipeAsync()
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<List<TarefaDTO>>.Falhas(usuario.Messages);

            var tarefas = await _tarefaRepository.ObterTarefasAtivasPorSubordinadosDiretosAsync(
                usuario.Dados.Id,
                usuario.Dados.Codigo);

            var dtos = await _map.MapToListDTOAsync([.. tarefas]);
            return Resultado<List<TarefaDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<List<TarefaDTO>>> ObterDisponiveisAsync()
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<List<TarefaDTO>>.Falhas(usuario.Messages);

            var departamentoIds = ObterDepartamentoIds(usuario.Dados);
            var cargoIds = ObterCargoIds(usuario.Dados);

            var tarefas = await _tarefaRepository.ObterTarefasAtivasDisponiveisParaUsuarioAsync(
                usuario.Dados.Id,
                usuario.Dados.Codigo,
                departamentoIds,
                cargoIds);

            var dtos = await _map.MapToListDTOAsync([.. tarefas]);
            return Resultado<List<TarefaDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<List<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoesAsync()
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<List<SolicitacaoObtencaoTarefaDTO>>.Falhas(usuario.Messages);

            var solicitacoes = await _solicitacaoObtencaoTarefaRepository.ObterPendentesPorAprovadorAsync(
                usuario.Dados.Id,
                usuario.Dados.Codigo);

            var dtos = new List<SolicitacaoObtencaoTarefaDTO>();
            foreach (var solicitacao in solicitacoes)
            {
                dtos.Add(await MapearSolicitacaoAsync(solicitacao));
            }

            return Resultado<List<SolicitacaoObtencaoTarefaDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync()
        {
            return await _tarefaPreparacaoService.ObterEstadosAsync();
        }

        public async Task<Resultado<TarefaDTO>> AtualizarAsync(int id, TarefaDTO tarefaDTO)
        {
            var tarefaExistente = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefaExistente is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var preparacao = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacao.Messages);

            var atualizado = await _tarefaRepository.AtualizarTarefaAsync(id, preparacao.Dados.Entidade);
            if (!atualizado)
                return Resultado<TarefaDTO>.Falha("Erro ao atualizar a tarefa.");

            tarefaDTO.Id = id;
            return Resultado<TarefaDTO>
                .Sucesso(tarefaDTO)
                .AdicionarMensagem("Tarefa atualizada com sucesso.");
        }

        public async Task<Resultado> ExcluirAsync(string id)
        {
            if (!int.TryParse(id, out var tarefaId))
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (tarefa is null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var deletado = await _tarefaRepository.RemoverRepositoryAsync(tarefa);
            if (!deletado)
                return Resultado.Falha("Erro ao remover a tarefa.");

            return Resultado
                .Sucesso()
                .AdicionarMensagem("Tarefa removida com sucesso.");
        }

        public async Task<Resultado<TarefaDTO>> AssumirAsync(int id)
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(usuario.Messages);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefa is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (tarefa.UsuarioId.HasValue)
                return Resultado<TarefaDTO>.Falha("Tarefa ja possui usuario responsavel.");

            if (tarefa.TarefaEstadoId == EstadoFinalizadaId)
                return Resultado<TarefaDTO>.Falha("Tarefa finalizada nao pode ser assumida.");

            if (!UsuarioPodeAssumir(usuario.Dados, tarefa))
                return Resultado<TarefaDTO>.Falha("Usuario nao possui acesso para assumir esta tarefa.");

            if (tarefa.ExigeAprovacaoParaObter)
                return Resultado<TarefaDTO>.Falha("Esta tarefa exige solicitacao de obtencao.");

            var assumida = await _tarefaRepository.AssumirTarefaAsync(id, usuario.Dados.Id, usuario.Dados.Codigo);
            if (!assumida)
                return Resultado<TarefaDTO>.Falha("Nao foi possivel assumir a tarefa.");

            var tarefaAtualizada = await _tarefaRepository.ObterTarefaPorId(id);
            var dto = await _map.MapToDTOAsync(tarefaAtualizada);
            return Resultado<TarefaDTO>
                .Sucesso(dto)
                .AdicionarMensagem("Tarefa assumida com sucesso.");
        }

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> SolicitarObtencaoAsync(int id)
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(usuario.Messages);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefa is null)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (tarefa.UsuarioId.HasValue)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha("Tarefa ja possui usuario responsavel.");

            if (tarefa.TarefaEstadoId == EstadoFinalizadaId)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha("Tarefa finalizada nao pode ser solicitada.");

            if (!tarefa.ExigeAprovacaoParaObter)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha("Esta tarefa nao exige aprovacao para obtencao.");

            if (!UsuarioPodeAssumir(usuario.Dados, tarefa))
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha("Usuario nao possui acesso para solicitar esta tarefa.");

            if (await _solicitacaoObtencaoTarefaRepository.ExisteSolicitacaoPendenteParaTarefaAsync(id))
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha("Ja existe solicitacao pendente para esta tarefa.");

            var aprovador = await ResolverAprovadorObtencaoAsync(usuario.Dados, tarefa);
            if (aprovador is null)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha("Nenhum aprovador disponivel para esta solicitacao.");

            var solicitacao = new SolicitacaoObtencaoTarefa
            {
                TarefaId = tarefa.Id,
                SolicitanteId = usuario.Dados.Id,
                SolicitanteCodigo = usuario.Dados.Codigo,
                AprovadorId = aprovador.Id,
                AprovadorCodigo = aprovador.Codigo,
                Status = (int)StatusSolicitacaoObtencaoTarefa.Pendente,
                DataSolicitacao = System.DateTime.Now
            };

            var gravada = await _solicitacaoObtencaoTarefaRepository.CriarAsync(solicitacao);
            if (!gravada)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha("Nao foi possivel criar a solicitacao.");

            var solicitacaoGravada = await _solicitacaoObtencaoTarefaRepository.ObterPorIdAsync(solicitacao.Id);
            var dto = await MapearSolicitacaoAsync(solicitacaoGravada);

            return Resultado<SolicitacaoObtencaoTarefaDTO>
                .Sucesso(dto)
                .AdicionarMensagem("Solicitacao enviada com sucesso.");
        }

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> AprovarSolicitacaoAsync(int id)
        {
            return await DecidirSolicitacaoAsync(id, aprovar: true);
        }

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> RecusarSolicitacaoAsync(int id)
        {
            return await DecidirSolicitacaoAsync(id, aprovar: false);
        }

        public async Task<Resultado<TarefaDTO>> ObterPorId(int id)
        {
            var tarefaRepository = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefaRepository is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dto = await _map.MapToDTOAsync(tarefaRepository);
            return Resultado<TarefaDTO>.Sucesso(dto);
        }

        private async Task<Resultado<Usuario>> ObterUsuarioLogadoAsync()
        {
            var usuarioCodigo = _userAccessor.ObterCodigoUsuarioLogado();
            if (usuarioCodigo.IsNullOrEmpty())
                return Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuarioCodigo);
            if (usuario is null)
                return Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            return Resultado<Usuario>.Sucesso(usuario);
        }

        private static IReadOnlyCollection<int> ObterDepartamentoIds(Usuario usuario)
        {
            return usuario.UsuarioCargoDepartamentos?
                .Select(rel => rel.DepartamentoId)
                .Distinct()
                .ToList() ?? [];
        }

        private static IReadOnlyCollection<int> ObterCargoIds(Usuario usuario)
        {
            return usuario.UsuarioCargoDepartamentos?
                .Select(rel => rel.CargoId)
                .Distinct()
                .ToList() ?? [];
        }

        private static bool UsuarioPodeAssumir(Usuario usuario, Tarefa tarefa)
        {
            if (!tarefa.DepartamentoId.HasValue)
            {
                return false;
            }

            if (tarefa.Departamento?.GestorDepartamentoId == usuario.Id &&
                tarefa.Departamento?.GestorDepartamentoCodigo == usuario.Codigo)
            {
                return true;
            }

            var departamentoIds = ObterDepartamentoIds(usuario);
            var cargoIds = ObterCargoIds(usuario);
            var estaNoDepartamento = departamentoIds.Contains(tarefa.DepartamentoId.Value);
            var cargoCompativel = !tarefa.CargoId.HasValue || cargoIds.Contains(tarefa.CargoId.Value);

            return estaNoDepartamento && cargoCompativel;
        }

        private async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> DecidirSolicitacaoAsync(int id, bool aprovar)
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(usuario.Messages);

            var atualizado = aprovar
                ? await _solicitacaoObtencaoTarefaRepository.AprovarAsync(id, usuario.Dados.Id, usuario.Dados.Codigo)
                : await _solicitacaoObtencaoTarefaRepository.RecusarAsync(id, usuario.Dados.Id, usuario.Dados.Codigo);

            if (!atualizado)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha("Nao foi possivel decidir a solicitacao.");

            var solicitacao = await _solicitacaoObtencaoTarefaRepository.ObterPorIdAsync(id);
            var dto = await MapearSolicitacaoAsync(solicitacao);
            await CriarNotificacaoDecisaoSolicitacaoAsync(solicitacao, aprovar);
            var mensagem = aprovar ? "Solicitacao aprovada com sucesso." : "Solicitacao recusada com sucesso.";

            return Resultado<SolicitacaoObtencaoTarefaDTO>
                .Sucesso(dto)
                .AdicionarMensagem(mensagem);
        }

        private async Task<Usuario> ResolverAprovadorObtencaoAsync(Usuario solicitante, Tarefa tarefa)
        {
            var gestorImediato = await ObterUsuarioAprovadorAsync(solicitante.GestorImediatoCodigo, solicitante);
            if (gestorImediato is not null)
            {
                return gestorImediato;
            }

            var gestorDepartamentoTarefa = await ObterUsuarioAprovadorAsync(tarefa.Departamento?.GestorDepartamentoCodigo, solicitante);
            if (gestorDepartamentoTarefa is not null)
            {
                return gestorDepartamentoTarefa;
            }

            var departamentoSolicitante = solicitante.UsuarioCargoDepartamentos?
                .Select(rel => rel.Departamento)
                .FirstOrDefault(departamento => departamento is not null);

            return await ObterUsuarioAprovadorAsync(departamentoSolicitante?.GestorDepartamentoCodigo, solicitante);
        }

        private async Task<Usuario> ObterUsuarioAprovadorAsync(string codigo, Usuario solicitante)
        {
            if (codigo.IsNullOrEmpty() || codigo == solicitante.Codigo)
            {
                return null;
            }

            return await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
        }

        private async Task<SolicitacaoObtencaoTarefaDTO> MapearSolicitacaoAsync(SolicitacaoObtencaoTarefa solicitacao)
        {
            return new SolicitacaoObtencaoTarefaDTO
            {
                Id = solicitacao.Id,
                TarefaId = solicitacao.TarefaId,
                Status = solicitacao.Status,
                DataSolicitacao = solicitacao.DataSolicitacao,
                DataDecisao = solicitacao.DataDecisao,
                Tarefa = await _map.MapToDTOAsync(solicitacao.Tarefa),
                Solicitante = MapearUsuarioResumo(solicitacao.Solicitante),
                Aprovador = MapearUsuarioResumo(solicitacao.Aprovador)
            };
        }

        private async Task CriarNotificacaoDecisaoSolicitacaoAsync(SolicitacaoObtencaoTarefa solicitacao, bool aprovada)
        {
            var identificador = solicitacao.Tarefa?.Identificador?.ToString() ?? solicitacao.TarefaId.ToString();
            var titulo = aprovada
                ? "Solicitacao de tarefa aprovada"
                : "Solicitacao de tarefa recusada";
            var mensagem = aprovada
                ? $"Sua solicitacao para obter a tarefa {identificador} foi aprovada."
                : $"Sua solicitacao para obter a tarefa {identificador} foi recusada.";

            await _notificacaoInternaService.CriarAsync(new NotificacaoInterna
            {
                UsuarioId = solicitacao.SolicitanteId,
                UsuarioCodigo = solicitacao.SolicitanteCodigo,
                Titulo = titulo,
                Mensagem = mensagem,
                Modulo = "Tarefas",
                TipoEvento = aprovada ? "SolicitacaoObtencaoAprovada" : "SolicitacaoObtencaoRecusada",
                TarefaId = solicitacao.TarefaId,
                UrlDestino = $"/atron/tarefas/editar/{solicitacao.TarefaId}",
                Lida = false
            });
        }

        private static UsuarioDTO MapearUsuarioResumo(Usuario usuario)
        {
            if (usuario is null)
            {
                return null;
            }

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Email = usuario.Email
            };
        }
    }
}
