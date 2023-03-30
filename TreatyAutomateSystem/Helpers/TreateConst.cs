namespace TreatyAutomateSystem.Helpers;

public static class TreateConst
{
    public const string PARAGRAPH_PART_FOR_COMPANY_NAME_REGEX = @"стороны,?\s*и\s*_*\s*";

    public const string PARAGRAPH_PART_FOR_PRACTICE_DIRECTOR_NAME = @"лице\s*_+\s*";

    public const string PARAGRAPH_PART_FOR_NA_OSNOVANII = @"лице\s*_+\s*";

    public const string UNDER_DASH_REGEG = @"\s*_+\s*";

    public static readonly string[] StudentTreateTableRegex = new string[]{
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
                @"счет",
                @"банк",
                @"почта.*@donstu",
                @"инн.кпп",
                @"\d+/\d+",
                @"счет\s*-\s*\d+",
                @"счет\s*–\s*\d+",
                @"телефон.*\d+директор",
                @"\d+0{3,}\d+"
    };
}
