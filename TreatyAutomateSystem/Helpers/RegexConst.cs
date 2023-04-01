namespace TreatyAutomateSystem.Helpers;

public static class RegexConsts
{
    public const string FIO_EXCEL_COLUMN_REGEX = @"фамилия.*имя.*отчество";

    public const string FIO_PATERN_REGEX = @"^[а-я]+\s[а-я]+\s[а-я]+$";

    public const string GROUP_PATERN_HITH_REGEX = @"^\d{3}\w?\w?\w?-\d{1,}\w?\w?\w?-\d{1}к?$"; // ВЫШКА

    public const string GROUP_PATERN_SECONDARY_REGEX = @"^\d{3}(-11)?\w{2}-\d{1}к?$"; // СПО

    public const string COURSE_IN_GROUP_PATERN_REGEX = @"\d{1}к?$";

    public const string SPEC_CODE_IN_START_PATERN_REGEX = @"^\d{2}.\d{2}.\d{2}";

    public const string SPEC_CODE_ONLY_PATERN_REGEX = @"^\d{2}.\d{2}.\d{2}$";


    public const string NUMBER_IN_COURSE_PATEREN_REGEX = @"\d";

    public const string SPEC_NAME_SYMBOLS_ONLY_REGEX = @"^(\D\w*\D\s?\D){1,}$";

    public const string RICVIZIT_REGEX = @"(ИНН)*(КПП)*";

    public const string SPEC_NAME_WITHD_CODE_REGEX = @"^\d{2}.\d{2}.\d{2}\s(\D\w*\D\s?\D){1,}$";

    public const string PRACTICE_DIRECTOR_REGEX = @"[A-Я][а-я]+\s[A-Я][а-я]+\s[A-Я][а-я]+";

    public const string NA_OSNOVANII_REGEX = @"(^устава$)*(от\s*\d{2}.\d{2}.\d{4})*";

    public const string COMPANY_NAME_REGEX = @"([А-Я][а-я]+)*([А-Я]{2,})*(«\w*»)*";
}
