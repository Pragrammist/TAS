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
    public HomeController(GroupesExcelParser parser, DbService dbService, PracticeDataExcelParser practiceParser)
    {
        _parser = parser;
        _dbService = dbService;
        _practiceParser = practiceParser;
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
        return new ObjectResult(students);
    }


    [HttpPost("/files/uploadrikvizit")]
    public async Task<IActionResult> UploadRikvizitFile(IFormFile file)
    {
        using var excelStream = file.OpenReadStream();
          
        return new ObjectResult("");
    }

    [HttpPost("/files/uploadpractic")]
    public async Task<IActionResult> UploadPracticDataFile(IFormFile file)
    {
        using var excelStream = file.OpenReadStream();
        var groups = _practiceParser.ParseExcel(excelStream);
        await _dbService.UploadManyGroups(groups);
        return Ok();
    }

    [HttpGet("/students/{query}")]
    public async Task<IActionResult> FindStudentsAsync(string query)
    {
        var students = (await _dbService.FindStudentsByQuery(query)).ToArray();
        if (students.Count() == 0)
            return BadRequest();
        return Ok();
    }
    
    [HttpGet("/files/generate/{nameAndGroup}")]
    public IActionResult GenerateDocx(string nameAndGroup)
    {
        var student = students.FirstOrDefault(s => s.ToLower() == nameAndGroup.ToLower());
        string base64File;
        if (student is null)
            base64File = Convert.ToBase64String(System.IO.File.ReadAllBytes($"wwwroot/notfound.pdf"));
        else
            base64File = Convert.ToBase64String(System.IO.File.ReadAllBytes($"wwwroot/{student}.pdf"));
        var resString = "data:application/pdf;base64," + base64File;
        return Content(resString, "application/pdf");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public static string ConvertToBase64(Stream stream)
    {
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();
        }

        string base64 = Convert.ToBase64String(bytes);
        return base64;
    }

}

