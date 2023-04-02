using System;
using Xunit;
using TreatyAutomateSystem.Services;
using System.Linq;
using System.Threading.Tasks;

namespace tests;

public class UnitTest1
{
    TreatyServiceBase.Options opt = new TreatyServiceBase.Options(@"/home/f/Documents/dotnet-app/TAS/tests", @"/home/f/Documents/dotnet-app/TAS/tests/Однопрофильный.docx");

    TreatyServiceBase.TreatyData data = new TreatyServiceBase.TreatyData("Устава", "ГБУ ДО РО \"СШОР № 13\"", "директора Гундарева Сергея Викторовича", $@"ГБУ ДО РО {"\""}СШОР № 13{"\""}
                        Юридический адрес
                        347900, Ростовская область, город Таганрог, ул. Ленина, д. 212-4
                        ИНН 
                        6154065344
                        КПП
                        615401001
                        ОГРН
                        1026102592966
                        ");

    StudentOneprofileTreatyService.StudentTreatyData studentTreatyData = new StudentOneprofileTreatyService.StudentTreatyData("Устава", "ГБУ ДО РО \"СШОР № 13\"", "директора Гундарева Сергея Викторовича", $@"ГБУ ДО РО {"\""}СШОР № 13{"\""}
                        Юридический адрес
                        347900, Ростовская область, город Таганрог, ул. Ленина, д. 212-4
                        ИНН 
                        6154065344
                        КПП
                        615401001
                        ОГРН
                        1026102592966",
                        "Коваленко Виталий Михайлович",
                        "09.02.05 Прикладная информатика(по отраслям)",
                        "Производственная практика",
                        "3",
                        "547пи-3",
                        DateTime.Parse("09.03.2022"),
                        DateTime.Parse("09.03.2030")
                    );

    
    [Fact]
    public  async Task Test1()
    {
        var dbase = new StudentOneprofileTreatyService(opt);
        using var doc = await dbase.InsertDataToTreate(studentTreatyData);

    }
}