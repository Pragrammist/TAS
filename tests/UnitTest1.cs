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
        FolderPathToSave =  @"/Users/macbook/Documents/dotnet/TAS/tests",
        TreatePlatePath = @"/Users/macbook/Documents/dotnet/TAS/tests/Образец индивидуального договора СПО.docx"

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
    public  async Task Test1()
    {
        var t = new StudentTreateService(opt);
        using var geneartedTreat = await t.InsertDataToTreate(student);

        
    }
}