using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TreatyAutomateSystem.Models;
using TreatyAutomateSystem.Services;

namespace TreatyAutomateSystem.Controllers;

public class HomeController : Controller
{
    string[] students = new string[] {
        "Коваленко Виталий Михайлович",
        "Коченко Игнать Юрьевич",
        "Зубенко Михаил Петрович Мафиозии",
        "Перчук Ира Павловна",
        "Карась Юля Игнатьевна"
    };
    readonly GroupesExcelParser _parser;
    readonly DbService  _dbService;
    readonly PracticeDataExcelParser _practiceParser;
    readonly OrganizationDataParser _orgParser;
    readonly TreateManager _treateManager;
    public HomeController(GroupesExcelParser parser,
    DbService dbService, 
    PracticeDataExcelParser practiceParser,
    OrganizationDataParser orgParser,
    TreateManager treateManager)
    {
        _parser = parser;
        _dbService = dbService;
        _practiceParser = practiceParser;
        _orgParser = orgParser;
        _treateManager = treateManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("/files/uploadgroup")]
    public async Task<IActionResult> UploadGroupAndStudentsFile(IFormFile file)
    {
        using var excelStream = file.OpenReadStream();
        var group = _parser.ParseExcel(excelStream);
        await _dbService.AddOrUpdateGroup(group);   
        return Ok();
    }


    [HttpPost("/files/uploadrikvizit")]
    public async Task<IActionResult> UploadRikvizitFile(IFormFile file)
    {
        using var excelStream = file.OpenReadStream();
        var comps = _orgParser.ParseExcel(excelStream);
        await _dbService.UploadCompanies(comps);
        return Ok();
    }

    [HttpPost("/files/uploadpractic")]
    public async Task<IActionResult> UploadPracticDataFile(IFormFile file)
    {
        using var excelStream = file.OpenReadStream();
        var groups = _practiceParser.ParseExcel(excelStream);
        await _dbService.UploadManyGroups(groups);
        return Ok();
    }
    
    [HttpGet("/companies")]
    public IActionResult GetCompanies(string query)
    {
        var comps = _dbService.GetCompanies().ToArray();
        if (students.Length == 0)
            return BadRequest();
        return new ObjectResult(comps);
    }

    [HttpGet("/students/{query}")]
    public async Task<IActionResult> FindStudentsAsync(string query)
    {
        var students = (await _dbService.FindStudentsByQuery(query)).ToArray();
        if (students.Count() == 0)
            return BadRequest();
        return new ObjectResult(students);
    }
    
    [HttpGet("/files/generate")]
    public async Task<IActionResult> GenerateDocx(string studentId, string companyName, TreateType treateType)
    {
        var doc = await _treateManager.GenerateOneProfileTreateTypeDocument(studentId, companyName);

        return File(doc, "officedocument.wordprocessingml.document");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    

}

