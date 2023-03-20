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
    private readonly GroupesExcelParser _parser;

    public HomeController(GroupesExcelParser parser)
    {
        _parser = parser;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("/files/upload")]
    public IActionResult UploadFile(IFormFile file)
    {

        var stream = file.OpenReadStream();
        try{
            var students = _parser.ParseExcel(stream);
        }catch{
            return BadRequest("file is not valid");
        }
        
        return new ObjectResult(students);
    }

    [HttpGet("/students/{query}")]
    public IActionResult FindStudents(string query)
    {
        var res = students.Where(s => s.ToLower().Contains(query.ToLower()));
        if (res.Count() == 0)
            return BadRequest();
        return new ObjectResult(res);
    }
    [HttpGet("/files/generate/{name}")]
    public IActionResult GenerateDocx(string name)
    {
        var student = students.FirstOrDefault(s => s.ToLower() == name.ToLower());
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

