using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace PersonalTracking.DAL.ModelsConfiguration
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(k => k.Id);
            builder.Property(dpt => dpt.Name).HasMaxLength(50).IsRequired();
        }
    }
}