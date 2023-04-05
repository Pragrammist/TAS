using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TreatyAutomateSystem.Models;
using TreatyAutomateSystem.Services;
using TreatyAutomateSystem.Helpers;
using TreatyAutomateSystem.Filters;

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
    readonly GroupWithStudentsExcelReader _parser;
    readonly DbService  _dbService;
    readonly PracticeDataExcelReader _practiceParser;
    readonly OrganizationDataParser _orgParser;
    readonly TreateManager _treateManager;
    public HomeController(GroupWithStudentsExcelReader parser,
    DbService dbService, 
    PracticeDataExcelReader practiceParser,
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
    
    [CustomExceptionFilter]
    [HttpPost("/files/uploadgroup")]
    public async Task<IActionResult> UploadGroupAndStudentsFile(IFormFile file)
    {
        using var excelStream = file.OpenReadStream();
        var group = _parser.ReadExcel(excelStream);
        await _dbService.AddOrUpdateGroup(group);   
        return Ok();
    }


    [CustomExceptionFilter]
    [HttpPost("/files/uploadrikvizit")]
    public async Task<IActionResult> UploadRikvizitFile(IFormFile file)
    {
        using var excelStream = file.OpenReadStream();
        var comps = _orgParser.ParseExcel(excelStream);
        await _dbService.UploadCompanies(comps);
        return Ok();
    }

    [CustomExceptionFilter]
    [HttpPost("/files/uploadpractic")]
    public async Task<IActionResult> UploadPracticDataFile(IFormFile file)
    {
        using var excelStream = file.OpenReadStream();
        var groups = _practiceParser.ReadExcel(excelStream);
        await _dbService.UploadManyGroups(groups);
        return Ok();
    }
    
    [CustomExceptionFilter]
    [HttpGet("/companies")]
    public IActionResult GetCompanies()
    {
        var comps = _dbService.GetNameCompanies();

        return new ObjectResult(comps);
    }

    [CustomExceptionFilter]
    [HttpGet("/students/{query}")]
    public async Task<IActionResult> FindStudentsAsync(string query)
    {
        var students = (await _dbService.FindStudentsByQuery(query)).ToArray();
        if (students.Count() == 0)
            return BadRequest();
        return new ObjectResult(students);
    }
    
    [CustomExceptionFilter]
    [HttpGet("/files/generate")]
    public async Task<IActionResult> GenerateDocx(string studentId, string companyName)
    {    
        var doc = await _treateManager.GenerateOneProfileTreateTypeDocument(studentId, companyName);
        var student = await _dbService.FindStudentById(studentId);
        var file = File(doc, "application/vnd.openxmlformats");
        
        file.FileDownloadName = $"{companyName} {student.Fio}({student.Group.Name}).docx";
        
        return file;
    }

    [CustomExceptionFilter]
    [HttpGet("/files/generatemanyprofile")]
    public async Task<IActionResult> GenerateManyProfilesDocx(string companyName)
    {
        var doc = await _treateManager.GenerateManyTreateTypeDocument(companyName);
        var file = File(doc, "application/vnd.openxmlformats");
        
        file.FileDownloadName = $"{companyName}.docx";
        
        return file;
    }
    public async Task<string> WordAsBase64(Stream stream)
    {
        MemoryStream mem = new MemoryStream();
        await stream.CopyToAsync(mem);
        return "data:application/vnd.openxmlformats;base64," + Convert.ToBase64String(mem.ToArray());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    

}

