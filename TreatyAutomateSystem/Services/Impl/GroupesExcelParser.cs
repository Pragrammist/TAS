using TreatyAutomateSystem.Models;
using System.Linq;
using System.Data;
using Excel;

namespace TreatyAutomateSystem.Services;
public class GroupesExcelParser 
{
    public GroupesExcelParser()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }
    const int FIRST_SHEET_INDEX = 0;
    const int ROW_CELL_START = 9;
    const int START_FIO_CELL = 2;
    const int START_COURSE_CELL = 3;
    const int START_FACUL_CELL = 4;
    const int START_ST_COND_CELL = 5;
    const int START_SPEC_CODE_CELL = 6;
    const int START_SPEC_NAME_CELL = 7;
    const int START_GROUP_CELL = 10;
    


    public IEnumerable<Student> ParseExcel(Stream stream)
    {
        using (var reader = ExcelReaderFactory.CreateBinaryReader(stream))
        {
            var dataSet = reader.AsDataSet();
            var studentTable = dataSet.Tables[FIRST_SHEET_INDEX] ?? throw new InvalidOperationException("students file is empty");
            var fios = Fios(studentTable);
            var courses = Courses(studentTable);
            var faculs = Faculs(studentTable);
            var stdConds = Conds(studentTable);
            var specCodes = SpecCodes(studentTable);
            var specNames = SpecNames(studentTable);
            var groups = Groups(studentTable);
            return fios.Select((fio,i) => {
                Speciality speciality = new Speciality(
                    name: specNames[i],
                    code: specCodes[i]
                );
                Course course = new Course(
                    num: int.Parse(courses[i])
                );

                var group = new Group(
                    speciality: speciality,
                    course: course,
                    name: groups[i]
                );
                return new Student(
                    fio: fio,
                    group: group,
                    facultative: faculs[i].ToLower() == "спо" ? FacultativeType.Sec : FacultativeType.Hgh,
                    stdCond: stdConds[i].ToLower() == "ко" ? StudyConditionType.Pd : StudyConditionType.Ste
                );
            });
        }
    }
    

    string[] Fios(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_FIO_CELL);
    
    string[] Courses(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_COURSE_CELL);

    string[] Faculs(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_FACUL_CELL);

    string[] Conds(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_ST_COND_CELL);

    string[] SpecCodes(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_SPEC_CODE_CELL);

    string[] SpecNames(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_SPEC_NAME_CELL);

    string[] Groups(DataTable studentTable) => ReadColumnAsEnumarable(studentTable, START_GROUP_CELL);

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
        return listResult.ToArray();
    }
}

