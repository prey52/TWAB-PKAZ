using TWAB.Areas.Identity.Data;
using TWAB.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWAB.Models;
using TWAB.Database;

namespace TWAB.Data;

public class TWABIdentityContext : IdentityDbContext<IdentityUser>
{
    private readonly HttpClient _httpClient;

    public TWABIdentityContext(DbContextOptions<TWABIdentityContext> options) : base(options)
    {

    }

    public DbSet<DBUser> dBUsers { get; set; }
    public DbSet<LokalizacjaFirmy> LokalizacjeFirm { get; set; }
    public DbSet<OfertyPracyModel> OfertyPracy { get; set; }
    public DbSet<OfertyPracyBenefity> Benefity { get; set; }
    public DbSet<OfertyPracyWymagania> Wymagania { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DBUser>()
            .HasOne(x => x.CompanyLocalization)
            .WithOne(y => y.Dbuser)
            .HasForeignKey<LokalizacjaFirmy>(l => l.DbuserID);

        //jeden do wielu z benefitami
        modelBuilder.Entity<OfertyPracyModel>()
            .HasMany(j => j.Benefity)
            .WithOne(b => b.OfertaPracy)
            .HasForeignKey(b => b.OfertaPracyId);

        //jeden do wielu z wymaganiami
        modelBuilder.Entity<OfertyPracyModel>()
            .HasMany(j => j.Wymagania)
            .WithOne(b => b.OfertaPracy)
            .HasForeignKey(b => b.OfertaPracyId);

        modelBuilder.ApplyConfiguration(new AppUserEntityConfiguration());
    }

    public class AppUserEntityConfiguration : IEntityTypeConfiguration<DBUser>
    {
        public void Configure(EntityTypeBuilder<DBUser> modelBuilder)
        {
            modelBuilder.Property(x => x.FirstName).HasMaxLength(255);
            modelBuilder.Property(x => x.LastName).HasMaxLength(255);
            modelBuilder.Property(x => x.BirthDate);
            modelBuilder.Property(x => x.CompanyName).HasMaxLength(255);
            modelBuilder.Property(x => x.CompanyLogo);
        }
    }
}