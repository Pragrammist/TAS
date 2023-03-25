using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;


namespace TreatyAutomateSystem.Services;

public class StudentTreateService
{
    readonly Options _options;
    public class StudentData
    {
        public string Name { get; set; } = null!;

        public string Speciality { get; set; } = null!;

        public string PracticeType { get; set; } = null!;

        public int CourseNum { get; set; }

        public string Group { get; set; } = null!;

        public DateTime Start { get; set; }

        public DateTime End { get; set; }


        public string NaOsnovanii { get; set; } = null!;


        public string CompanyName { get; set; } = null!;


        public string PracticeDirector { get; set; } = null!;

        
        public string CompanyRicvizit { get; set; } = null!;
    }
    public class Options
    {
        public string FolderPathToSave { get; set; } = null!;

        public string TreatePlatePath { get; set; } = null!;
    }
    public StudentTreateService(Options options)
    {
        _options = options;
    }
    
    bool IsMatchedForCompanyData(string text) => 
            MatchedForCompanyName(text).Success && 
            MatchedForPracticeDirector(text).Success && 
            MatchedForNaOsnovanii(text).Success;
    
    Match MatchedForCompanyName(string whereInsert) => 
        new Regex(@"стороны,?\s*и\s*_*\s*").Match(whereInsert);
    

    Match MatchedForPracticeDirector(string whereInsert) => 
        new Regex(@"лице\s*_+\s*").Match(whereInsert);
    

    Match MatchedForNaOsnovanii(string whereInsert) => 
        new Regex(@"основании\s*_+").Match(whereInsert);
    

    public async Task<Stream> InsertDataToTreate(StudentData student)
    {
        using var doc = GetCopyOfDocument(_options.TreatePlatePath);
        
        InsertData(doc, student);

        var savedDoc = SaveDoc(doc, student);

        return new MemoryStream(); //await GetStreamAndDeleteFile(savedDoc);
    }
    void InsertData(WordprocessingDocument doc, StudentData student)
    {
        InsertCompanyDataToParagraph(doc, student);
        InsertCompDataToTable(doc, student);
        InsertStudentDataToTable(doc, student);
    }
    async Task<Stream> GetStreamAndDeleteFile(string path)
    {
        MemoryStream streamRes = new MemoryStream();
        using var fileStream = File.OpenRead(path);
        await fileStream.CopyToAsync(streamRes);
        File.Delete(path);
        return streamRes;
    }

    WordprocessingDocument GetCopyOfDocument(string path)
    {
        var original = WordprocessingDocument.Open(_options.TreatePlatePath, true);
        var doc = (WordprocessingDocument)original.Clone();
        original.Close();
        return doc;
    }

    void InsertStudentDataToTable(WordprocessingDocument doc, StudentData student)
    {
        var cells = CellsWhereInsertStudentData(doc);
        var dataToInsert = PrepareStudentDataToInsert(student);
        // мы горизонтально проходим по таблице и вставляем туда данные
        // в правильном порядке
        int i = 0;
        foreach(var cell in cells)
        {
            var cellParagraph = cell.First(p => p is Paragraph);
            var run = cellParagraph.First(r => r is Run);
            var text = new Text(dataToInsert[i]);
            run.AppendChild(text);
            i++;
        }
    }
    void InsertCompDataToTable(WordprocessingDocument doc, StudentData student)
    {
        var cell = CellWhereInsertCompData(doc);
        var par = cell.First(p => p is Paragraph);
        var run = par.First(r => r is Run);
        var text = new Text(student.CompanyRicvizit);
        run.AppendChild(text);
    }
    OpenXmlElement CellWhereInsertCompData(WordprocessingDocument doc)
    {
        var body = GetBodyOfDocument(doc);
        var table =(Table)body.First(e => e is Table && HasCompSignatureData(e));
        var row = table.First(r => r is TableRow && HasCompSignatureData(r));
        var cell = row.First(s => s is TableCell && s.InnerText == string.Empty);
        return cell;
    }
   

    bool HasCompSignatureData(OpenXmlElement element) =>
    IsTableForUserDataMultipleRegex(
            element.InnerText,
                @"счет",
                @"банк",
                @"почта.*@donstu",
                @"инн.кпп",
                @"\d+/\d+",
                @"счет\s*-\s*\d+",
                @"счет\s*–\s*\d+",
                @"телефон.*\d+директор",
                @"\d+0{3,}\d+");


    


    void InsertCompanyDataToParagraph(WordprocessingDocument doc, StudentData student)
    {
        var body = GetBodyOfDocument(doc);

        var p = (Paragraph)body.First(s => s is Paragraph && IsMatchedForCompanyData(s.InnerText));
        var run = (Run)p.First(r => r is Run && IsMatchedForCompanyData(r.InnerText));

        ReplaceUnderlineText(run, student);
    }
    void ReplaceUnderlineText(Run run, StudentData studentData)
    {
        var text = run.First(t => t is Text && IsMatchedForCompanyData(t.InnerText));
        text.Remove();
        var newText = new Text(GenerateCompDataText(text.InnerText, studentData));
        run.AddChild(newText);
    }
    

    string GenerateCompDataText(string whereInsert, StudentData studentData)
    {
        var forCompNameMatched = MatchedForCompanyName(whereInsert);
        var forCompNameGeneratedText = InsertDataInsteadOfUnderDash(forCompNameMatched.Value, studentData.CompanyName);

        var forPrDirNameMatched = MatchedForPracticeDirector(whereInsert);
        var forPrDirNameGeneratedText = InsertDataInsteadOfUnderDash(forPrDirNameMatched.Value, studentData.PracticeDirector);

        var forNaOsnovaniiMatched = MatchedForNaOsnovanii(whereInsert);
        var forNaOsnovaniiGeneratedText = InsertDataInsteadOfUnderDash(forNaOsnovaniiMatched.Value, studentData.NaOsnovanii);

        var res = whereInsert
            .Replace(forCompNameMatched.Value, forCompNameGeneratedText)
            .Replace(forPrDirNameMatched.Value, forPrDirNameGeneratedText)
            .Replace(forNaOsnovaniiMatched.Value, forNaOsnovaniiGeneratedText);
        return res;
    }
    string InsertDataInsteadOfUnderDash(string whereInsert, string data)
    {
        var match = UnderDashMatch(whereInsert);

        var underDash = match.Value;

        return whereInsert.Replace(underDash, $" {data}");
    }
    Match UnderDashMatch(string whereInsert) => new Regex(@"\s*_+\s*").Match(whereInsert);
    IEnumerable<OpenXmlElement> CellsWhereInsertStudentData(WordprocessingDocument doc)
    {
        var body = GetBodyOfDocument(doc);
        var table =(Table)body.First(e => e is Table && HasUserDataSignatureInHeaderOfTable(e));
        var row = table.First(r => r is TableRow && !HasUserDataSignatureInHeaderOfTable(r));
        var cells = row.Where(s => s is TableCell && s.InnerText == string.Empty);
        return cells;
    }

    bool HasUserDataSignatureInHeaderOfTable(OpenXmlElement element) => 
    IsTableForUserDataMultipleRegex(
            element.InnerText,
                @"вид\s*практической\s*подготовки",
                @"срок\s*практической\s*подготовки",
                @"шифр\s*специальности\s*",
                @"ф\s*и\s*о\s*обучающегося\s*",
                @"курс",
                @"группа",
                @"окончание",
                @"начало");
    bool IsTableForUserDataMultipleRegex(string data, params string[] regexs)
    {
        foreach(var regex in regexs)
            if(new Regex(regex, RegexOptions.IgnoreCase).Match(data).Success)
                    return true;
        return false;
    }

    Body GetBodyOfDocument(WordprocessingDocument doc)
        => doc?.MainDocumentPart?.Document.Body ?? throw new NullReferenceException("body of treat is null");


    string[] PrepareStudentDataToInsert(StudentData student) => 
        new string[]
            {
                student.Speciality,
                student.PracticeType,
                CourseNumAndGroupRightFormat(student),
                student.Name,
                ConvertDate(student.Start),
                ConvertDate(student.End),
            };
    string ConvertDate(DateTime date) => date.ToString("dd.mm.yyyy");
    string CourseNumAndGroupRightFormat(StudentData student) => $"{student.CourseNum} {student.Group}";


    string SaveDoc(WordprocessingDocument doc, StudentData studentToGenerateDocName)
    {
        var pathToSave = PathToSave(studentToGenerateDocName);
        using var doc2 = doc.SaveAs(pathToSave);
        return pathToSave;
    }
    string PathToSave(StudentData student) => 
        Path.Combine(
            _options.FolderPathToSave, 
            GetFileName(student)
        );
    string GetFileName(StudentData student)
    {
        var extToSave = Path.GetExtension(_options.TreatePlatePath);
        var fileName = $"{student.Name}({student.Group}){extToSave}";
        return fileName;
    }
}
