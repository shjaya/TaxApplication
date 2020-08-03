using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MunicipalityTaxApi.Models
{
    public partial class MunicipalityTaxStoreContext : DbContext
    {
        public MunicipalityTaxStoreContext(DbContextOptions<MunicipalityTaxStoreContext> options)
            : base(options)
        {
        }
        public virtual DbSet<MunicipalityTaxDetail> MunicipalityTaxDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MunicipalityTaxDetail>(entity =>
            {
                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Frequency).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
