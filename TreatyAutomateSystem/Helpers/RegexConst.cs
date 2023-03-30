namespace TreatyAutomateSystem.Helpers;

public static class RegexConsts
{
    public const string FIO_EXCEL_COLUMN_REGEX = @"фамилия.*имя.*отчество";

    public const string FIO_PATERN_REGEX = @"^[а-я]*\s[а-я]*\s[а-я]*$";

    public const string GROUP_PATERN_HITH_REGEX = @"^\d{3}\w?\w?\w?-\d{1,}\w?\w?\w?-\d{1}к?$"; // ВЫШКА

    public const string GROUP_PATERN_SECONDARY_REGEX = @"^\d{3}\w{2}-\d{1}к?$"; // СПО
}
