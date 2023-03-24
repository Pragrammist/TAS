using TreatyAutomateSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace TreatyAutomateSystem.Services;

public class TasDbContext : DbContext
{
    public DbSet<Student> Students { get; set; } = null!;

    public DbSet<Group> Groups { get; set; } = null!;

    public DbSet<Speciality> Specialities { get; set; } = null!;



    public TasDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasKey(k => k.Fio);
        modelBuilder.Entity<Group>().HasKey(g => g.Name);
        modelBuilder.Entity<Speciality>().HasKey(s => s.Code);

        modelBuilder.Entity<Group>().HasOne(g => g.Speciality).WithMany();
        modelBuilder.Entity<Group>().HasMany(g => g.Students).WithOne(s => s.Group);
    }
}
