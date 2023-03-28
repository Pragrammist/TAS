using System;
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
    public IEnumerable<string> GetCompanies() => _dbContext.Companies.Select(s => s.Name);
    public async Task<Group?> FindGroupOrDefault(string groupName) => 
        await _dbContext.Groups
                    .Include(g => g.Students)
                    .Include(g => g.Speciality)
                    .FirstOrDefaultAsync(g => g.Name == groupName);
    public async Task UploadCompanies(IEnumerable<Company> companies)
    {
        foreach(var company in companies)
        {
            var fComp = await _dbContext.Companies.FindAsync(company.Name);
            if(fComp is null)
                await _dbContext.Companies.AddAsync(company);
            else
                UpdateCompanyData(fComp, company);
            await _dbContext.SaveChangesAsync();
        }
    }
    void UpdateCompanyData(Company toUpdate, Company data)
    {
        toUpdate.Name = data.Name;
        toUpdate.DirectorName = data.DirectorName;
        toUpdate.NaOsnovanii = data.NaOsnovanii;
        toUpdate.Recvizit = data.Recvizit;
        
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
            toUpdate.PracticeStart = data.PracticeStart;
        
        if(data.PracticeType is not null)
            toUpdate.PracticeType = data.PracticeType;
    }
    public Task<IEnumerable<FindStudentResult>> FindStudentsByQuery(string query)
{
        var toLowQ = query.ToLower();
        var students = _dbContext.Students.Include(s => s.Group)
            .Where(f => 
                f.Fio == query ||
                f.Fio.Contains(query)
            ).Select(s => new FindStudentResult { Name = s.Fio, Group = s.Group.Name, Id = s.Id }).Take(10);
        return Task.FromResult((IEnumerable<FindStudentResult>)students);
    }   
    
    public async Task<Student> FindStudentById(string id) =>
        await _dbContext.Students.Include(s => s.Group).ThenInclude(g => g.Speciality).FirstAsync(f => f.Id == id);
    
    public async Task<Company> FindCompanyByName(string name) =>
        await _dbContext.Companies.FindAsync(name) 
            ?? throw new NullReferenceException($"Company with name {name} doesn't exists");
}
