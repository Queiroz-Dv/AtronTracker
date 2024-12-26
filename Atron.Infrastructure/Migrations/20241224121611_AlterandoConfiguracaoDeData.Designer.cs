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
    [Migration("20241224121611_AlterandoConfiguracaoDeData")]
    partial class AlterandoConfiguracaoDeData
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

                    b.Property<Guid>("IdSequencial")
                        .HasColumnType("uniqueidentifier");

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

                    b.Property<Guid>("IdSequencial")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id", "Codigo");

                    b.ToTable("Departamentos");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Mes", b =>
                {
                    b.Property<int>("MesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MesId"), 1L, 1);

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("MesId");

                    b.ToTable("Meses");

                    b.HasData(
                        new
                        {
                            MesId = 1,
                            Descricao = "Janeiro"
                        },
                        new
                        {
                            MesId = 2,
                            Descricao = "Fevereiro"
                        },
                        new
                        {
                            MesId = 3,
                            Descricao = "Março"
                        },
                        new
                        {
                            MesId = 4,
                            Descricao = "Abril"
                        },
                        new
                        {
                            MesId = 5,
                            Descricao = "Maio"
                        },
                        new
                        {
                            MesId = 6,
                            Descricao = "Junho"
                        },
                        new
                        {
                            MesId = 7,
                            Descricao = "Julho"
                        },
                        new
                        {
                            MesId = 8,
                            Descricao = "Agosto"
                        },
                        new
                        {
                            MesId = 9,
                            Descricao = "Setembro"
                        },
                        new
                        {
                            MesId = 10,
                            Descricao = "Outubro"
                        },
                        new
                        {
                            MesId = 11,
                            Descricao = "Novembro"
                        },
                        new
                        {
                            MesId = 12,
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

                    b.Property<Guid>("IdSequencial")
                        .HasColumnType("uniqueidentifier");

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

                    b.Property<DateTime?>("Ano")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<Guid>("IdSequencial")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MesId")
                        .HasColumnType("int");

                    b.Property<int>("SalarioMensal")
                        .HasColumnType("int");

                    b.Property<string>("UsuarioCodigo")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

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

                    b.Property<int>("EstadoDaTarefaId")
                        .HasColumnType("int");

                    b.Property<Guid>("IdSequencial")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UsuarioCodigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EstadoDaTarefaId");

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

                    b.Property<string>("CargoCodigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CargoId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DataNascimento")
                        .HasColumnType("datetime2");

                    b.Property<string>("DepartamentoCodigo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DepartamentoId")
                        .HasColumnType("int");

                    b.Property<Guid>("IdSequencial")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int>("Salario")
                        .HasColumnType("int");

                    b.Property<string>("Sobrenome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id", "Codigo");

                    b.HasIndex("CargoId", "CargoCodigo");

                    b.HasIndex("DepartamentoId", "DepartamentoCodigo");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Cargo", b =>
                {
                    b.HasOne("Atron.Domain.Entities.Departamento", "Departamento")
                        .WithMany("Cargos")
                        .HasForeignKey("DepartmentoId", "DepartamentoCodigo");

                    b.Navigation("Departamento");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Tarefa", b =>
                {
                    b.HasOne("Atron.Domain.Entities.TarefaEstado", "TarefaEstado")
                        .WithMany("Tarefas")
                        .HasForeignKey("EstadoDaTarefaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Atron.Domain.Entities.Usuario", "Usuario")
                        .WithMany("Tarefas")
                        .HasForeignKey("UsuarioId", "UsuarioCodigo");

                    b.Navigation("TarefaEstado");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Usuario", b =>
                {
                    b.HasOne("Atron.Domain.Entities.Cargo", "Cargo")
                        .WithMany("Usuarios")
                        .HasForeignKey("CargoId", "CargoCodigo");

                    b.HasOne("Atron.Domain.Entities.Departamento", "Departamento")
                        .WithMany("Usuarios")
                        .HasForeignKey("DepartamentoId", "DepartamentoCodigo");

                    b.Navigation("Cargo");

                    b.Navigation("Departamento");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Cargo", b =>
                {
                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Departamento", b =>
                {
                    b.Navigation("Cargos");

                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("Atron.Domain.Entities.TarefaEstado", b =>
                {
                    b.Navigation("Tarefas");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Usuario", b =>
                {
                    b.Navigation("Tarefas");
                });
#pragma warning restore 612, 618
        }
    }
}
