using DocumentFormat.OpenXml.Wordprocessing;
using static TreatyAutomateSystem.Helpers.TreatyHelpersMethod;

using static TreatyAutomateSystem.Helpers.TreateConst;
using TreatyAutomateSystem.Helpers;

namespace TreatyAutomateSystem.Services;

public class StudentOneprofileTreatyService : TreatyServiceBase
{
    
    public record StudentTreatyData : TreatyData
    {
        public StudentTreatyData(string naOsnovanii, 
            string companyName, 
            string practiceDirector, 
            string companyRicvizit,
            string name,
            string speciality,
            string practiceType,
            string courseNum,
            string group,
            DateTime start, 
            DateTime end) : base(naOsnovanii, companyName, practiceDirector, companyRicvizit)
        {
            Name = name.ToUpperFirstLater();
            Speciality = speciality.ToUpperFirstLaterFirstWord();
            PracticeType = practiceType.ToUpperFirstLaterFirstWord();
            CourseNum = courseNum;
            Group = group;
            Start = start;
            End = end;
        }
        public string Name { get; set; } = null!;

        public string Speciality { get; set; } = null!;

        public string PracticeType { get; set; } = null!;

        public string CourseNum { get; set; } = null!;

        public string Group { get; set; } = null!;

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

    }
    
    
    public StudentOneprofileTreatyService(Options options) : base(options)
    {

    }
    

    public async Task<Stream> InsertDataToTreate(StudentTreatyData student)
    {
        using var doc = InsertBaseDataToNewCopyOfDocument(student);
        
        var body = GetBodyOfDocument(doc);

        InsertStudentData(body, student);

        return await SaveDocAsStream(doc, student);
    }

    void InsertStudentData(Body body, StudentTreatyData student)
    {
        var data = new InsertionDataArguments<string[]>(body, PrepareStudentDataToInsert(student), "данные студента", StudentTableRegex);

        InsertDataInNextAfterMatchedRegexRow(data);
    }

    protected override string GetFileName(TreatyData data)
    {
        var student = (StudentTreatyData)data;
        var extToSave = Path.GetExtension(_options.TreatePlatePath);
        var fileName = $"{student.CompanyName} {student.Name}({student.Group}){extToSave}";
        return fileName;
    }

    string[] PrepareStudentDataToInsert(StudentTreatyData student) => 
        new string[]
            {
                "1",
                student.Speciality,
                student.PracticeType,
                CourseNumAndGroupRightFormat(student),
                student.Name,
                ConvertDate(student.Start),
                ConvertDate(student.End),
            };
    string ConvertDate(DateTime date) => date.ToString("dd.mm.yyyy");
    string CourseNumAndGroupRightFormat(StudentTreatyData student) => $"{student.CourseNum} {student.Group}";
}
