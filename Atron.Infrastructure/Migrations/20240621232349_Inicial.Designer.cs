﻿// <auto-generated />
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Atron.Infrastructure.Migrations
{
    [DbContext(typeof(AtronDbContext))]
    [Migration("20240621232349_Inicial")]
    partial class Inicial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Atron.Domain.Entities.Cargo", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("DepartamentoId_Antigo")
                        .HasColumnType("int");

                    b.Property<string>("DepartmentoCodigo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DepartmentoId")
                        .HasColumnType("int");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentoId");

                    b.ToTable("Cargos");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Departamento", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Departamentos");
                });

            modelBuilder.Entity("Atron.Domain.Entities.Mes", b =>
                {
                    b.Property<int>("MesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

            modelBuilder.Entity("Atron.Domain.Entities.Cargo", b =>
                {
                    b.HasOne("Atron.Domain.Entities.Departamento", "Departmento")
                        .WithMany()
                        .HasForeignKey("DepartmentoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Departmento");
                });
#pragma warning restore 612, 618
        }
    }
}
