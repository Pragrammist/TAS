using Microsoft.AspNetCore.Mvc;
using TreatyAutomateSystem.Models;
using TreatyAutomateSystem.Services;

[Route("{controller}")]
public class AdminController : Controller
{
    string[] groups = new string[]{
        "547пи-3к",
        "553ис-3к",
        "546юс-3",
    };
    readonly DbService _dbService;
    public AdminController(DbService dbService)
    {
        _dbService = dbService;
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

    StudentDto[] Students => new StudentDto[]
    {
        new StudentDto{
            ConditionType = "Пл",
            Course = 3,
            Fio = "Коваленко Виталий Михайлович",
            Group = "547пи-3к",
            Speciality = "09.02.05 Прикладная информатика",
            Id = 1
        },
        new StudentDto{
            ConditionType = "Пл",
            Course = 3,
            Fio = "Лифиренко Михаил Дмитриевич",
            Group = "547пи-3к",
            Speciality = "09.02.05 Прикладная информатика",
            Id = 1
        },
        new StudentDto{
            ConditionType = "Пл",
            Course = 3,
            Fio = "Алтунин Антон Витальевич",
            Group = "547пи-3к",
            Speciality = "09.02.05 Прикладная информатика",
            Id = 1
        },
    };
}