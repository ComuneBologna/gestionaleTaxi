using Asf.Taxi.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using SmartTech.Common.ApplicationUsers;
using SmartTech.Common.ApplicationUsers.Entities;

namespace Asf.Taxi.DAL
{
    public class TaxiDriverDBContext : CustomUsersDBContext<TaxiDriverDBContext, UserEntity, TaxiAuthorityUserEntity>
    {
        public TaxiDriverDBContext(DbContextOptions<TaxiDriverDBContext> options)
            : base(options)
        {
        }

        public DbSet<LicenseeEntity> Licensees { get; set; }

        public DbSet<LicenseesIssuingOfficeEntity> LicenseesIssuingOffices { get; set; }

        public DbSet<FinancialAdministrationEntity> FinancialAdministrations { get; set; }

        public DbSet<DocumentEntity> Documents { get; set; }

        public DbSet<TaxiDriverAssociationEntity> TaxiDriverAssociations { get; set; }

        public DbSet<VehicleEntity> Vehicles { get; set; }

        public DbSet<TaxiDriverEntity> Drivers { get; set; }

        public DbSet<TaxiDriverSubstitutionEntity> DriverSubstitutions { get; set; }

        public DbSet<SubShiftEntity> SubShifts { get; set; }

        public DbSet<ShiftEntity> Shifts { get; set; }

        public DbSet<CalendarShiftEntity> CalendarShifts { get; set; }

        public DbSet<AuditEntity> Audits { get; set; }

        public DbSet<LicenseeHistoryEntity> LicenseesHistory { get; set; }

        //public DbSet<ImportEntity> TaxiImports { get; set; }

        //public DbSet<ErrorEntity> Errors { get; set; }

        public DbSet<LicenseeTaxiDriverEntity> LicenseesTaxiDrivers { get; set; }

        public DbSet<TemplateEntity> Templates { get; set; }

        public DbSet<RequestsRegisterEntity> RequestsRegisters { get; set; }

        public DbSet<ProcessCodeEntity> ProcessCodes { get; set; }

        public DbSet<EmailEntity> ProtocolEmails { get; set; }

        public DbSet<RecipientEntity> Recipients { get; set; }

        public DbSet<CredentialEntity> Credentials { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors().EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxiDriverEntity>()
                .HasMany(sr => sr.Substitutions)
                .WithOne(x => x.DriverTo);

            modelBuilder.Entity<TaxiAuthorityUserEntity>().HasNoDiscriminator();

            modelBuilder.Entity<LicenseeEntity>()
                .HasMany(sr => sr.CalendarShifts)
                .WithOne(dst => dst.Licensee);

            modelBuilder.Entity<LicenseeTaxiDriverEntity>()
                .HasKey(entity => new
                {
                    entity.AuthorityId,
                    entity.LicenseeId,
                    entity.TaxiDriverId,
                    entity.LicenseeType,
                    entity.TaxiDriverType
                });

            modelBuilder.Entity<ProcessCodeEntity>().Property(m => m.FullTextSearch)
                .HasComputedColumnSql("[Code] + ' ' + [Description]");

            base.OnModelCreating(modelBuilder);
        }
    }
}