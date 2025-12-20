using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace DAL.DbContext
{
    public class AutoSalonContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AutoSalonContext(DbContextOptions<AutoSalonContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars => Set<Car>();
        public DbSet<CarModel> CarModels => Set<CarModel>();
        public DbSet<TrimLevel> TrimLevels => Set<TrimLevel>();
        public DbSet<EngineSpec> Engines => Set<EngineSpec>();
        public DbSet<TransmissionSpec> Transmissions => Set<TransmissionSpec>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<ExtraService> ExtraServices => Set<ExtraService>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleExtraService> SaleExtraServices => Set<SaleExtraService>();
        public DbSet<TestDrive> TestDrives => Set<TestDrive>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var utcConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<Car>()
                .HasIndex(c => c.Vin)
                .IsUnique();

            modelBuilder.Entity<SaleExtraService>()
                .HasKey(s => new { s.SaleId, s.ExtraServiceId });

            modelBuilder.Entity<SaleExtraService>()
                .HasOne(x => x.Sale)
                .WithMany(s => s.SaleExtraServices)
                .HasForeignKey(x => x.SaleId);

            modelBuilder.Entity<SaleExtraService>()
                .HasOne(x => x.ExtraService)
                .WithMany(e => e.SaleExtraServices)
                .HasForeignKey(x => x.ExtraServiceId);

            modelBuilder.Entity<Sale>()
                .Property(s => s.SaleDate)
                .HasConversion(utcConverter);

            modelBuilder.Entity<TestDrive>()
                .Property(t => t.Slot)
                .HasConversion(utcConverter);

            base.OnModelCreating(modelBuilder);
        }
    }
}
