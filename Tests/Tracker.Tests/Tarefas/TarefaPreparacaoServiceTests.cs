using Application.DTO;
using Application.Mapping;
using Application.Services.EntitiesServices.Tarefas;
using Application.Validador;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Moq;
using Shared.Application.Interfaces.Mapping;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaPreparacaoServiceTests
{
    [Fact]
    public async Task PrepararParaPersistenciaAsync_DeveBloquearTarefaAtribuidaQuandoUsuarioEDepartamentoNaoTiveremGestor()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var usuario = CriarUsuario(departamento);
        var service = CriarService(usuario, departamento);

        var resultado = await service.PrepararParaPersistenciaAsync(CriarTarefaDto());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == "Nao foi possivel criar a tarefa porque o usuario responsavel nao possui gestor imediato e o departamento nao possui gestor definido.");
    }

    [Fact]
    public async Task PrepararParaPersistenciaAsync_DevePermitirTarefaAtribuidaQuandoUsuarioTemGestorImediato()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var usuario = CriarUsuario(departamento);
        usuario.GestorImediatoId = 99;
        usuario.GestorImediatoCodigo = "GST";
        var service = CriarService(usuario, departamento);
        var tarefaDTO = CriarTarefaDto();

        var resultado = await service.PrepararParaPersistenciaAsync(tarefaDTO);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(usuario.Id, resultado.Dados.UsuarioId);
        Assert.Equal(usuario.Codigo, resultado.Dados.UsuarioCodigo);
        Assert.Equal(usuario.Id, tarefaDTO.Usuario?.Id);
    }

    [Fact]
    public async Task PrepararParaPersistenciaAsync_DevePermitirTarefaAtribuidaQuandoDepartamentoDoUsuarioTemGestor()
    {
        var departamento = new Departamento
        {
            Id = 10,
            Codigo = "DPT",
            Descricao = "Departamento",
            GestorDepartamentoId = 99,
            GestorDepartamentoCodigo = "GST"
        };
        var usuario = CriarUsuario(departamento);
        var service = CriarService(usuario, departamento);

        var resultado = await service.PrepararParaPersistenciaAsync(CriarTarefaDto());

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(usuario.Id, resultado.Dados.UsuarioId);
        Assert.Equal(usuario.Codigo, resultado.Dados.UsuarioCodigo);
    }

    [Fact]
    public async Task PrepararParaPersistenciaAsync_DevePermitirTarefaAtribuidaAoProprioGestorDoDepartamento()
    {
        var departamento = new Departamento
        {
            Id = 10,
            Codigo = "DPT",
            Descricao = "Departamento",
            GestorDepartamentoId = 20,
            GestorDepartamentoCodigo = "USR"
        };
        var usuario = CriarUsuario(departamento);
        var service = CriarService(usuario, departamento);

        var resultado = await service.PrepararParaPersistenciaAsync(CriarTarefaDto());

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(usuario.Id, resultado.Dados.UsuarioId);
        Assert.Equal(usuario.Codigo, resultado.Dados.UsuarioCodigo);
    }

    [Fact]
    public async Task PrepararParaPersistenciaAsync_DeveRetornarFalhaQuandoEstadoNaoExiste()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var usuario = CriarUsuario(departamento);
        var service = CriarService(usuario, departamento);
        var tarefa = CriarTarefaDto();
        tarefa.EstadoDaTarefa.Id = 999;

        var resultado = await service.PrepararParaPersistenciaAsync(tarefa);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == Application.Resources.TarefaResource.Erro_EstadoNaoEncontrado);
    }

    [Fact]
    public async Task PrepararParaPersistenciaAsync_DevePropagarFalhaQuandoUsuarioNaoExiste()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var usuario = CriarUsuario(departamento);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository
            .Setup(repository => repository.ObterUsuarioPorCodigoAsync("USR"))
            .ReturnsAsync((Usuario)null!);
        var service = CriarService(
            usuario,
            departamento,
            new DependenciasPreparacao(UsuarioRepository: usuarioRepository.Object));

        var resultado = await service.PrepararParaPersistenciaAsync(CriarTarefaDto());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == Shared.Application.Resources.NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
    }

    [Fact]
    public async Task PrepararParaPersistenciaAsync_DevePropagarFalhaQuandoDepartamentoNaoExiste()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var usuario = CriarUsuario(departamento);
        var departamentoRepository = new Mock<IDepartamentoRepository>();
        departamentoRepository
            .Setup(repository => repository.ObterDepartamentoPorCodigoRepository("DPT"))
            .ReturnsAsync((Departamento)null!);
        var service = CriarService(
            usuario,
            departamento,
            new DependenciasPreparacao(DepartamentoRepository: departamentoRepository.Object));
        var tarefa = CriarTarefaDto();
        tarefa.DestinoInicial = (int)DestinoInicialTarefa.DepartamentoCargo;
        tarefa.UsuarioCodigo = null;
        tarefa.DepartamentoCodigo = "DPT";

        var resultado = await service.PrepararParaPersistenciaAsync(tarefa);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == Application.Resources.TarefaResource.Erro_DepartamentoNaoEncontrado);
    }

    private static TarefaPreparacaoService CriarService(
        Usuario usuario,
        Departamento departamento,
        DependenciasPreparacao? dependencias = null)
    {
        var usuarioRelacionamento = new TarefaUsuarioRelacionamentoService(
            dependencias?.UsuarioRepository ?? new UsuarioRepositoryFake(usuario),
            new UsuarioMapFake());
        var estruturaRelacionamento = new TarefaDepartamentoCargoRelacionamentoService(
            dependencias?.DepartamentoRepository ?? new DepartamentoRepositoryFake(departamento),
            new CargoRepositoryFake([]));
        var tarefaRelacionamento = new TarefaRelacionamentoService(
            usuarioRelacionamento,
            estruturaRelacionamento);
        var tarefaEstadoRepository = new Mock<ITarefaEstadoRepository>();
        tarefaEstadoRepository
            .Setup(repository => repository.ObterPorIdAsync(1))
            .ReturnsAsync(new TarefaEstado { Id = 1, Descricao = "Aberta" });

        return new TarefaPreparacaoService(
            tarefaRelacionamento,
            tarefaEstadoRepository.Object,
            new TarefaEstadoMapping(),
            new TarefaMapFake(),
            new TarefaValidador());
    }

    private sealed record DependenciasPreparacao(
        IUsuarioRepository? UsuarioRepository = null,
        IDepartamentoRepository? DepartamentoRepository = null);

    private static Usuario CriarUsuario(Departamento departamento)
    {
        return new Usuario
        {
            Id = 20,
            Codigo = "USR",
            Nome = "Usuario",
            Sobrenome = "Teste",
            Email = "usuario@teste.com",
            UsuarioCargoDepartamentos =
            [
                new UsuarioCargoDepartamento
                {
                    UsuarioId = 20,
                    UsuarioCodigo = "USR",
                    CargoId = 30,
                    CargoCodigo = "CRG",
                    DepartamentoId = departamento.Id,
                    DepartamentoCodigo = departamento.Codigo,
                    Departamento = departamento
                }
            ]
        };
    }

    private static TarefaDTO CriarTarefaDto()
    {
        return new TarefaDTO
        {
            DestinoInicial = (int)DestinoInicialTarefa.Usuario,
            UsuarioCodigo = "USR",
            Titulo = "Tarefa teste",
            Conteudo = "Conteudo",
            DataInicial = new DateTime(2026, 7, 7),
            DataFinal = new DateTime(2026, 7, 8),
            EstadoDaTarefa = new TarefaEstadoDTO { Id = 1, Descricao = "Aberta" }
        };
    }

    private sealed class TarefaMapFake : IToEntityMapper<Tarefa, TarefaDTO>
    {
        public Tarefa MapToEntity(TarefaDTO dto)
        {
            return new Tarefa
            {
                Id = dto.Id,
                DestinoInicial = dto.DestinoInicial,
                ExigeAprovacaoParaObter = dto.ExigeAprovacaoParaObter,
                UsuarioCodigo = dto.UsuarioCodigo?.ToUpper(),
                DepartamentoCodigo = dto.DepartamentoCodigo?.ToUpper(),
                CargoCodigo = dto.CargoCodigo?.ToUpper(),
                Titulo = dto.Titulo,
                Conteudo = dto.Conteudo,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal,
                TarefaEstadoId = dto.EstadoDaTarefa?.Id ?? 0
            };
        }

        public IEnumerable<Tarefa> MapToEntities(IEnumerable<TarefaDTO>? dtos)
            => dtos?.Select(MapToEntity) ?? [];
    }

    private sealed class UsuarioMapFake : IToDtoMapper<Usuario, UsuarioDTO>
    {
        public UsuarioDTO MapToDto(Usuario entity)
            => new()
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Sobrenome = entity.Sobrenome,
                Email = entity.Email,
                ReceberNotificacaoInternaTarefa = entity.ReceberNotificacaoInternaTarefa,
                ReceberNotificacaoTarefaPorEmail = entity.ReceberNotificacaoTarefaPorEmail
            };

        public IEnumerable<UsuarioDTO> MapToDtos(IEnumerable<Usuario>? entities)
            => entities?.Select(MapToDto) ?? [];
    }

    private sealed class UsuarioRepositoryFake(Usuario usuario) : IUsuarioRepository
    {
        public Task<bool> AtualizarRepositoryAsync(Usuario entity) => Task.FromResult(true);

        public Task<bool> AtualizarRepositoryAsync(int id, Usuario entity) => Task.FromResult(true);

        public Task<bool> AtualizarPreferenciaNotificacaoTarefaPorEmailAsync(string codigo, bool receberNotificacao) => Task.FromResult(true);

        public Task<bool> AtualizarPreferenciasNotificacaoTarefaAsync(string codigo, bool receberNotificacaoInterna, bool receberNotificacaoPorEmail) => Task.FromResult(true);

        public Task<bool> AtualizarUsuarioAsync(Usuario usuario) => Task.FromResult(true);

        public Task<bool> CriarRepositoryAsync(Usuario entity) => Task.FromResult(true);

        public Task<bool> CriarUsuarioAsync(Usuario usuario) => Task.FromResult(true);

        public Task<bool> ConfirmarEmailAsync(string codigo) => Task.FromResult(true);

        public Task<Usuario> ObterInativoPorEmailAsync(string email) => Task.FromResult<Usuario>(null!);

        public Task<Usuario> ObterPorCodigoRepositoryAsync(string codigo) => Task.FromResult(codigo == usuario.Codigo ? usuario : null!);

        public Task<Usuario> ObterPorIdRepositoryAsync(int id) => Task.FromResult(id == usuario.Id ? usuario : null!);

        public Task<IEnumerable<Usuario>> ObterTodosRepositoryAsync() => Task.FromResult<IEnumerable<Usuario>>([usuario]);

        public Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity() => Task.FromResult(new List<UsuarioIdentity>());

        public Task<Usuario> ObterUsuarioGeralPorCodigoAsync(string codigo) => Task.FromResult(codigo == usuario.Codigo ? usuario : null!);

        public Task<Usuario> ObterUsuarioGeralPorEmailAsync(string email) => Task.FromResult<Usuario>(null!);

        public Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo) => Task.FromResult(codigo == usuario.Codigo ? usuario : null!);

        public Task<Usuario> ObterUsuarioPorIdAsync(int? id) => Task.FromResult(id == usuario.Id ? usuario : null!);

        public Task<IEnumerable<Usuario>> ObterUsuariosAsync() => Task.FromResult<IEnumerable<Usuario>>([usuario]);

        public Task<bool> RemoverRepositoryAsync(Usuario entity) => Task.FromResult(true);

        public Task<bool> RemoverUsuarioAsync(Usuario usuario) => Task.FromResult(true);

        public Task<bool> VerificarEmailExistenteAsync(string email) => Task.FromResult(false);
    }

    private sealed class DepartamentoRepositoryFake(Departamento departamento) : IDepartamentoRepository
    {
        public Task<bool> AtualizarDepartamentoRepositoryAsync(Departamento departamento) => Task.FromResult(true);

        public Task<bool> CriarDepartamentoRepositoryAsync(Departamento departamento) => Task.FromResult(true);

        public Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
            => Task.FromResult(codigo == departamento.Codigo ? departamento : null!);

        public Task<Departamento> ObterDepartamentoPorCodigoRepository(string codigo)
            => Task.FromResult(codigo == departamento.Codigo ? departamento : null!);

        public Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
            => Task.FromResult(id == departamento.Id ? departamento : null!);

        public Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
            => Task.FromResult<IEnumerable<Departamento>>([departamento]);

        public Task<IEnumerable<Departamento>> ObterDepartamentosPorCodigoGestorAsync(string usuarioCodigo)
            => Task.FromResult<IEnumerable<Departamento>>([]);

        public Task<bool> RemoverDepartmentoRepositoryAsync(Departamento departamento) => Task.FromResult(true);
    }

    private sealed class CargoRepositoryFake(IReadOnlyCollection<Cargo> cargos) : ICargoRepository
    {
        public Task<bool> AtualizarCargoAsync(Cargo cargo) => Task.FromResult(true);

        public Task<bool> AtualizarRepositoryAsync(Cargo entity) => Task.FromResult(true);

        public Task<bool> AtualizarRepositoryAsync(int id, Cargo entity) => Task.FromResult(true);

        public Task<bool> CriarCargoAsync(Cargo cargo) => Task.FromResult(true);

        public Task<bool> CriarRepositoryAsync(Cargo entity) => Task.FromResult(true);

        public Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
            => Task.FromResult(cargos.SingleOrDefault(cargo => cargo.Codigo == codigo) ?? null!);

        public Task<Cargo> ObterCargoPorIdAsync(int? id)
            => Task.FromResult(cargos.SingleOrDefault(cargo => cargo.Id == id) ?? null!);

        public Task<IEnumerable<Cargo>> ObterCargosAsync() => Task.FromResult<IEnumerable<Cargo>>(cargos);

        public Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
            => Task.FromResult<IEnumerable<Cargo>>(cargos.Where(cargo =>
                cargo.DepartamentoId == departamentoId &&
                cargo.DepartamentoCodigo == departamentoCodigo));

        public Task<Cargo> ObterPorCodigoRepositoryAsync(string codigo)
            => Task.FromResult(cargos.SingleOrDefault(cargo => cargo.Codigo == codigo) ?? null!);

        public Task<Cargo> ObterPorIdRepositoryAsync(int id)
            => Task.FromResult(cargos.SingleOrDefault(cargo => cargo.Id == id) ?? null!);

        public Task<IEnumerable<Cargo>> ObterTodosRepositoryAsync()
            => Task.FromResult<IEnumerable<Cargo>>(cargos);

        public Task<bool> RemoverCargoAsync(Cargo cargo) => Task.FromResult(true);

        public Task<bool> RemoverRepositoryAsync(Cargo entity) => Task.FromResult(true);
    }
}
