﻿// <auto-generated />
using System;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    [DbContext(typeof(AtronDbContext))]
    [Migration("20250119194600_RemovendoIdSequencial")]
    partial class RemovendoIdSequencial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.32")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Atron.Domain.Entities.Cargo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Codigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DepartamentoCodigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DepartmentoId")
                        .HasColumnType("int");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id", "Codigo");

                    b.HasIndex("DepartmentoId", "DepartamentoCodigo");

                    b.ToTable("Cargos");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Departamento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Codigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id", "Codigo");

                    b.ToTable("Departamentos");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Mes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.ToTable("Meses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Descricao = "Janeiro"
                        },
                        new
                        {
                            Id = 2,
                            Descricao = "Fevereiro"
                        },
                        new
                        {
                            Id = 3,
                            Descricao = "Março"
                        },
                        new
                        {
                            Id = 4,
                            Descricao = "Abril"
                        },
                        new
                        {
                            Id = 5,
                            Descricao = "Maio"
                        },
                        new
                        {
                            Id = 6,
                            Descricao = "Junho"
                        },
                        new
                        {
                            Id = 7,
                            Descricao = "Julho"
                        },
                        new
                        {
                            Id = 8,
                            Descricao = "Agosto"
                        },
                        new
                        {
                            Id = 9,
                            Descricao = "Setembro"
                        },
                        new
                        {
                            Id = 10,
                            Descricao = "Outubro"
                        },
                        new
                        {
                            Id = 11,
                            Descricao = "Novembro"
                        },
                        new
                        {
                            Id = 12,
                            Descricao = "Dezembro"
                        });
                });

            modelBuilder.Entity("Atron.Domain.Entities.Permissao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DataFinal")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataInicial")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .HasMaxLength(2500)
                        .HasColumnType("nvarchar(2500)");

                    b.Property<int>("PermissaoEstadoId")
                        .HasColumnType("int");

                    b.Property<int>("QuantidadeDeDias")
                        .HasMaxLength(365)
                        .HasColumnType("int");

                    b.Property<string>("UsuarioCodigo")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Permissoes");
                });

            modelBuilder.Entity("Atron.Domain.Entities.PermissaoEstado", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("Id");

                    b.ToTable("PermissoesEstados");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Descricao = "Em atividade"
                        },
                        new
                        {
                            Id = 2,
                            Descricao = "Aprovada"
                        },
                        new
                        {
                            Id = 3,
                            Descricao = "Desaprovada"
                        });
                });

            modelBuilder.Entity("Atron.Domain.Entities.Salario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Ano")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.Property<int>("MesId")
                        .HasColumnType("int");

                    b.Property<int>("SalarioMensal")
                        .HasColumnType("int");

                    b.Property<string>("UsuarioCodigo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MesId")
                        .IsUnique();

                    b.HasIndex("UsuarioId", "UsuarioCodigo")
                        .IsUnique();

                    b.ToTable("Salarios");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Tarefa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Conteudo")
                        .HasMaxLength(2500)
                        .HasColumnType("nvarchar(2500)");

                    b.Property<DateTime>("DataFinal")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataInicial")
                        .HasColumnType("datetime2");

                    b.Property<int>("TarefaEstadoId")
                        .HasColumnType("int");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UsuarioCodigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId", "UsuarioCodigo");

                    b.ToTable("Tarefas");
                });

            modelBuilder.Entity("Atron.Domain.Entities.TarefaEstado", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Descricao")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("Id");

                    b.ToTable("TarefaEstados");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Descricao = "Em atividade"
                        },
                        new
                        {
                            Id = 2,
                            Descricao = "Pendente de aprovação"
                        },
                        new
                        {
                            Id = 3,
                            Descricao = "Entregue"
                        },
                        new
                        {
                            Id = 4,
                            Descricao = "Finalizada"
                        },
                        new
                        {
                            Id = 5,
                            Descricao = "Iniciada"
                        });
                });

            modelBuilder.Entity("Atron.Domain.Entities.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Codigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("DataNascimento")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int?>("SalarioAtual")
                        .HasColumnType("int");

                    b.Property<string>("Sobrenome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id", "Codigo");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Atron.Domain.Entities.UsuarioCargoDepartamento", b =>
                {
                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.Property<string>("UsuarioCodigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CargoId")
                        .HasColumnType("int");

                    b.Property<string>("CargoCodigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DepartamentoId")
                        .HasColumnType("int");

                    b.Property<string>("DepartamentoCodigo")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UsuarioId", "UsuarioCodigo", "CargoId", "CargoCodigo", "DepartamentoId", "DepartamentoCodigo");

                    b.HasIndex("CargoId", "CargoCodigo");

                    b.HasIndex("DepartamentoId", "DepartamentoCodigo");

                    b.ToTable("UsuarioCargoDepartamento", (string)null);
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AppRole", (string)null);
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationRoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ClaimValue")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AppRoleClaim", (string)null);
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AppUser", (string)null);
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationUserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ClaimValue")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AppUserClaim", (string)null);
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationUserLogin", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AppUserLogin", (string)null);
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationUserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AppUserRole", (string)null);
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationUserToken", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Value")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AppUserToken", (string)null);
                });

            modelBuilder.Entity("Atron.Domain.Entities.Cargo", b =>
                {
                    b.HasOne("Atron.Domain.Entities.Departamento", "Departamento")
                        .WithMany("Cargos")
                        .HasForeignKey("DepartmentoId", "DepartamentoCodigo");

                    b.Navigation("Departamento");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Salario", b =>
                {
                    b.HasOne("Atron.Domain.Entities.Mes", "Mes")
                        .WithOne("Salario")
                        .HasForeignKey("Atron.Domain.Entities.Salario", "MesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Atron.Domain.Entities.Usuario", "Usuario")
                        .WithOne("Salario")
                        .HasForeignKey("Atron.Domain.Entities.Salario", "UsuarioId", "UsuarioCodigo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Mes");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Tarefa", b =>
                {
                    b.HasOne("Atron.Domain.Entities.Usuario", "Usuario")
                        .WithMany("Tarefas")
                        .HasForeignKey("UsuarioId", "UsuarioCodigo");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Atron.Domain.Entities.UsuarioCargoDepartamento", b =>
                {
                    b.HasOne("Atron.Domain.Entities.Cargo", "Cargo")
                        .WithMany("UsuarioCargoDepartamentos")
                        .HasForeignKey("CargoId", "CargoCodigo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Atron.Domain.Entities.Departamento", "Departamento")
                        .WithMany("UsuarioCargoDepartamentos")
                        .HasForeignKey("DepartamentoId", "DepartamentoCodigo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Atron.Domain.Entities.Usuario", "Usuario")
                        .WithMany("UsuarioCargoDepartamentos")
                        .HasForeignKey("UsuarioId", "UsuarioCodigo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cargo");

                    b.Navigation("Departamento");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationRoleClaim", b =>
                {
                    b.HasOne("Shared.Models.ApplicationModels.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationUserClaim", b =>
                {
                    b.HasOne("Shared.Models.ApplicationModels.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationUserLogin", b =>
                {
                    b.HasOne("Shared.Models.ApplicationModels.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationUserRole", b =>
                {
                    b.HasOne("Shared.Models.ApplicationModels.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.ApplicationModels.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Shared.Models.ApplicationModels.ApplicationUserToken", b =>
                {
                    b.HasOne("Shared.Models.ApplicationModels.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Atron.Domain.Entities.Cargo", b =>
                {
                    b.Navigation("UsuarioCargoDepartamentos");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Departamento", b =>
                {
                    b.Navigation("Cargos");

                    b.Navigation("UsuarioCargoDepartamentos");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Mes", b =>
                {
                    b.Navigation("Salario");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Usuario", b =>
                {
                    b.Navigation("Salario");

                    b.Navigation("Tarefas");

                    b.Navigation("UsuarioCargoDepartamentos");
                });
#pragma warning restore 612, 618
        }
    }
}
