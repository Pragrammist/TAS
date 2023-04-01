using TreatyAutomateSystem.Models;
using static TreatyAutomateSystem.Helpers.RegexConsts;
using System.Data;
using Excel;
using TreatyAutomateSystem.Helpers;
namespace TreatyAutomateSystem.Services;

public class GroupWithStudentsExcelReader : TreatyExcelReaderBase
{
    const int FIRST_SHEET_INDEX = 0;
    const int ROW_CELL_START = 9;
    const int FIO_CELL_COLUMN = 2;
    const int ST_COND_COLUMN = 5;
    const int SPEC_CODE_COLUMN = 6;
    const int SPEC_NAME_COLUMN = 7;
    const int GROUP_COLUMN = 10;

    protected override int FirstSheetIndex => FIRST_SHEET_INDEX;

    protected override int[] RowIndexes => new int[]{
        ROW_CELL_START
    };

    protected override int[] ColumnIndexes => new int[]{
        FIO_CELL_COLUMN,
        ST_COND_COLUMN,
        SPEC_CODE_COLUMN,
        SPEC_NAME_COLUMN,
        GROUP_COLUMN
    };


    public Group ReadExcel(Stream stream) =>  GetGroupWithStudents (
        GetDataTable(stream)
    );
    

    Group GetGroup(DataTable studentTable) 
    {
        var passData = new GroupsParser.GroupArraysAsPassGroupsData(
            Groups(studentTable),
            GetSpecialities(studentTable)

        );
        var groups = GetGroupsParser.GetGroups(passData);

        return groups.First();
    }
    Speciality[] GetSpecialities(DataTable studentTable)
    {
        var passData = new SpecialitiesParser.GroupArraysAsPassSpecialitiesData(
            SpecNames(studentTable),
            SpecCodes(studentTable)

        );

        return GetSpecialitiesParser.GetSpecialities(passData).ToArray();
    }

    Group GetGroupWithStudents(DataTable studentTable)
    {   
        var group = GetGroup(studentTable);

        var data = new StudentParser.GroupArraysAsPassStudentsData(
            Fios(studentTable),
            Conds(studentTable),
            group
        );

        GetStudentParser.FillGroupAndGetStudents(data);

        return group;
    }

    
    
    string[] Fios(DataTable studentTable) => ReadColumnAsArray(studentTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, FIO_CELL_COLUMN, IsReplaceSomeRussianSymbols: true));
    
    string[] Conds(DataTable studentTable) => ReadColumnAsArray(studentTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, ST_COND_COLUMN));

    string[] SpecCodes(DataTable studentTable) => ReadColumnAsArray(studentTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, SPEC_CODE_COLUMN));

    string[] SpecNames(DataTable studentTable) => ReadColumnAsArray(studentTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, SPEC_NAME_COLUMN));

    string[] Groups(DataTable studentTable) => ReadColumnAsArray(studentTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, GROUP_COLUMN, IsRemoveAllSpaces: true));

    
}


