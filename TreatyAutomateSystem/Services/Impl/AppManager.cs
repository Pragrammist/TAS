using TreatyAutomateSystem.Models;
using TreatyAutomateSystem.Helpers;
namespace TreatyAutomateSystem.Services;

public class TreateManager
{
    readonly DbService _service;
    readonly StudentOneprofileTreatyService _treateService;
    readonly ManyprofilesTreatyService _manyProfileTreateService;
    public TreateManager(
        DbService service, 
        StudentOneprofileTreatyService treateService, 
        ManyprofilesTreatyService manyProfileTreateService)
    {
        _service = service;
        _treateService = treateService;
        _manyProfileTreateService = manyProfileTreateService;
    }

    public async Task<Stream> GenerateManyTreateTypeDocument(string companyName)
    {
        var company = await _service.FindCompanyByName(companyName);

        var studentData = GetCompanyData(company);
        
        var res = await _manyProfileTreateService.InsertDataToTreaty(studentData);
        return res;
    }
    ManyprofilesTreatyService.TreatyData GetCompanyData(Company company) => 
        new ManyprofilesTreatyService.TreatyData(company.NaOsnovanii, company.Name, company.DirectorName, company.Recvizit);
    
    public async Task<Stream> GenerateOneProfileTreateTypeDocument(string id, string companyName)
    {
        var student = await _service.FindStudentById(id);
        var company = await _service.FindCompanyByName(companyName);

        var studentData = GetStudentData(student, company, student.Group, student.Group.Speciality);
        
        var res = await _treateService.InsertDataToTreaty(studentData);
        return res;
    }
    StudentOneprofileTreatyService.StudentTreatyData GetStudentData(Student student, Company company, Group group, Speciality speciality) =>
        new StudentOneprofileTreatyService.StudentTreatyData(
                company.NaOsnovanii, 
                company.Name, 
                company.DirectorName, 
                company.Recvizit, 
                student.Fio, 
                speciality.Name, 
                group.PracticeType?.GetValueForTreatyFromDescription() ?? "",
                group.CourseNum ?? "", 
                group.Name, 
                group.PracticeStart ?? new DateTime(), 
                group.PracticeEnd ?? new DateTime()
            );
    
}