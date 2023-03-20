using Xunit;
using TreatyAutomateSystem.Services;
using System.Linq;

namespace tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var parser = new GroupesExcelParser();

        var file = System.IO.File.Open(@"/home/f/Documents/dotnet-app/TAS/tests/test.xls", System.IO.FileMode.Open);
        var students = parser.ParseExcel(file);

        
    }
}