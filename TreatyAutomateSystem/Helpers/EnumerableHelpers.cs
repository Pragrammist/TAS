using System.Text.RegularExpressions;

namespace TreatyAutomateSystem.Helpers;

public static class EnumerableHelpers
{
    public static IEnumerable<string> ToLower(this IEnumerable<string> en) => en.Select(s => s.ToLower());

    public static IEnumerable<string> TrimEndings(this IEnumerable<string> en) => en.Select(s => s.TrimStart().TrimEnd());

    public static IEnumerable<string> Trim(this IEnumerable<string> en) => en.Select(s => s.Trim());

    public static IEnumerable<string> NormolizeEmpties(this IEnumerable<string> en) => en.Select(s => s.NormolizeEmpties());
}

public class AppExceptionBase : Exception
{
    public AppExceptionBase(string m) : base(m){}
    public AppExceptionBase(){}
}

public static class StringRegexHelpers
{
    public static string GiveSpecialityCode(this string speciality)
    {
        var match = Regex.Match(speciality, @"\d{2}.\d{2}.\d{2}");
        
        if(!match.Success)
            throw new AppExceptionBase("Строка специальности не содержит код");
        return match.Value;
    }

    public static string NormolizeEmpties(this string str)
    {
        var words = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var sumWords = String.Concat(words);

        return sumWords;
    }
}