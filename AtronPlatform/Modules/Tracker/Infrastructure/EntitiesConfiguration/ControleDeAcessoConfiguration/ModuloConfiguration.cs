using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration.ControleDeAcessoConfiguration
{
    public class ModuloConfiguration : IEntityTypeConfiguration<Modulo>
    {
        public void Configure(EntityTypeBuilder<Modulo> builder)
        {
            builder.HasKey(mod => new { mod.Id, mod.Codigo });
            builder.Property(mod => mod.Id).ValueGeneratedOnAdd();

            builder.Property(mod => mod.Codigo).IsRequired().HasMaxLength(10);
            builder.Property(mod => mod.Descricao).IsRequired().HasMaxLength(100);


            builder.HasMany(mod => mod.PerfilDeAcessoModulos)
                   .WithOne(pam => pam.Modulo)
                   .HasForeignKey(pam => new { pam.ModuloId, pam.ModuloCodigo });        

            builder.HasData(
            new Modulo { Id = 1, Codigo = "DPT", Descricao = "Departamentos" },
            new Modulo { Id = 2, Codigo = "CRG", Descricao = "Cargos" },
            new Modulo { Id = 3, Codigo = "USR", Descricao = "Usuários" },
            new Modulo { Id = 4, Codigo = "TAR", Descricao = "Tarefas" },
            new Modulo { Id = 6, Codigo = "PERF", Descricao = "Perfil de acesso" },
            new Modulo { Id = 10, Codigo = "RPERFUSR", Descricao = "Relacionamento de perfil e usuários" },
            new Modulo { Id = 11, Codigo = "PLC", Descricao = "Planejamento de Custos" },
            new Modulo { Id = 12, Codigo = "CAT", Descricao = "Categorias" });
        }
    }
}
