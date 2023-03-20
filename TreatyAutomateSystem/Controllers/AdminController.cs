using Microsoft.AspNetCore.Mvc;
using TreatyAutomateSystem.Models;

[Route("{controller}")]
public class AdminController : Controller
{
    string[] groups = new string[]{
        "547пи-3к",
        "553ис-3к",
        "546юс-3",
    };
    public AdminController()
    {
        
    }
    [Route("{pageType}")]
    public IActionResult Index(AdminPageType pageType, string? group = null)
    {
        string? foundGroup = null;

        if(group is not null)
            foundGroup = GetGroup(group);
        
        if(foundGroup is null && group is not null)
            return Content("группа не найдена");

        var model = new AdminPageRouteDataModel 
        {
            Group = foundGroup,
            PageType = pageType
        };

        return View(model);
    }
    string? GetGroup(string query) => groups.FirstOrDefault(g => g.Contains(query));
    
    [HttpGet("/groups/{query}")]
    public IActionResult GetStudentsFromGroup(string query)
    {
        var res = Students.Where(s => s.Group.Contains(query));
        return new ObjectResult( 
            value: res
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