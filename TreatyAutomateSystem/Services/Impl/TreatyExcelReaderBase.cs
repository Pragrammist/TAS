using System.Data;
using Excel;
using TreatyAutomateSystem.Helpers;
using TreatyAutomateSystem.Models;
using static TreatyAutomateSystem.Helpers.RegexConsts;

namespace TreatyAutomateSystem.Services;

public abstract partial class TreatyExcelReaderBase
{
    protected abstract int FirstSheetIndex { get; }

    protected abstract int[] RowIndexes { get; }

    protected abstract int[] ColumnIndexes { get; }

    protected TreatyExcelReaderBase()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }
    
    protected record ReadColumnAsEnumarableOptions 
    (
        int RowCellStart, 
        int ColumnIndex, 
        bool IsToLower = true, 
        bool IsTrimEndings = true, 
        bool IsNormolizeEmpties = true,
        bool IsRemoveAllSpaces = false
    );
    

    protected virtual void CheckTableForIndexex(DataTable studentTable)
    {
        for(int i = 0; i < RowIndexes.Length; i++)
            if(RowIndexes[i] >= studentTable.Rows.Count)
                throw new AppExceptionBase($"Документ имеет не достаточное число строк. " +
                $"Не найденная под номером: {RowIndexes[i] + 1}");
        
        for(int i = 0; i < ColumnIndexes.Length; i++)
            if(ColumnIndexes[i] >= studentTable.Columns.Count)
                throw new AppExceptionBase($"Документ имеет не достаточное число столбцов. " +
                $"Не найденная под номером: {ColumnIndexes[i] + 1}");
    }

    protected virtual string[] ReadColumnAsEnumarable(DataTable studentTable, ReadColumnAsEnumarableOptions options)
    {
        List<string> listResult = new List<string>();
        var i = options.RowCellStart;
        string? currentCell = null;
        do
        {
            currentCell = studentTable.Rows[i][options.ColumnIndex].ToString();
            if(!string.IsNullOrEmpty(currentCell))
                listResult.Add(currentCell);
            
            i++;
        }while(!string.IsNullOrEmpty(currentCell));

        IEnumerable<string> enRes = NormalizeBySettings(listResult, options);

        return enRes.ToArray();
    }
    protected virtual void ValidSheets(DataSet dataSet)
    {
        if(dataSet.Tables.Count < FirstSheetIndex)
            throw new AppExceptionBase("нет первой страницы");
    }
    IEnumerable<string> NormalizeBySettings(IEnumerable<string> enRes, ReadColumnAsEnumarableOptions options)
    {
        if(options.IsToLower)
            enRes = enRes.ToLower();
        if(options.IsTrimEndings)
            enRes = enRes.TrimEndings();
        if(options.IsNormolizeEmpties)
            enRes = enRes.NormolizeEmpties();
        if(options.IsRemoveAllSpaces)
            enRes = enRes.RemoveAllSpaces();
        return enRes;
    }


    protected virtual DataTable GetDataTable(Stream stream)
    {
        using var reader = ExcelReaderFactory.CreateBinaryReader(stream);
        
        var dataSet = reader.AsDataSet();
        ValidSheets(dataSet);

        var studentTable = dataSet.Tables[FirstSheetIndex];

        //CheckTableForIndexex(studentTable);

        return studentTable;
    }
}
//парсер с группами
public abstract partial class TreatyExcelReaderBase
{
    public GroupsParser GetGroupsParser => new GroupsParser();

    public class GroupsParser
    {
        public record GroupArraysAsPassGroupsData(string[] GroupNames, Speciality[] Specialities, string[]? PracticeTypes = null, string[]? PracticeStarts = null, string[]? PracticeEnds = null)
        {
            
            public virtual void ValidData()
            {
                ArraysLengthCheck();
                ValidGroupName();
            }
            void ArraysLengthCheck()
            {
                if(
                    GroupNames.Length == 0 ||
                    GroupNames.Length != Specialities.Length ||
                    (PracticeTypes is not null && GroupNames.Length != PracticeTypes.Length) || 
                    (PracticeStarts is not null && GroupNames.Length != PracticeStarts.Length) ||
                    (PracticeEnds is not null && GroupNames.Length != PracticeEnds.Length)
                ) throw new AppExceptionBase("Данные заполнены не равномерно или пустые");
            }

            void ValidGroupName()
            {
                GroupNames.AnyIsNotMatchedForGroup();
            }
            

        }



        public IEnumerable<Group> GetGroups(GroupArraysAsPassGroupsData data)
        {
            data.ValidData();

            var groupRes = data.GroupNames.Select((groupName,i) => {

                        var group = new Group(
                            speciality: data.Specialities[i],
                            facultative: groupName.ParseFacultativeType(),
                            courseNum: groupName.ParseCourseFromGroup(),
                            name: groupName,
                            prStart: data.PracticeStarts?[i].ParseFromOADateOrString(),
                            prEnd: data.PracticeEnds?[i].ParseFromOADateOrString(),
                            practiceType: data.PracticeEnds?[i].ParsePracticeType()
                        );
                        return group;
                    }
                );
            return groupRes;
        }
    }
}
//парсер для специальностей
public abstract partial class TreatyExcelReaderBase
{
    public SpecialitiesParser GetSpecialitiesParser => new SpecialitiesParser();

    public class SpecialitiesParser
    {
        public record GroupArraysAsPassSpecialitiesData(string[] SpecNames, string[]? SpecCodes = null)
        {
            public virtual void ValidData()
            {
                ArraysLengthCheck();
                ValidSpecialityNameAndCoder();
            }
            void ArraysLengthCheck()
            {
                if(
                    SpecNames.Length == 0 ||
                    (SpecCodes is not null && SpecNames.Length != SpecCodes.Length)
                ) throw new AppExceptionBase("Данные заполнены не равномерно или пустые");
            }

            void ValidSpecialityNameAndCoder()
            {
                if(SpecCodes is null)
                    SpecNames.AnyIsNotMatchedForRegex(SPEC_NAME_WITHD_CODE_REGEX, "название специальности и код специальности");
                else
                {
                    SpecNames.AnyIsNotMatchedForRegex(SPEC_NAME_SYMBOLS_ONLY_REGEX, "название специальности");
                    SpecCodes.AnyIsNotMatchedForRegex(SPEC_CODE_ONLY_PATERN_REGEX, "код специальности");

                }
                
                
            }
            

        }


        
        public IEnumerable<Speciality> GetSpecialities(GroupArraysAsPassSpecialitiesData data)
        {
            data.ValidData();
            
            var specialitesRes = data.SpecNames.Select((specName,i) => {
                        Speciality speciality = new Speciality(
                            code: data.SpecCodes?[i] ?? specName.ParseSpecialityCode(),
                            name:  data.SpecCodes is null ? data.SpecNames[i] : $"{data.SpecCodes[i]} {data.SpecNames[i]}"
                        );

                        
                        return speciality;
                    }
                );
            return specialitesRes;
        }
    }
}
//парсер для студентов
public abstract partial class TreatyExcelReaderBase
{
    public StudentParser GetStudentParser => new StudentParser();

    public class StudentParser
    {
        public record GroupArraysAsPassStudentsData(string[] Fios, string[] StudyConditions, Group Group)
        {
            public virtual void ValidData()
            {
                ArraysLengthCheck();
                ValidFios();
            }
            void ArraysLengthCheck()
            {
                if(
                    Fios.Length == 0 ||
                    Fios.Length != StudyConditions.Length
                ) throw new AppExceptionBase("Данные заполнены не равномерно или пустые");
            }

            void ValidFios()
            {
                Fios.Select(s => s.Replace("ё", "е"))
                    .AnyIsNotMatchedForRegex(FIO_PATERN_REGEX, "ФИО")
                    .ToArray();
                
            }
            

        }


        
        public IEnumerable<Student> FillGroupAndGetStudents(GroupArraysAsPassStudentsData data)
        {
            var fios = data.Fios;
            var stdConds = data.StudyConditions;
            
            var students =  fios.Select((fio,i) => {
                
                var student = new Student(
                    fio: fio,
                    group: data.Group,
                    stdCond: stdConds[i].ParseStudyConditionType()
                );
                return student;
            });
            data.Group.Students = students.ToList();
            return students;
        }
    }
}
