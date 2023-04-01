using System;
using Xunit;
using TreatyAutomateSystem.Services;
using System.Linq;
using System.Threading.Tasks;

namespace tests;

public class UnitTest1
{
    StudentOneprofileTreateService.Options opt = new StudentOneprofileTreateService.Options
    {
        FolderPathToSave =  @"/Users/macbook/Documents/dotnet/TAS/tests",
        TreatePlatePath = @"/Users/macbook/Documents/dotnet/TAS/tests/Образец индивидуального договора СПО.docx"

    };

    StudentOneprofileTreateService.StudentData student = new StudentOneprofileTreateService.StudentData
    {
        CourseNum = "3",

        Group = "547пи-3",

        Name = "Коваленко Виталий Михайлович",

        PracticeType = "Производственная практика",

        Speciality = "09.02.05 Прикладная информатика(по отраслям)",

        Start = DateTime.Parse("09.03.2022"),

        End = DateTime.Parse("09.03.2030"),

        PracticeDirector = "директора Гундарева Сергея Викторовича",

        CompanyRicvizit = $@"ГБУ ДО РО {"\""}СШОР № 13{"\""}
                        Юридический адрес
                        347900, Ростовская область, город Таганрог, ул. Ленина, д. 212-4
                        ИНН 
                        6154065344
                        КПП
                        615401001
                        ОГРН
                        1026102592966
                        ",

        CompanyName = "ГБУ ДО РО \"СШОР № 13\"",

        NaOsnovanii =  "Устава"
    };
    
    [Fact]
    public  void Test1()
    {
        var practiceDataExcelParser = new PracticeDataExcelReader();
        var res = practiceDataExcelParser.ReadExcel(System.IO.File.OpenRead("/home/f/Documents/dotnet-app/TAS/tests/список практик СПО 2022-23.xls"));
        // var groupseExcelPArser = new GroupWithStudentsExcelReader();

        // var res = groupseExcelPArser.ReadExcel(System.IO.File.OpenRead("/home/f/Documents/dotnet-app/TAS/tests/test.xls"));
    
    }
}