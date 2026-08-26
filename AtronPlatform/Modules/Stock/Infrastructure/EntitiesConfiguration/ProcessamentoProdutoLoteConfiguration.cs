#nullable enable

using System.Text.Json;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration;

public sealed class ProcessamentoProdutoLoteConfiguration
    : IEntityTypeConfiguration<ProcessamentoProdutoLote>
{
    public void Configure(EntityTypeBuilder<ProcessamentoProdutoLote> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Status)
            .HasDefaultValue(EStatusProcessamentoProdutoLote.Pendente)
            .IsRequired();
        builder.OwnsOne(item => item.Solicitacao, solicitacao =>
        {
            solicitacao.Property(item => item.CodigoBase)
                .HasColumnName("CodigoBase")
                .HasMaxLength(25)
                .IsRequired();
            solicitacao.Property(item => item.QuantidadeSolicitada)
                .HasColumnName("QuantidadeSolicitada");
            solicitacao.Property(item => item.SolicitanteCodigo)
                .HasColumnName("SolicitanteCodigo")
                .HasMaxLength(50)
                .IsRequired();
            solicitacao.Property(item => item.Descricao)
                .HasColumnName("Descricao")
                .HasMaxLength(50)
                .IsRequired();
            solicitacao.Property(item => item.DescricaoComplementar)
                .HasColumnName("DescricaoComplementar");
            solicitacao.Property(item => item.DataAquisicao)
                .HasColumnName("DataAquisicao");
            solicitacao.Property(item => item.PrecoUnitario)
                .HasColumnName("PrecoUnitario")
                .HasPrecision(18, 2);

            var categoriaCodigos = solicitacao.Property(item => item.CategoriaCodigos)
                .HasColumnName("CategoriaCodigos")
                .HasConversion(
                    codigos => JsonSerializer.Serialize(codigos, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<List<string>>(
                        json,
                        (JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("text")
                .IsRequired();
            categoriaCodigos.Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (left, right) => left == right
                    || (left != null && right != null && left.SequenceEqual(right)),
                codigos => codigos.Aggregate(
                    0,
                    (hash, codigo) => HashCode.Combine(hash, codigo.GetHashCode())),
                codigos => codigos.ToList()));
        });
        builder.Navigation(item => item.Solicitacao).IsRequired();
        builder.OwnsOne(item => item.Resultado, resultado =>
        {
            resultado.Property(item => item.QuantidadeProcessada)
                .HasColumnName("QuantidadeProcessada");
            resultado.Property(item => item.Erro)
                .HasColumnName("Erro")
                .HasMaxLength(2000);
        });
        builder.Navigation(item => item.Resultado).IsRequired();
        builder.Property(item => item.ReservadoEm).HasColumnType("timestamp with time zone");
        builder.Property(item => item.ReservaExpiraEm).HasColumnType("timestamp with time zone");
        builder.Property(item => item.TokenReserva).IsConcurrencyToken();
        builder.HasIndex(item => new { item.Status, item.Id });
        builder.HasIndex(item => new { item.Status, item.ReservaExpiraEm, item.Id });
        builder.HasIndex(item => item.LoteProdutoId).IsUnique();
        builder.HasOne(item => item.LoteProduto)
            .WithMany()
            .HasForeignKey(item => item.LoteProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
