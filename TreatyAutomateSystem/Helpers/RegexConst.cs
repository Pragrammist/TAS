namespace TreatyAutomateSystem.Helpers;

public static class RegexConsts
{
    public const string FIO_EXCEL_COLUMN_REGEX = @"фамилия.*имя.*отчество";

    public const string FIO_PATERN_REGEX = @"^[а-я]*\s[а-я]*\s[а-я]*$";

    public const string GROUP_PATERN_HITH_REGEX = @"^\d{3}\w?\w?\w?-\d{1,}\w?\w?\w?-\d{1}к?$"; // ВЫШКА

    public const string GROUP_PATERN_SECONDARY_REGEX = @"^\d{3}\w{2}-\d{1}к?$"; // СПО

    public const string COURSE_IN_GROUP_PATERN_REGEX = @"\d{1}к?$";

    public const string SPEC_CODE_IN_START_PATERN_REGEX = @"^\d{2}.\d{2}.\d{2}";

    public const string SPEC_CODE_ONLY_PATERN_REGEX = @"^\d{2}.\d{2}.\d{2}$";

    public const string NUMBER_IN_COURSE_PATEREN_REGEX = @"\d";

    public const string SPEC_NAME_SYMBOLS_ONLY_REGEX = @"^(\D\w*\D\s?\D){1,}$";

    public const string STUDY_COND_REGEX = @"к$";
}
