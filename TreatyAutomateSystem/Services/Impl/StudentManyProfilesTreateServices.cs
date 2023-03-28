using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;


namespace TreatyAutomateSystem.Services;

public class CompanyManyprofilesTreateService
{
    readonly Options _options;
    public class CompanyData
    {
        
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
    public CompanyManyprofilesTreateService(Options options)
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
    

    public async Task<Stream> InsertDataToTreate(CompanyData student)
    {
        using var doc = GetCopyOfDocument(_options.TreatePlatePath);
        
        InsertData(doc, student);

        var savedDoc = SaveDoc(doc, student);

        return await GetStreamAndDeleteFile(savedDoc);
    }
    void InsertData(WordprocessingDocument doc, CompanyData student)
    {
        InsertCompanyDataToParagraph(doc, student);
        InsertCompDataToTable(doc, student);
    }
    async Task<Stream> GetStreamAndDeleteFile(string path)
    {
        MemoryStream streamRes = new MemoryStream();
        using var fileStream = File.OpenRead(path);
        await fileStream.CopyToAsync(streamRes);
        File.Delete(path);
        streamRes.Position = 0;
        return streamRes;
    }

    WordprocessingDocument GetCopyOfDocument(string path)
    {
        var original = WordprocessingDocument.Open(_options.TreatePlatePath, true);
        var doc = (WordprocessingDocument)original.Clone();
        original.Close();
        return doc;
    }

    
    void InsertCompDataToTable(WordprocessingDocument doc, CompanyData student)
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
    HasAnyRegexSignature(
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


    


    void InsertCompanyDataToParagraph(WordprocessingDocument doc, CompanyData student)
    {
        var body = GetBodyOfDocument(doc);

        var p = (Paragraph)body.First(s => s is Paragraph && IsMatchedForCompanyData(s.InnerText));
        var run = (Run)p.First(r => r is Run && IsMatchedForCompanyData(r.InnerText));

        ReplaceUnderlineText(run, student);
    }
    void ReplaceUnderlineText(Run run, CompanyData studentData)
    {
        var text = run.First(t => t is Text && IsMatchedForCompanyData(t.InnerText));
        text.Remove();
        var newText = new Text(GenerateCompDataText(text.InnerText, studentData));
        run.AddChild(newText);
    }
    

    string GenerateCompDataText(string whereInsert, CompanyData studentData)
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
    

    
    bool HasAnyRegexSignature(string data, params string[] regexs)
    {
        foreach(var regex in regexs)
            if(new Regex(regex, RegexOptions.IgnoreCase).Match(data).Success)
                    return true;
        return false;
    }

    Body GetBodyOfDocument(WordprocessingDocument doc)
        => doc?.MainDocumentPart?.Document.Body ?? throw new NullReferenceException("body of treat is null");


    
    


    string SaveDoc(WordprocessingDocument doc, CompanyData studentToGenerateDocName)
    {
        var pathToSave = PathToSave(studentToGenerateDocName);
        using var doc2 = doc.SaveAs(pathToSave);
        return pathToSave;
    }
    string PathToSave(CompanyData student) => 
        Path.Combine(
            _options.FolderPathToSave, 
            GetFileName(student)
        );
    string GetFileName(CompanyData student)
    {
        var extToSave = Path.GetExtension(_options.TreatePlatePath);
        var fileName = $"{student.CompanyName}{extToSave}";
        return fileName;
    }
}