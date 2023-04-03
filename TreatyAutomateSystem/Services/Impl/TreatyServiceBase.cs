using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;

using static TreatyAutomateSystem.Helpers.TreateConst;
using TreatyAutomateSystem.Helpers;

namespace TreatyAutomateSystem.Services;


public class S{
   
    

}

public class TreatyServiceBase
{
    protected readonly Options _options;
    public record TreatyData (
        string NaOsnovanii, 
        string CompanyName, 
        string PracticeDirector, 
        string CompanyRicvizit
    );
    
    public record Options(
        string FolderPathToSave, 
        string TreatePlatePath
    );

    public TreatyServiceBase(Options options)
    {
        _options = options;
    }
    
    

    protected WordprocessingDocument InsertBaseDataToNewCopyOfDocument(TreatyData data)
    {
        var doc = GetCopyOfDocument(_options.TreatePlatePath);

        InsertCompanyDataToParagraph(doc, data);
        
        return doc;

        
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
    protected async Task<Stream> SaveDocAsStream(WordprocessingDocument doc, TreatyData data)
    {
        var savedPath = SaveDoc(doc, data);

        return await GetStreamAndDeleteFile(savedPath);
    }
    WordprocessingDocument GetCopyOfDocument(string path)
    {
        var original = WordprocessingDocument.Open(_options.TreatePlatePath, true);
        var doc = (WordprocessingDocument)original.Clone();
        original.Close();
        return doc;
    }

    protected record InsertionDataArguments<DataType>(Body Body, DataType Data,string DataName, string[] Regexs);

    



    void InsertNewText(OpenXmlElement paragraph, string insertText)
    {
        var run = paragraph.First(r => r is Run);
        
        var existText = run.FirstOrDefault(p => p is Text);

        if(existText is not null)
            existText.Remove();

        var text = new Text(insertText);
        run.AppendChild(text);
    }
    protected void InsertDataToEmptyTableCellInRowNearWithMatchedCell(InsertionDataArguments<string> arg)
    {
        var cell = CellWhereInsertDataNearWithRegex(arg.Body, arg.Regexs) ?? throw new AppExceptionBase($"не найдена таблица подходяшая для {arg.DataName}");
        var par = (Paragraph)cell.First(p => p is Paragraph);
        InsertNewText(par, arg.Data);
    }
    
    OpenXmlElement? CellWhereInsertDataNearWithRegex(Body body, string[] nearRegexs)
    {
        var table = body.FirstOrDefault(e => e is Table && e.InnerText.HasAnyRegexSignature(nearRegexs));
        var row = table?.FirstOrDefault(r => r is TableRow && r.InnerText.HasAnyRegexSignature(nearRegexs));
        var cell = row?
            .SkipWhile(c => c is not TableCell && !c.InnerText.HasAllRegexSignature(nearRegexs))
            .FirstOrDefault(s => s is TableCell && !s.InnerText.HasAllRegexSignature(nearRegexs));
        return cell;
    }
   


    void InsertCompanyDataToParagraph(WordprocessingDocument doc, TreatyData data)
    {
        var body = GetBodyOfDocument(doc);

        InsertDataToEmptyTableCellInRowNearWithMatchedCell(new InsertionDataArguments<string>(body, data.CompanyRicvizit, "Реквизит", DgtuRecvizitRegex));

        InsertDataToParagraphInsteadUnderlineWhereHasMatchedString(new InsertionDataArguments<string>(body, data.CompanyName, "стороны, и_" ,new [] {PARAGRAPH_PART_FOR_COMPANY_NAME_REGEX}));

        InsertDataToParagraphInsteadUnderlineWhereHasMatchedString(new InsertionDataArguments<string>(body, data.NaOsnovanii, "основании_", new [] {PARAGRAPH_PART_FOR_NA_OSNOVANII}));

        InsertDataToParagraphInsteadUnderlineWhereHasMatchedString(new InsertionDataArguments<string>(body, data.PracticeDirector, "лице_", new [] {PARAGRAPH_PART_FOR_PRACTICE_DIRECTOR_NAME}));
    }

    protected void InsertDataToParagraphInsteadUnderlineWhereHasMatchedString(InsertionDataArguments<string> arg)
    {
        var p = arg.Body.FirstOrDefault(s => s is Paragraph && s.InnerText.HasAnyRegexSignature(arg.Regexs)) ?? throw new AppExceptionBase($"не найден параграф для {arg.DataName}");
        var run = p.First(r => r is Run && r.InnerText.HasAnyRegexSignature(arg.Regexs));

        ReplaceUnderlineText(run, arg.Data, arg.Regexs);
    }

    void ReplaceUnderlineText(OpenXmlElement run, string data, string[] regexs)
    {
        var text = run.First(t => t is Text && t.InnerText.HasAnyRegexSignature(regexs));
        text.Remove();
        var newText = new Text(GenerateCompDataText(text.InnerText, data, regexs));
        run.AppendChild(newText);
    }
    string GenerateCompDataText(string whereInsert, string data, string[] regexs)
    {
        var dataMatched = whereInsert.GiveFirstMatchedRegex(regexs:regexs);
        var forDataGeneratedText = InsertDataInsteadOfUnderDash(dataMatched.Value, data);

        var res = whereInsert
            .Replace(dataMatched.Value, forDataGeneratedText);
            
        return res;
    }
    string InsertDataInsteadOfUnderDash(string whereInsert, string data)
    {
        var match = UnderDashMatch(whereInsert);

        var underDash = match.Value;

        return whereInsert.Replace(underDash, $" {data}");
    }
    protected Match UnderDashMatch(string whereInsert) => Regex.Match(whereInsert, UNDER_DASH_REGEG);
    
    
    

    protected void InsertDataInNextAfterMatchedRegexRow(InsertionDataArguments<string[]> args)
    {
        var cells = CellsWhereInsertStudentData(args.Body, args.Regexs) ?? throw new AppExceptionBase($"не найдена таблица для {args.DataName}");
        
        if(cells.Count() != args.Data.Length)
            throw new AppExceptionBase($"Проверьте таблицу для {args.DataName}");
        // мы горизонтально проходим по таблице и вставляем туда данные
        // в правильном порядке
        int i = 0;
        foreach(var cell in cells)
        {
            var cellParagraph = (Paragraph)cell.First(p => p is Paragraph);
            InsertNewText(cellParagraph, args.Data[i]);
            i++;
        }
    }
    IEnumerable<OpenXmlElement>? CellsWhereInsertStudentData(Body body, string[] nearRegexs)
    {
        var table = body.FirstOrDefault(e => e is Table && e.InnerText.HasAnyRegexSignature(nearRegexs));
        var row = table?
            .SkipWhile(r => r is not TableRow && !r.InnerText.HasAnyRegexSignature(nearRegexs))
            .FirstOrDefault(r => r is TableRow && !r.InnerText.HasAnyRegexSignature(nearRegexs));
        var cells = row?.Where(s => s is TableCell);
        return cells;
    }

    

    protected Body GetBodyOfDocument(WordprocessingDocument doc)
        => doc?.MainDocumentPart?.Document.Body ?? throw new NullReferenceException("body of treat is null");


    
   
   


    string SaveDoc(WordprocessingDocument doc, TreatyData studentToGenerateDocName)
    {
        var pathToSave = PathToSave(studentToGenerateDocName);
        using var doc2 = doc.SaveAs(pathToSave);
        return pathToSave;
    }
    
    string PathToSave(TreatyData student) => 
        Path.Combine(
            _options.FolderPathToSave, 
            GetFileName(student)
        );
    
    protected virtual string GetFileName(TreatyData data)
    {
        var extToSave = Path.GetExtension(_options.TreatePlatePath);
        var fileName = $"{data.CompanyName}{extToSave}";
        return fileName;
    }
}
