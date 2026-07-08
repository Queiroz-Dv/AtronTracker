using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(usr => new { usr.Id, usr.Codigo });
            builder.Property(usr => usr.Id).ValueGeneratedOnAdd();

            builder.Property(usr => usr.Codigo).IsRequired().HasMaxLength(10);
            builder.Property(usr => usr.Nome).IsRequired().HasMaxLength(25);
            builder.Property(usr => usr.Sobrenome).IsRequired().HasMaxLength(50);
            builder.Property(usr => usr.Email).IsRequired().HasMaxLength(50);
            builder.Property(usr => usr.DataNascimento);
            builder.Property(usr => usr.Inativo).IsRequired().HasDefaultValue(false);
            builder.Property(usr => usr.ReceberNotificacaoInternaTarefa).IsRequired().HasDefaultValue(true);
            builder.Property(usr => usr.ReceberNotificacaoTarefaPorEmail).IsRequired().HasDefaultValue(false);
            builder.Property(usr => usr.CodigoReativacao).HasMaxLength(6).IsRequired(false);
            builder.Property(usr => usr.GestorImediatoCodigo).HasMaxLength(10).IsRequired(false);

            builder.HasOne(usr => usr.GestorImediato)
                   .WithMany(usr => usr.SubordinadosDiretos)
                   .HasForeignKey(usr => new { usr.GestorImediatoId, usr.GestorImediatoCodigo })
                   .HasPrincipalKey(usr => new { usr.Id, usr.Codigo })
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);
        }
    }
}
