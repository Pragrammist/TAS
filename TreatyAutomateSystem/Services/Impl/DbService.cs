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
public class DbService
{
    
    readonly TasDbContext _dbContext;
    public DbService(TasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddOrUpdateGroup(Group group)
    {
        var fGroup = await _dbContext.Groups
            .Include(i => i.Speciality)
            .Include(i => i.Students)
            .FirstOrDefaultAsync(g => g.Name == group.Name);
        
        
        if(fGroup is null)
            await _dbContext.Groups.AddAsync(group);
        else
        {          
            fGroup.Students = group.Students;
            _dbContext.Update(fGroup);
            await _dbContext.SaveChangesAsync();
        }
                 
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> FindStudentsByQuery(string query)
    {
        var toLowQ = query.ToLower();
        var students = _dbContext.Students.Include(s => s.Group)
            .Where(f => 
                f.Fio.ToLower().Contains(toLowQ) || 
                toLowQ.Contains(f.Fio.ToLower()) ||
                f.Fio == query ||
                f.Fio.Contains(query)
            ).Take(10).Select(s => $"{s.Fio}({s.Group.Name})");
        return students;
    }
    
}