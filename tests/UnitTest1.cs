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
    public  void Test1()
    {
        var p = new OrganizationDataParser();
        using var file = System.IO.File.OpenRead(@"/Users/macbook/Documents/dotnet/TAS/tests/Реквизиты предприятий.xls");
        var res = p.ParseExcel(file);
    }
}