using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;
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
    

    public async Task<Stream> InsertDataToTreate(StudentData student)
    {
        var doc = GetCopyOfDocument(_options.TreatePlatePath);

        InsertDataToTable(CellsWhereInsertData(doc), PrepareDataToInsert(student));

        var savedDoc = SaveDoc(doc, student);

        return await GetStreamAndDeleteFile(savedDoc);
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

    void InsertDataToTable(IEnumerable<OpenXmlElement> cells, string[] dataToInsert)
    {
        // мы горизонтально проходим по таблице и вставляем туда данные
        // в правильном порядке
        int i = 0;
        foreach(var cell in cells)
        {
            var cellParagraph = cell.First(p => p is Paragraph);
            var d = dataToInsert[i];
            
            var text = new Text(d);
            var run = new Run(text);
            cellParagraph.AppendChild(run);
            i++;
        }
    }


    IEnumerable<OpenXmlElement> CellsWhereInsertData(WordprocessingDocument doc)
    {
        var body = GetBodyOfDocument(doc);
        var table =(Table)body.First(e => e is Table);
        var row = table.Last(r => r is TableRow);
        var cells = row.Where(s => s.InnerText == string.Empty && s is TableCell);
        return cells;
    }
    Body GetBodyOfDocument(WordprocessingDocument doc)
        => doc?.MainDocumentPart?.Document.Body ?? throw new NullReferenceException("body of treat is null");


    string[] PrepareDataToInsert(StudentData student) => 
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
        var doc2 = doc.SaveAs(pathToSave);
        doc.Close();
        doc2.Close();
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
