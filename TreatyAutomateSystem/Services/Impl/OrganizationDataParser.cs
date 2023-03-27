using TreatyAutomateSystem.Models;
using System.Data;
using Excel;

namespace TreatyAutomateSystem.Services;

public class OrganizationDataParser
{
    public OrganizationDataParser()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }
    const int FIRST_SHEET_INDEX = 0;
    const int ROW_CELL_START = 1;
    const int ROW_CELL_VALID_FIELDS = 0;
    const int START_ORG_NAME_CELL = 1;
    const int START_PRACTICE_DIRECTOR_CELL = 2;
    const int START_NA_OSNOVANII_CELL = 3;
    const int START_RECVIZIT_CELL = 4;
    
    


    public IEnumerable<Company> ParseExcel(Stream stream)
    {
        using var reader = ExcelReaderFactory.CreateBinaryReader(stream);
        var dataSet = reader.AsDataSet();
        
        var compTable =  dataSet.Tables[FIRST_SHEET_INDEX] ?? throw new InvalidOperationException("students file is empty");
        
        return GetCompanies(compTable);
    }
    
    IEnumerable<Company> GetCompanies(DataTable compTable)
    {
        var orgNames = OrgNames(compTable);

        var dirNames = DirectorPracticeName(compTable);

        var ricvs = Recvizits(compTable);

        var naOsnovanii = NaOsnovanii(compTable);
        return orgNames.Select((name, i) => new Company{
            Name = orgNames[i],
            Recvizit = ricvs[i],
            DirectorName = dirNames[i],
            NaOsnovanii = naOsnovanii[i]
        });
    }

    string[] NaOsnovanii(DataTable compTable) => ReadColumnAsEnumarable(compTable, START_NA_OSNOVANII_CELL);

    string[] OrgNames(DataTable compTable) => ReadColumnAsEnumarable(compTable, START_ORG_NAME_CELL);

    string[] DirectorPracticeName(DataTable compTable) => ReadColumnAsEnumarable(compTable, START_PRACTICE_DIRECTOR_CELL);

    string[] Recvizits(DataTable compTable) => ReadColumnAsEnumarable(compTable, START_RECVIZIT_CELL);

    string[] ReadColumnAsEnumarable(DataTable compTable, int columnIndex)
    {
        List<string> listResult = new List<string>();
        var i = ROW_CELL_START;
        string? currentCell = null;
        do
        {
            if(i >= compTable.Rows.Count)
                break;
            currentCell = compTable.Rows[i][columnIndex].ToString();
            if(!string.IsNullOrEmpty(currentCell))
                listResult.Add(currentCell);
            
            i++;
        }while(!string.IsNullOrEmpty(currentCell));
        return listResult.ToArray();
    }
}