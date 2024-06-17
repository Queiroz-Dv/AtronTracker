using Microsoft.EntityFrameworkCore;
using Models;

namespace PersonalTracking.DAL.DataStorage
{
    /// <summary>
    /// Classe padrão para os objetos do repository
    /// </summary>
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        { }

        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
        }
    }
}