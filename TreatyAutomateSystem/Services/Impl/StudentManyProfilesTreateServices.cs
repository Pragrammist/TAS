using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;


namespace TreatyAutomateSystem.Services;

public class ManyprofilesTreatyService : TreatyServiceBase
{
    
    
    public ManyprofilesTreatyService(Options options) : base(options)
    {
        
    }


    public override async Task<Stream> InsertDataToTreaty(TreatyData data)
    {
        using var doc = InsertBaseDataToNewCopyOfDocument(data);

        return await SaveDocAsStream(doc, data);
    }

    
}