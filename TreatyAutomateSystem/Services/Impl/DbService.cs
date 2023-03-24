using TreatyAutomateSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace TreatyAutomateSystem.Services;
public class DbService
{
    
    readonly TasDbContext _dbContext;
    public DbService(TasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UploadManyGroups(IEnumerable<Group> groups)
    {
        foreach(var group in groups)
        {
            await AddOrUpdateGroup(group);
        }
    }
    
    public async Task AddOrUpdateGroup(Group group)
    {
        await PrepareGroup(group);

        var fGroup = await _dbContext.Groups
            .Include(i => i.Speciality)
            .Include(i => i.Students)
            .FirstOrDefaultAsync(g => g.Name == group.Name);
        
        
        
        if(fGroup is null)
            await _dbContext.Groups.AddAsync(group);
        else
        {   
            UpdateGroupFromData(fGroup, group);
            fGroup.Students = group.Students;
            _dbContext.Update(fGroup);
        }
        await _dbContext.SaveChangesAsync();
    }
    async Task PrepareGroup(Group group)
    {
        var fSpec = await _dbContext.Specialities.FindAsync(group.Speciality.Code);

        if(fSpec is not null)
            group.Speciality = fSpec;
    }

    void UpdateGroupFromData(Group toUpdate, Group data)
    {
        if(data.Students.Count > 0)
        {
            _dbContext.Students.RemoveRange(toUpdate.Students);
            toUpdate.Students = data.Students;
        }

        if(data.CourseNum is not null)
            toUpdate.CourseNum = data.CourseNum;
        
        if(data.PracticeType is not null)
            toUpdate.PracticeType = data.PracticeType;

        if(data.Facultative is not null)
            toUpdate.Facultative = data.Facultative;

        if(data.PracticeEnd is not null)
            toUpdate.PracticeEnd = data.PracticeEnd;
        
        if(data.PracticeStart is not null)
            toUpdate.PracticeEnd = data.PracticeStart;
        
        if(data.PracticeType is not null)
            toUpdate.PracticeType = data.PracticeType;
    }
    public Task<IEnumerable<string>> FindStudentsByQuery(string query)
    {
        var toLowQ = query.ToLower();
        var students = _dbContext.Students.Include(s => s.Group)
            .Where(f => 
                f.Fio == query ||
                f.Fio.Contains(query)
            ).Take(10).Select(s => $"{s.Fio}({s.Group.Name})");
        return Task.FromResult((IEnumerable<string>)students);
    }
    
}