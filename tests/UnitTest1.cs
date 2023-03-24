using System;
using Xunit;
using TreatyAutomateSystem.Services;
using System.Linq;
using System.Threading.Tasks;

namespace tests;

public class UnitTest1
{
    StudentTreateService.Options opt = new StudentTreateService.Options
        {
            FolderPathToSave =  @"/home/f/Documents/dotnet-app/TAS/tests/",
            TreatePlatePath = @"/home/f/Documents/dotnet-app/TAS/tests/test.docx"

        };

    StudentTreateService.StudentData student = new StudentTreateService.StudentData
        {
            CourseNum = 3,
            Group = "547пи-3",
            Name = "Коваленко Виталий Михайлович",
            PracticeType = "Производственная практика",
            Speciality = "09.02.05 Прикладная информатика(по отраслям)",
            Start = DateTime.Parse("09.03.2022"),
            End = DateTime.Parse("09.03.2030"), 
        };
    
    [Fact]
    public async Task Test1()
    {
        var p = new PracticeDataExcelParser();
        using var file = System.IO.File.OpenRead(@"/Users/macbook/Documents/dotnet/TAS/tests/список практик СПО 2022-23.xls");
        var res = p.ParseExcel(file);
    }
}