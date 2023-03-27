using TreatyAutomateSystem.Models;
namespace TreatyAutomateSystem.Services;

public class TreateManager
{
    readonly DbService _service;
    readonly StudentTreateService _treateService;
    public TreateManager(DbService service, StudentTreateService treateService)
    {
        _service = service;
        _treateService = treateService;
    }
    public async Task<Stream> GenerateOneProfileTreateTypeDocument(string id, string companyName)
    {
        var student = await _service.FindStudentById(id);
        var company = await _service.FindCompanyByName(companyName);

        var studentData = GetStudentData(student, company, student.Group, student.Group.Speciality);
        
        var res = await _treateService.InsertDataToTreate(studentData);
        return res;
    }
    StudentTreateService.StudentData GetStudentData(Student student, Company company, Group group, Speciality speciality) =>
        new StudentTreateService.StudentData  {
            Name = student.Fio,
            Speciality = speciality.Name,
            PracticeType = group.PracticeType ?? "",
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