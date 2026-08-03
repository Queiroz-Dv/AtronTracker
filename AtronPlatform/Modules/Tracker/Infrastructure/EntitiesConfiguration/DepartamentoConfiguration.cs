using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
    {
        public void Configure(EntityTypeBuilder<Departamento> builder)
        {
            builder.HasKey(dpt => new { dpt.Id, dpt.Codigo });
            builder.Property(dpt => dpt.Id).ValueGeneratedOnAdd();

            builder.Property(dpt => dpt.Descricao)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(dpt => dpt.GestorDepartamentoCodigo).HasMaxLength(10).IsRequired(false);

            builder.HasOne(dpt => dpt.GestorDepartamento)
                   .WithMany()
                   .HasForeignKey(dpt => new { dpt.GestorDepartamentoId, dpt.GestorDepartamentoCodigo })
                   .HasPrincipalKey(usr => new { usr.Id, usr.Codigo })
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);
        }
    }
}
