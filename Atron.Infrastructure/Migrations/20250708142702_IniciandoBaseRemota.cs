using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class IniciandoBaseRemota : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefreshToken = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RefreshTokenExpireTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CriadoPor = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    AlteradoPor = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    ModuloCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Inativo = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Removido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.UniqueConstraint("AK_Categorias_Codigo", x => x.Codigo);
                    table.UniqueConstraint("AK_Categorias_Id_Codigo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Endereco_UF = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Endereco_CEP = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.UniqueConstraint("AK_Clientes_Id_Codigo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "PropriedadesDeFluxo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropriedadesDeFluxo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Sobrenome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalarioAtual = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "AppRoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRoleClaim_AppRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserClaim_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AppUserLogin_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AppUserRole_AppRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserRole_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserToken",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AppUserToken_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Historicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditoriaId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Historicos_Auditorias_AuditoriaId",
                        column: x => x.AuditoriaId,
                        principalTable: "Auditorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => new { x.Id, x.Codigo });
                    table.ForeignKey(
                        name: "FK_Cargos_Departamentos_DepartamentoId_DepartamentoCodigo",
                        columns: x => new { x.DepartamentoId, x.DepartamentoCodigo },
                        principalTable: "Departamentos",
                        principalColumns: new[] { "Id", "Codigo" });
                });

            migrationBuilder.CreateTable(
                name: "PropriedadeDeFluxoModulo",
                columns: table => new
                {
                    ModuloId = table.Column<int>(type: "int", nullable: false),
                    ModuloCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PropriedadeDeFluxoId = table.Column<int>(type: "int", nullable: false),
                    PropriedadeDeFluxoCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropriedadeDeFluxoModulo", x => new { x.ModuloId, x.ModuloCodigo, x.PropriedadeDeFluxoId, x.PropriedadeDeFluxoCodigo });
                    table.ForeignKey(
                        name: "FK_PropriedadeDeFluxoModulo_Modulos_ModuloId_ModuloCodigo",
                        columns: x => new { x.ModuloId, x.ModuloCodigo },
                        principalTable: "Modulos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropriedadeDeFluxoModulo_PropriedadesDeFluxo_PropriedadeDeFluxoId_PropriedadeDeFluxoCodigo",
                        columns: x => new { x.PropriedadeDeFluxoId, x.PropriedadeDeFluxoCodigo },
                        principalTable: "PropriedadesDeFluxo",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfisDeAcesso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisDeAcesso", x => new { x.Id, x.Codigo });
                    table.ForeignKey(
                        name: "FK_PerfisDeAcesso_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" });
                });

            migrationBuilder.CreateTable(
                name: "Salarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    SalarioMensal = table.Column<int>(type: "int", nullable: false),
                    Ano = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    MesId = table.Column<int>(type: "int", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salarios_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tarefas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    DataInicial = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFinal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TarefaEstadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" });
                });

            migrationBuilder.CreateTable(
                name: "UsuarioCargoDepartamento",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    CargoId = table.Column<int>(type: "int", nullable: false),
                    CargoCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCargoDepartamento", x => new { x.UsuarioId, x.UsuarioCodigo, x.CargoId, x.CargoCodigo, x.DepartamentoId, x.DepartamentoCodigo });
                    table.ForeignKey(
                        name: "FK_UsuarioCargoDepartamento_Cargos_CargoId_CargoCodigo",
                        columns: x => new { x.CargoId, x.CargoCodigo },
                        principalTable: "Cargos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCargoDepartamento_Departamentos_DepartamentoId_DepartamentoCodigo",
                        columns: x => new { x.DepartamentoId, x.DepartamentoCodigo },
                        principalTable: "Departamentos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCargoDepartamento_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilDeAcessoModulos",
                columns: table => new
                {
                    PerfilDeAcessoId = table.Column<int>(type: "int", nullable: false),
                    PerfilDeAcessoCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ModuloId = table.Column<int>(type: "int", nullable: false),
                    ModuloCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilDeAcessoModulos", x => new { x.PerfilDeAcessoId, x.PerfilDeAcessoCodigo, x.ModuloId, x.ModuloCodigo });
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoModulos_Modulos_ModuloId_ModuloCodigo",
                        columns: x => new { x.ModuloId, x.ModuloCodigo },
                        principalTable: "Modulos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoModulos_PerfisDeAcesso_PerfilDeAcessoId_PerfilDeAcessoCodigo",
                        columns: x => new { x.PerfilDeAcessoId, x.PerfilDeAcessoCodigo },
                        principalTable: "PerfisDeAcesso",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilDeAcessoUsuarios",
                columns: table => new
                {
                    PerfilDeAcessoId = table.Column<int>(type: "int", nullable: false),
                    PerfilDeAcessoCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilDeAcessoUsuarios", x => new { x.PerfilDeAcessoId, x.PerfilDeAcessoCodigo, x.UsuarioId, x.UsuarioCodigo });
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoUsuarios_PerfisDeAcesso_PerfilDeAcessoId_PerfilDeAcessoCodigo",
                        columns: x => new { x.PerfilDeAcessoId, x.PerfilDeAcessoCodigo },
                        principalTable: "PerfisDeAcesso",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoUsuarios_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoCategoria",
                columns: table => new
                {
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CategoriaCodigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoCategoria", x => new { x.ProdutoId, x.CategoriaId });
                    table.ForeignKey(
                        name: "FK_ProdutoCategoria_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuantidadeEmEstoque = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Removido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VendaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.UniqueConstraint("AK_Produtos_Codigo", x => x.Codigo);
                    table.UniqueConstraint("AK_Produtos_Id_Codigo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataVenda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantidadeDeProdutoVendido = table.Column<int>(type: "int", nullable: false),
                    PrecoDoProduto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Removido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime", nullable: true),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    CategoriaCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    ClienteCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendas_Categorias_CategoriaId_CategoriaCodigo",
                        columns: x => new { x.CategoriaId, x.CategoriaCodigo },
                        principalTable: "Categorias",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendas_Clientes_ClienteId_ClienteCodigo",
                        columns: x => new { x.ClienteId, x.ClienteCodigo },
                        principalTable: "Clientes",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendas_Produtos_ProdutoId_ProdutoCodigo",
                        columns: x => new { x.ProdutoId, x.ProdutoCodigo },
                        principalTable: "Produtos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Codigo", "Id", "Descricao" },
                values: new object[,]
                {
                    { "DPT", 1, "Departamentos" },
                    { "CRG", 2, "Cargos" },
                    { "USR", 3, "Usuários" },
                    { "TAR", 4, "Tarefas" },
                    { "SAL", 5, "Salários" },
                    { "PAC", 6, "Políticas e Acessos" }
                });

            migrationBuilder.InsertData(
                table: "PropriedadesDeFluxo",
                columns: new[] { "Codigo", "Id" },
                values: new object[,]
                {
                    { "GRAVAR", 1 },
                    { "CONSULTAR", 2 },
                    { "DELETAR", 3 },
                    { "ATUALIZAR", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AppRole",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoleClaim_RoleId",
                table: "AppRoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AppUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AppUser",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserClaim_UserId",
                table: "AppUserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserLogin_UserId",
                table: "AppUserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRole_RoleId",
                table: "AppUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_DepartamentoId_DepartamentoCodigo",
                table: "Cargos",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Codigo",
                table: "Categorias",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_AuditoriaId",
                table: "Historicos",
                column: "AuditoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilDeAcessoModulos_ModuloId_ModuloCodigo",
                table: "PerfilDeAcessoModulos",
                columns: new[] { "ModuloId", "ModuloCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_PerfilDeAcessoUsuarios_UsuarioId_UsuarioCodigo",
                table: "PerfilDeAcessoUsuarios",
                columns: new[] { "UsuarioId", "UsuarioCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_PerfisDeAcesso_UsuarioId_UsuarioCodigo",
                table: "PerfisDeAcesso",
                columns: new[] { "UsuarioId", "UsuarioCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoCategoria_CategoriaId",
                table: "ProdutoCategoria",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_Codigo",
                table: "Produtos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_VendaId",
                table: "Produtos",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropriedadeDeFluxoModulo_PropriedadeDeFluxoId_PropriedadeDeFluxoCodigo",
                table: "PropriedadeDeFluxoModulo",
                columns: new[] { "PropriedadeDeFluxoId", "PropriedadeDeFluxoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Salarios_UsuarioId_UsuarioCodigo",
                table: "Salarios",
                columns: new[] { "UsuarioId", "UsuarioCodigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_UsuarioId_UsuarioCodigo",
                table: "Tarefas",
                columns: new[] { "UsuarioId", "UsuarioCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCargoDepartamento_CargoId_CargoCodigo",
                table: "UsuarioCargoDepartamento",
                columns: new[] { "CargoId", "CargoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCargoDepartamento_DepartamentoId_DepartamentoCodigo",
                table: "UsuarioCargoDepartamento",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_CategoriaId_CategoriaCodigo",
                table: "Vendas",
                columns: new[] { "CategoriaId", "CategoriaCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteId_ClienteCodigo",
                table: "Vendas",
                columns: new[] { "ClienteId", "ClienteCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ProdutoId_ProdutoCodigo",
                table: "Vendas",
                columns: new[] { "ProdutoId", "ProdutoCodigo" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoCategoria_Produtos_ProdutoId",
                table: "ProdutoCategoria",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Vendas_VendaId",
                table: "Produtos",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Categorias_CategoriaId_CategoriaCodigo",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId_ProdutoCodigo",
                table: "Vendas");

            migrationBuilder.DropTable(
                name: "AppRoleClaim");

            migrationBuilder.DropTable(
                name: "AppUserClaim");

            migrationBuilder.DropTable(
                name: "AppUserLogin");

            migrationBuilder.DropTable(
                name: "AppUserRole");

            migrationBuilder.DropTable(
                name: "AppUserToken");

            migrationBuilder.DropTable(
                name: "Historicos");

            migrationBuilder.DropTable(
                name: "PerfilDeAcessoModulos");

            migrationBuilder.DropTable(
                name: "PerfilDeAcessoUsuarios");

            migrationBuilder.DropTable(
                name: "ProdutoCategoria");

            migrationBuilder.DropTable(
                name: "PropriedadeDeFluxoModulo");

            migrationBuilder.DropTable(
                name: "Salarios");

            migrationBuilder.DropTable(
                name: "Tarefas");

            migrationBuilder.DropTable(
                name: "UsuarioCargoDepartamento");

            migrationBuilder.DropTable(
                name: "AppRole");

            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "PerfisDeAcesso");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "PropriedadesDeFluxo");

            migrationBuilder.DropTable(
                name: "Cargos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
