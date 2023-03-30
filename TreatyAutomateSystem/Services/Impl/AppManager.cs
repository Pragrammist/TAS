using TreatyAutomateSystem.Models;
using TreatyAutomateSystem.Helpers;
namespace TreatyAutomateSystem.Services;

public class TreateManager
{
    readonly DbService _service;
    readonly StudentOneprofileTreateService _treateService;
    readonly CompanyManyprofilesTreateService _manyProfileTreateService;
    public TreateManager(
        DbService service, 
        StudentOneprofileTreateService treateService, 
        CompanyManyprofilesTreateService manyProfileTreateService)
    {
        _service = service;
        _treateService = treateService;
        _manyProfileTreateService = manyProfileTreateService;
    }

    public async Task<Stream> GenerateManyTreateTypeDocument(string companyName)
    {
        var company = await _service.FindCompanyByName(companyName);

        var studentData = GetCompanyData(company);
        
        var res = await _manyProfileTreateService.InsertDataToTreate(studentData);
        return res;
    }
    CompanyManyprofilesTreateService.CompanyData GetCompanyData(Company company) => 
        new CompanyManyprofilesTreateService.CompanyData{
            NaOsnovanii = company.NaOsnovanii,
            CompanyName = company.Name,
            PracticeDirector = company.DirectorName,
            CompanyRicvizit = company.Recvizit
        };
    
    public async Task<Stream> GenerateOneProfileTreateTypeDocument(string id, string companyName)
    {
        var student = await _service.FindStudentById(id);
        var company = await _service.FindCompanyByName(companyName);

        var studentData = GetStudentData(student, company, student.Group, student.Group.Speciality);
        
        var res = await _treateService.InsertDataToTreate(studentData);
        return res;
    }
    StudentOneprofileTreateService.StudentData GetStudentData(Student student, Company company, Group group, Speciality speciality) =>
        new StudentOneprofileTreateService.StudentData  {
            Name = student.Fio,
            Speciality = speciality.Name,
            PracticeType = group.PracticeType?.GetValueForTreatyFromDescription() ?? "",
            CourseNum = group.CourseNum ?? "",
            Group = group.Name,
            Start = group.PracticeStart ?? new DateTime(),
            End = group.PracticeEnd ?? new DateTime(),
            NaOsnovanii = company.NaOsnovanii,
            PracticeDirector = company.DirectorName,
            CompanyRicvizit = company.Recvizit,
            CompanyName = company.Name
        };
    
}