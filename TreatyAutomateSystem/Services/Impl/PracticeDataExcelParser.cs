using TreatyAutomateSystem.Models;
using System.Data;
using Excel;
using TreatyAutomateSystem.Helpers;

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
        var specNames = SpecNames(studentTable);
        var groups = Groups(studentTable);
        var prTypes = PracticeTypes(studentTable);
        var prStarts = PracticeStarts(studentTable);
        var prEnds = PracticeEnds(studentTable);

        var groupRes = groups.Select((groupName,i) => {
                    Speciality speciality = new Speciality(
                        code: specNames[i].ParseSpecialityCode(),
                        name: specNames[i]
                    );

                    var group = new Group(
                        speciality: speciality,
                        facultative: groupName.ParseFacultativeType(),
                        courseNum: groupName.ParseCourseFromGroup(),
                        name: groupName,
                        prStart: prStarts[i].ParseFromOADateOrString(),
                        prEnd: prEnds[i].ParseFromOADateOrString(),
                        practiceType: prTypes[i].ParsePracticeType()
                    );
                    return group;
                }
            );
        return groupRes;
    }

    


    
    string[] PracticeTypes(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_PRACTICE_TYPE_CELL);
    string[] PracticeStarts(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_PRACTICE_START_CELL);
    string[] PracticeEnds(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_PRACTICE_END_CELL);
    string[] SpecNames(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_SPEC_NAME_CELL);

    string[] Groups(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_GROUP_CELL).RemoveAllSpaces().ToArray();

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
        return listResult.TrimEndings().NormolizeEmpties().ToLower().ToArray();
    }
}
