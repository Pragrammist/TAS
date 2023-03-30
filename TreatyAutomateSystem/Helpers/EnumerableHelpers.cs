using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using TreatyAutomateSystem.Models;

namespace TreatyAutomateSystem.Helpers;

public static class EnumerableHelpers
{
    public static IEnumerable<string> ToLower(this IEnumerable<string> en) => en.Select(s => s.ToLower());

    public static IEnumerable<string> TrimEndings(this IEnumerable<string> en) => en.Select(s => s.TrimEndings());

    public static IEnumerable<string> RemoveAllSpaces(this IEnumerable<string> en) => en.Select(s => s.Replace(" ", string.Empty));

    public static IEnumerable<string> NormolizeEmpties(this IEnumerable<string> en) => en.Select(s => s.NormolizeEmpties());

    public static IEnumerable<string> GiveSpecialityCode(this IEnumerable<string> en) => en.Select(s => s.ParseSpecialityCode());
}

public class AppExceptionBase : Exception
{
    public AppExceptionBase(string m) : base(m){}
    public AppExceptionBase(){}
}

public static class StringRegexHelpers
{
    public static PracticeType ParsePracticeType(this string str)
    {
        if(str.Contains("диплом"))
            return PracticeType.Diploma;
        else if(str.Contains("производ"))
            return PracticeType.Factory;
        else if(str.Contains("учеб"))
            return PracticeType.Factory;
        
        throw new AppExceptionBase($"непонятный тип практики {str}");
    }
    public static string ParseSpecialityCode(this string speciality)
    {
        var match = Regex.Match(speciality, @"^\d{2}.\d{2}.\d{2}");
        
        if(!match.Success)
            throw new AppExceptionBase($"Строка специальности не содержит код в начале {speciality}");
        return match.Value;
    }

    public static string TrimEndings(this string str) => str.TrimEnd().TrimStart();
    public static string NormolizeEmpties(this string str)
    {
        var words = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var sumWords = words.Aggregate((s1, s2)=> s1+ " " + s2).TrimEndings();

        return sumWords;
    }
    // ^\d{3}\w{2}-\d{1}к?$
    public static FacultativeType ParseFacultativeType(this string group)
    {
        if(Regex.IsMatch(group, @"^\d{3}\w{2}-\d{1}к?$"))
            return FacultativeType.Sec;
        else if(Regex.IsMatch(group, @"^\d{3}\w?\w?\w?-\d{1,}\w?\w?\w?-\d{1}к?$"))
            return FacultativeType.Hgh;
        
        throw new AppExceptionBase($"Группа не правильного формата {group}");
    }

    public static string ParseCourseFromGroup(this string str)
    {
        var mCourse = Regex.Match(str, @"\d{1}к?$", RegexOptions.IgnoreCase);
        if(!mCourse.Success)
            throw new AppExceptionBase($"группа не содержит в конце курс {str}");
        return Regex.Match(mCourse.Value, @"\d").Value;
    }
}

public static class RegexConst
{
    public const string FIO_COLUMN_REGEX_1 = @"ф\s*и\s*о\s*обучающегося\s*";

    public const string FIO_COLUMN_REGEX_2 = @"фамилия.*имя.*отчество";

    public const string SPEC_CODE_COLUMN_REGEX_1 = @"шифр\s*специальности\s*";
}

public static class RegexHelpers
{
    public static bool HasAnyRegexSignature(string data, params string[] regexs)
    {
        foreach(var regex in regexs)
            if(new Regex(regex, RegexOptions.IgnoreCase).Match(data).Success)
                    return true;
        return false;
    }
}

public static class EnumHelpers
{
    public static string GetValueForTreatyFromDescription<T>(this T enumerationValue)
    where T : struct
    {
        Type type = enumerationValue.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
        }

        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[]? memberInfo = type.GetMember(enumerationValue.ToString() ?? throw new NullReferenceException($"{enumerationValue} is null"));
        if (memberInfo != null && memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs != null && attrs.Length > 0)
            {
                //Pull out the description value
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        //If we have no description attribute, just return the ToString of the enum
        throw new InvalidOperationException($"{enumerationValue} не содержит для {nameof(DescriptionAttribute)}атрибут для какого(их)-то значения(ий)");
    }

}

public static class DateTimeHelpers
{
    public static DateTime ParseFromOADateOrString(this string date)
    {
        double dDateParsed;

        double.TryParse(date, out dDateParsed);

        if(double.TryParse(date, out dDateParsed))
            return FromOADateWihAppException(dDateParsed);
        
        DateTime res;

        if(!DateTime.TryParse(date, out res))
            throw new AppExceptionBase($"Неправильный формат даты {date}");
        return res;

    }
    static DateTime FromOADateWihAppException(double dDate)
    {
        try
        {
            return DateTime.FromOADate(dDate);
        }
        catch(ArgumentException)
        {
            throw new AppExceptionBase($"Неправильный формат даты {dDate}");
        }
    }

    
}