using System.Security.Cryptography.X509Certificates;
namespace TreatyAutomateSystem.Helpers;

public static class TreateConst
{
    public const string PARAGRAPH_PART_FOR_COMPANY_NAME_REGEX = @"стороны,?\s*и\s*_+\s*";

    public const string PARAGRAPH_PART_FOR_PRACTICE_DIRECTOR_NAME = @"лице\s*_+\s*";

    public const string PARAGRAPH_PART_FOR_NA_OSNOVANII = @"основании\s*_+\s*";


    public const string UNDER_DASH_REGEG = @"\s*_+\s*";

    public static readonly string[] StudentTableRegex = new string[]{
                @"вид\s*практической\s*подготовки",
                @"срок\s*практической\s*подготовки",
                @"шифр\s*специальности\s*",
                @"ф\s*и\s*о\s*обучающегося\s*",
                @"курс",
                @"группа",
                @"окончание",
                @"начало"
    };

    

    public static readonly string[] DgtuRecvizitRegex = new string[]{
                @"ИНН.*6165033136",
                @"КПП.*615443002",
    };
}
