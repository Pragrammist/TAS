using TreatyAutomateSystem.Models;
using System.Data;
using Excel;

namespace TreatyAutomateSystem.Services;

public class PracticeDataExcelParser
{
    public PracticeDataExcelParser()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }
    const int FIRST_SHEET_INDEX = 0;
    const int ROW_CELL_START = 1;

    const int START_GROUP_CELL = 0;
    const int START_SPEC_CODE_CELL = 1;
    const int START_SPEC_NAME_CELL = 1;
    
    const int START_PRACTICE_TYPE_CELL = 4;
    const int START_PRACTICE_START_CELL = 5;
    const int START_PRACTICE_END_CELL = 6;


    public IEnumerable<Group> ParseExcel(Stream stream)
    {
        using var reader = ExcelReaderFactory.CreateBinaryReader(stream);
        var dataSet = reader.AsDataSet();
        var groupesTable = dataSet.Tables[FIRST_SHEET_INDEX] ?? throw new InvalidOperationException("students file is empty");
        return GetGroups(groupesTable);
    }
    
    IEnumerable<Group> GetGroups(DataTable studentTable)
    {
        var specCodes = SpecCodes(studentTable);
        var specNames = SpecNames(studentTable);
        var groups = Groups(studentTable);
        var prTypes = PracticeTypes(studentTable);
        var prStarts = PracticeStarts(studentTable);
        var prEnds = PracticeEnds(studentTable);

        var groupRes = groups.Select((groupName,i) => {
                    Speciality speciality = new Speciality(
                        name: specNames[i],
                        code: specCodes[i]
                    );

                    var group = new Group(
                        speciality: speciality,
                        name: groupName,
                        prStart: DateTime.FromOADate(double.Parse(prStarts[i])),
                        prEnd: DateTime.FromOADate(double.Parse(prEnds[i])),
                        practiceType: prTypes[i]
                    );
                    return group;
                }
            );
        return groupRes;
    }

    


    string[] SpecCodes(DataTable studentTable) => SpecNames(studentTable)
        // берем первое слово в имени, т.к. это код
        .Select(n => n.Split(' ', StringSplitOptions.RemoveEmptyEntries).First())
        .ToArray();
    string[] PracticeTypes(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_PRACTICE_TYPE_CELL);
    string[] PracticeStarts(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_PRACTICE_START_CELL);
    string[] PracticeEnds(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_PRACTICE_END_CELL);
    string[] SpecNames(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_SPEC_NAME_CELL);

    string[] Groups(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_GROUP_CELL);

    string[] ReadColumnAsEnumarable(DataTable studentTable, int columnIndex)
    {
        List<string> listResult = new List<string>();
        var i = ROW_CELL_START;
        string? currentCell = null;
        do
        {
            if(i >= studentTable.Rows.Count)
                break;
            currentCell = studentTable.Rows[i][columnIndex].ToString();
            if(!string.IsNullOrEmpty(currentCell))
                listResult.Add(currentCell);
            
            i++;
        }while(!string.IsNullOrEmpty(currentCell));
        return listResult.ToArray();
    }
}
