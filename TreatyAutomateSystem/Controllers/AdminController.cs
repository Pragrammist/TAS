using Microsoft.AspNetCore.Mvc;
using TreatyAutomateSystem.Models;
using TreatyAutomateSystem.Services;
using TreatyAutomateSystem.Helpers;
using TreatyAutomateSystem.Filters;

[Route("{controller}")]
public class AdminController : Controller
{   
    const string TEST_DOCS_FOLDER = @"DocsTest/";
    const string DOCS_FOLDER = @"Docs/";
    readonly DbService _dbService;
    public AdminController(DbService dbService)
    {
        _dbService = dbService;
       
    }
    [CustomExceptionFilter]
    [Route("{pageType}")]
    public async Task<IActionResult> Index(AdminPageType pageType, string? group = null)
    {
        Group? foundGroup = null;

        if(group is not null)
            foundGroup = await GetGroup(group);
        
        if(foundGroup is null && group is not null)
            return Content("группа не найдена");

        var model = new AdminPageRouteDataModel 
        {
            Group = foundGroup?.Name,
            PageType = pageType
        };

        return View(model);
    }
    async Task<Group?> GetGroup(string group) => await _dbService.FindGroupOrDefault(group);

    [HttpGet("/groups/{name}")]
    public async Task<IActionResult> GetStudentsFromGroup(string name)
    {
        var group = await _dbService.FindGroupOrDefault(name) ?? throw new NullReferenceException("Группа не найдена");
        foreach(var student in group.Students)
        {
            student.Fio = student.Fio.ToUpperFirstLater();
            student.Group = new Group(
                group.Speciality, 
                group.Name, 
                group.CourseNum, 
                group.Facultative, 
                group.PracticeStart, 
                group.PracticeEnd, 
                group.PracticeType
            );
        }
        return new ObjectResult( 
            value: group
        );
    }
    [HttpGet("/groups")]
    public IActionResult GetGroupsFromGroup()
    {
        var groups = _dbService.GetGroups();
        
        return new ObjectResult( 
            value: groups
        );
    }
    
    [HttpGet("admin/data/companies")]
    public IActionResult GetCompanies()
    {
        var companies = _dbService.GetCompanies();
        
        return new ObjectResult( 
            value: companies
        );
    }

    [HttpGet("admin/data/specialities")]
    public IActionResult GetSpecialities()
    {
        var specialities = _dbService.GetSpecialities();
        
        return new ObjectResult( 
            value: specialities
        );
    }

    [CustomExceptionFilter]
    [HttpPost("admin/files/treaty")]
    public async Task<IActionResult> UploadTreatyTemplate(UploadTreatyModel model)
    {
        if(model.TreatyTemplate is null)
            return BadRequest("вы не выбрали файл"); 

        var testDocsPath = await WriteToTestFolder(model);
        var testRes = await TestTreatyTemplate(testDocsPath, model.TreateType);
        await DeleteFileFromTestAndSaveIfSuccess(testRes, testDocsPath);
        if(testRes.IsSuccess)
            return File(testRes.Stream, "application/vnd.openxmlformats", $"{model.TreateType.GetValueForTreatyFromDescription()}(тест).docx");
        else
            return BadRequest(testRes.Message);
    }
    async Task<string> WriteToTestFolder(UploadTreatyModel model)
    {
        var fileName = GetTemplateProfileName(model);
        
        
        string testDocsPath = TEST_DOCS_FOLDER+fileName;
        using var writeStream = new System.IO.FileStream(testDocsPath, FileMode.OpenOrCreate, FileAccess.Write);
        using var stream = model.TreatyTemplate.OpenReadStream();
        
        await stream.CopyToAsync(writeStream);

        return testDocsPath;
    }

    record TestTreatyResult(Stream Stream, bool IsSuccess = true, string? Message = null);
    async Task<TestTreatyResult> TestTreatyTemplate(string temPath, TreatyType treatyType)
    {
        
        try
        {
            var stream = treatyType == TreatyType.ONE_PROFILE 
            ? await OneProfileTreatyServiceToTest(temPath) 
            : await ManyProfileTreatyServiceToTest(temPath);
            return new TestTreatyResult(stream);
        }
        catch(AppExceptionBase ex)
        {
            return new TestTreatyResult(new MemoryStream(), false, ex.Message);
        }
        catch(Exception ex)
        {
            return new TestTreatyResult(new MemoryStream(), false, $"Неизвестная ошибка проверьте файл {ex.Message}");
        }
        
    }
    async Task DeleteFileFromTestAndSaveIfSuccess(TestTreatyResult res, string treatyPath)
    {
        if(res.IsSuccess)
            await WriteToDocsFolder(treatyPath);
        
        System.IO.File.Delete(treatyPath);
    }
    async Task WriteToDocsFolder(string treatyPath)
    {
        using var fileWriteStream = System.IO.File.OpenWrite(DOCS_FOLDER+Path.GetFileName(treatyPath));
        using var fielReadStream = System.IO.File.OpenRead(treatyPath);
        await fielReadStream.CopyToAsync(fileWriteStream);
    }


    async Task<Stream> OneProfileTreatyServiceToTest(string savedPath) 
    {   
        var service = new StudentOneprofileTreatyService(
                new TreatyServiceBase.Options(TEST_DOCS_FOLDER, savedPath)
            );
        return await service.InsertDataToTreaty(StudentOneprofileTreatyService.StudentTreatyData.GetStudentTestData());
    }
        

    async Task<Stream> ManyProfileTreatyServiceToTest(string savedPath)
    {
        var service = new ManyprofilesTreatyService(
                new TreatyServiceBase.Options(TEST_DOCS_FOLDER, savedPath)
            );

        return await service.InsertDataToTreaty(TreatyServiceBase.TreatyData.GetTestData());
    }
        

    
    string GetTemplateProfileName(UploadTreatyModel model) => $"{model.TreateType.GetValueForTreatyFromDescription()}{Path.GetExtension(model.TreatyTemplate.FileName)}";
}