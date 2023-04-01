using System.Runtime.CompilerServices;
using TreatyAutomateSystem.Models;
using System.Data;
using Excel;
using TreatyAutomateSystem.Helpers;
using static TreatyAutomateSystem.Helpers.RegexConsts;
using static TreatyAutomateSystem.Services.TreatyExcelReaderBase.GroupsParser;
using static TreatyAutomateSystem.Services.TreatyExcelReaderBase.SpecialitiesParser;

namespace TreatyAutomateSystem.Services;

public class PracticeDataExcelReader : TreatyExcelReaderBase
{
    protected override int FirstSheetIndex => FIRST_SHEET_INDEX;

    protected override int[] RowIndexes => new int[] {
        ROW_CELL_START
    };

    protected override int[] ColumnIndexes => new int[]{
        START_PRACTICE_TYPE_CELL,
        START_SPEC_CODE_CELL,
        START_SPEC_CODE_CELL,
        START_SPEC_NAME_CELL,
        START_GROUP_CELL,
        START_PRACTICE_START_CELL,
        START_PRACTICE_END_CELL
    };

    const int FIRST_SHEET_INDEX = 0;
    const int ROW_CELL_START = 1;

    const int START_GROUP_CELL = 0;
    const int START_SPEC_CODE_CELL = 1;
    const int START_SPEC_NAME_CELL = 1;
    
    const int START_PRACTICE_TYPE_CELL = 4;
    const int START_PRACTICE_START_CELL = 5;
    const int START_PRACTICE_END_CELL = 6;

     
    public IEnumerable<Group> ReadExcel(Stream stream)
    {
        var groupsTable = GetDataTable(stream);
        var specialitiesData = new GroupArraysAsPassSpecialitiesData(
            SpecNames(groupsTable)
        );
        var specialities = GetSpecialitiesParser.GetSpecialities(specialitiesData).ToArray();
        var groupsData = new GroupArraysAsPassGroupsData
        (
            GroupNames(groupsTable),
            specialities,
            PracticeTypes(groupsTable),
            PracticeStarts(groupsTable),
            PracticeEnds(groupsTable)

        );
        return GetGroupsParser.GetGroups(groupsData);
    }
    
    




    
    string[] PracticeTypes(DataTable groupsTable) => ReadColumnAsArray(groupsTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, START_PRACTICE_TYPE_CELL));

    string[] PracticeStarts(DataTable groupsTable) => ReadColumnAsArray(groupsTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, START_PRACTICE_START_CELL));

    string[] PracticeEnds(DataTable groupsTable) => ReadColumnAsArray(groupsTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, START_PRACTICE_END_CELL));

    string[] SpecNames(DataTable groupsTable) => ReadColumnAsArray(groupsTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, START_SPEC_NAME_CELL));

    string[] GroupNames(DataTable groupsTable) => ReadColumnAsArray(groupsTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, START_GROUP_CELL, IsRemoveAllSpaces: true));

}


