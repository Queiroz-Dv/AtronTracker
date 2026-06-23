using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;

namespace Shared.Infrastructure.Context
{
    public class SharedDbContext(DbContextOptions<SharedDbContext> options) : DbContext(options)
    {
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Historico> Historicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.HasSequence<long>("HistoricoSeq")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedDbContext).Assembly);

            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                {
                    var propertyType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;

                    if (propertyType == typeof(DateTime))
                        property.SetColumnType("timestamp without time zone");
                }

                modelBuilder.Entity<Auditoria>()
                    .Property(a => a.DataCriacao)
                    .HasDefaultValueSql("now() AT TIME ZONE 'UTC'");

                modelBuilder.Entity<Historico>()
                    .Property(h => h.CodigoHistorico)
                    .HasDefaultValueSql("nextval('\"HistoricoSeq\"')");

                modelBuilder.Entity<Historico>()
                    .Property(h => h.DataCriacao)
                    .HasDefaultValueSql("now() AT TIME ZONE 'UTC'");
            }
        }
    }
}
