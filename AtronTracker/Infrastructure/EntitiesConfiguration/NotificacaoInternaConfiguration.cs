using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class NotificacaoInternaConfiguration : IEntityTypeConfiguration<NotificacaoInterna>
    {
        public void Configure(EntityTypeBuilder<NotificacaoInterna> builder)
        {
            builder.HasKey(key => key.Id);

            builder.Property(ntf => ntf.UsuarioCodigo).IsRequired().HasMaxLength(10);
            builder.Property(ntf => ntf.Titulo).IsRequired().HasMaxLength(120);
            builder.Property(ntf => ntf.Mensagem).IsRequired().HasMaxLength(500);
            builder.Property(ntf => ntf.Modulo).IsRequired().HasMaxLength(50);
            builder.Property(ntf => ntf.TipoEvento).IsRequired().HasMaxLength(80);
            builder.Property(ntf => ntf.UrlDestino).HasMaxLength(250).IsRequired(false);
            builder.Property(ntf => ntf.Lida).IsRequired().HasDefaultValue(false);
            builder.Property(ntf => ntf.DataCriacao).IsRequired();
            builder.Property(ntf => ntf.DataLeitura).IsRequired(false);

            builder.HasIndex(ntf => new { ntf.UsuarioId, ntf.UsuarioCodigo, ntf.Lida });
            builder.HasIndex(ntf => ntf.TarefaId);

            builder.HasOne(ntf => ntf.Usuario)
                   .WithMany()
                   .HasForeignKey(ntf => new { ntf.UsuarioId, ntf.UsuarioCodigo })
                   .HasPrincipalKey(usr => new { usr.Id, usr.Codigo })
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ntf => ntf.Tarefa)
                   .WithMany()
                   .HasForeignKey(ntf => ntf.TarefaId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);
        }
    }
}
