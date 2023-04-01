using TreatyAutomateSystem.Models;
using System.Data;
using Excel;
using static TreatyAutomateSystem.Services.TreatyExcelReaderBase.CompanyParser;

namespace TreatyAutomateSystem.Services;


public class OrganizationDataParser : TreatyExcelReaderBase
{
    protected override int FirstSheetIndex => FIRST_SHEET_INDEX;

    protected override int[] RowIndexes => new int [] { ROW_CELL_START };

    protected override int[] ColumnIndexes => new int [] { START_ORG_NAME_CELL, START_PRACTICE_DIRECTOR_CELL, START_NA_OSNOVANII_CELL, START_RECVIZIT_CELL };


    const int FIRST_SHEET_INDEX = 0;
    const int ROW_CELL_START = 1;
    const int START_ORG_NAME_CELL = 1;
    const int START_PRACTICE_DIRECTOR_CELL = 2;
    const int START_NA_OSNOVANII_CELL = 3;
    const int START_RECVIZIT_CELL = 4;
    
    


    public IEnumerable<Company> ParseExcel(Stream stream)
    {
        var companiesTable = GetDataTable(stream);

        var data = new GroupArraysAsPassCompaniesData(
            CompNames(companiesTable),
            DirectorPracticeName(companiesTable),
            Recvizits(companiesTable),
            NaOsnovanii(companiesTable)
        );
        
        return GetCompanyParser.GetCompanies(data);
    }
    
    

    string[] NaOsnovanii(DataTable compTable) => ReadColumnAsArray(compTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, START_NA_OSNOVANII_CELL, IsToLower: false));

    string[] CompNames(DataTable compTable) => ReadColumnAsArray(compTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, START_ORG_NAME_CELL, IsToLower: false));

    string[] DirectorPracticeName(DataTable compTable) => ReadColumnAsArray(compTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, START_PRACTICE_DIRECTOR_CELL, IsToLower: false, IsReplaceSomeRussianSymbols: true));

    string[] Recvizits(DataTable compTable) => ReadColumnAsArray(compTable, new ReadColumnAsEnumarableOptions(ROW_CELL_START, START_RECVIZIT_CELL, IsToLower: false));
}
