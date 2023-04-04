using System;
using Xunit;
using TreatyAutomateSystem.Services;
using System.Linq;
using System.Threading.Tasks;
using static TreatyAutomateSystem.Services.TreatyServiceBase;
using static TreatyAutomateSystem.Services.StudentOneprofileTreatyService;
namespace tests;

public class UnitTest1
{
    TreatyServiceBase.Options opt = new TreatyServiceBase.Options(@"/home/f/Documents/dotnet-app/TAS/tests", @"/home/f/Documents/dotnet-app/TAS/tests/Однопрофильный.docx");

    TreatyServiceBase.TreatyData data => TreatyData.GetTestData();

    StudentOneprofileTreatyService.StudentTreatyData studentTreatyData => StudentTreatyData.GetStudentTestData();

    
    [Fact]
    public  async Task Test1()
    {
        var dbase = new StudentOneprofileTreatyService(opt);
        using var doc = await dbase.InsertDataToTreaty(studentTreatyData);

    }
}