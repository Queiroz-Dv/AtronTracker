using AtronNotificacoes.Domain;
using Microsoft.EntityFrameworkCore;

namespace AtronNotificacoes.Infrastructure;

public sealed class NotificacoesDbContext(DbContextOptions<NotificacoesDbContext> options) : DbContext(options)
{
    public DbSet<NotificacaoInterna> NotificacoesInternas => Set<NotificacaoInterna>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<long>("notificacoes_ids").StartsAt(1000001);

        modelBuilder.Entity<NotificacaoInterna>(builder =>
        {
            builder.ToTable("Notificacoes");
            builder.HasKey(notificacao => notificacao.Id);
            builder.Property(notificacao => notificacao.Id)
                .ValueGeneratedOnAdd()
            .HasDefaultValueSql("nextval('notificacoes_ids')");
            builder.Property(notificacao => notificacao.DestinatarioCodigo).HasMaxLength(50).IsRequired();
            builder.Property(notificacao => notificacao.ModuloOrigem).HasMaxLength(50).IsRequired();
            builder.Property(notificacao => notificacao.TipoEvento).HasMaxLength(80).IsRequired();
            builder.Property(notificacao => notificacao.Titulo).HasMaxLength(120).IsRequired();
            builder.Property(notificacao => notificacao.Mensagem).HasMaxLength(500).IsRequired();
            builder.Property(notificacao => notificacao.UrlDestino).HasMaxLength(250);
            builder.Property(notificacao => notificacao.ReferenciaExterna).HasMaxLength(120);
            builder.Property(notificacao => notificacao.ChaveIdempotencia).HasMaxLength(160);
            builder.Property(notificacao => notificacao.CorrelacaoId).HasMaxLength(100);
            builder.Property(notificacao => notificacao.DataCriacao).IsRequired();
            builder.HasIndex(notificacao => new { notificacao.DestinatarioCodigo, notificacao.DataExclusao, notificacao.Lida, notificacao.DataCriacao });
            builder.HasIndex(notificacao => new { notificacao.ModuloOrigem, notificacao.ChaveIdempotencia })
                .IsUnique()
                .HasFilter("\"ChaveIdempotencia\" IS NOT NULL");
        });
    }
}
