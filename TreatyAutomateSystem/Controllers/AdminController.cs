using Microsoft.AspNetCore.Mvc;
using TreatyAutomateSystem.Models;
using TreatyAutomateSystem.Services;
using TreatyAutomateSystem.Helpers;

[Route("{controller}")]
public class AdminController : Controller
{
    readonly DbService _dbService;
    readonly StudentOneprofileTreatyService _oneprofileTreatyService;
    readonly ManyprofilesTreatyService _manyprofilesTreatyService;
    public AdminController(DbService dbService, StudentOneprofileTreatyService oneprofileTreatyService, ManyprofilesTreatyService manyprofilesTreatyService)
    {
        _dbService = dbService;
        _oneprofileTreatyService = oneprofileTreatyService;
        _manyprofilesTreatyService = manyprofilesTreatyService;
    }
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

    [HttpPost("admin/files/treaty")]
    public IActionResult UploadTreatyTemplate(UploadTreatyModel model)
    {
        var fileName = GetTemplateProfileName(model);
        //d_manyprofilesTreatyService.InsertDataToTreaty();
        return Content($"{model.TreateType} {model.TreatyTemplate.FileName}");
    }
    string GetTemplateProfileName(UploadTreatyModel model) => $"{model.TreateType.GetValueForTreatyFromDescription()}{Path.GetExtension(model.TreatyTemplate.FileName)}";
}