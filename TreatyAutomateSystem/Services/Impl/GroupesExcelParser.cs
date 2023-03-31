using TreatyAutomateSystem.Models;
using static TreatyAutomateSystem.Helpers.RegexConsts;
using System.Data;
using Excel;
using TreatyAutomateSystem.Helpers;


namespace TreatyAutomateSystem.Services;
public class GroupsExcelParser 
{
    public GroupsExcelParser()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    
    const int FIRST_SHEET_INDEX = 0;
    const int ROW_CELL_START = 9;
    const int START_FIO_CELL = 2;
    // const int START_COURSE_CELL = 3;
    // const int START_FACUL_CELL = 4;
    const int START_ST_COND_CELL = 5;
    const int START_SPEC_CODE_CELL = 6;
    const int START_SPEC_NAME_CELL = 7;
    const int START_GROUP_CELL = 10;
    
    class StartReading
    {
        public int Row { get; set; }

        public int Column { get; set; }
    }

    public Group ParseExcel(Stream stream)
    {
        using var reader = ExcelReaderFactory.CreateBinaryReader(stream);
        
        var dataSet = reader.AsDataSet();

        var studentTable = dataSet.Tables[FIRST_SHEET_INDEX] 
            ?? throw new ApplicationException("students file is empty");

        CheckTableForIndexex(studentTable);
        
        //var stdConds = Conds(studentTable);
        var group = GetGroup(studentTable);
        FillGroupByStudents(studentTable, group);

        return group;
        
    }
    void FillGroupByStudents(DataTable studentTable, Group group)
    {
        var fios = Fios(studentTable);
        var stdConds = Conds(studentTable);
        
        var students =  fios.Select((fio,i) => {
            var student = new Student(
                fio: fio,
                group: group,
                stdCond: stdConds[i].ParseStudyConditionType()
            );
            return student;
        });
        group.Students = students.ToList();
    }
    void CheckTableForIndexex(DataTable studentTable)
    {
        var rowIndexes = new int[] {
            ROW_CELL_START
        };

        var columnIndexes = new int[]{
            START_FIO_CELL,
            START_ST_COND_CELL,
            START_SPEC_CODE_CELL,
            START_SPEC_NAME_CELL,
            START_GROUP_CELL
        };

        for(int i = 0; i < rowIndexes.Length; i++)
            if(rowIndexes[i] >= studentTable.Rows.Count)
                throw new AppExceptionBase($"Документ имеет не достаточное число строк. " +
                $"Не найденная под номером: {rowIndexes[i] + 1}");
        
        for(int i = 0; i < columnIndexes.Length; i++)
            if(columnIndexes[i] >= studentTable.Columns.Count)
                throw new AppExceptionBase($"Документ имеет не достаточное число столбцов. " +
                $"Не найденная под номером: {columnIndexes[i] + 1}");
    }
    Group GetGroup(DataTable studentTable)
    {
        var fios = Fios(studentTable);
        var specCodes = SpecCodes(studentTable);
        var specNames = SpecNames(studentTable);
        // var faculs = Faculs(studentTable);
        var groups = Groups(studentTable);
        // var courses = Courses(studentTable);
        var group = groups.Select((groupName,i) => {
                    Speciality speciality = new Speciality(
                        name: specNames[i],
                        code: specCodes[i]
                    );

                    var group = new Group(
                        speciality: speciality,
                        courseNum: groupName.ParseCourseFromGroup(),
                        facultative: groupName.ParseFacultativeType(),
                        name: groupName
                    );
                    return group;
                }
            ).First();
        return group;
    }

    string[] Fios(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_FIO_CELL)
        .Select(s => s.Replace("ё", "е"))
        .AnyIsNotMatchedForRegex(FIO_PATERN_REGEX, "ФИО")
        .ToArray();
    
    // string[] Courses(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_COURSE_CELL);

    // string[] Faculs(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_FACUL_CELL);

    string[] Conds(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_ST_COND_CELL);

    string[] SpecCodes(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_SPEC_CODE_CELL)
        .AnyIsNotMatchedForRegex(SPEC_CODE_ONLY_PATERN_REGEX, "код специальности")
        .ToArray();

    string[] SpecNames(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_SPEC_NAME_CELL)
        .AnyIsNotMatchedForRegex(SPEC_NAME_SYMBOLS_ONLY_REGEX, "название специаллности")
        .ToArray();

    string[] Groups(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_GROUP_CELL)
        .RemoveAllSpaces()
        .AnyIsNotMatchedForGroup()
        .ToArray();

    string[] ReadColumnAsEnumarable(DataTable studentTable, int columnIndex)
    {
        List<string> listResult = new List<string>();
        var i = ROW_CELL_START;
        string? currentCell = null;
        do
        {
            currentCell = studentTable.Rows[i][columnIndex].ToString();
            if(!string.IsNullOrEmpty(currentCell))
                listResult.Add(currentCell);
            
            i++;
        }while(!string.IsNullOrEmpty(currentCell));
        return listResult.ToLower().TrimEndings().NormolizeEmpties().ToArray();
    }
}
