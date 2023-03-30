using System.Text.RegularExpressions;
namespace TreatyAutomateSystem.Helpers;

public static class EnumerableHelpers
{
    public static IEnumerable<string> ToLower(this IEnumerable<string> en) => en.Select(s => s.ToLower());

    public static IEnumerable<string> TrimEndings(this IEnumerable<string> en) => en.Select(s => s.TrimEndings());

    public static IEnumerable<string> RemoveAllSpaces(this IEnumerable<string> en) => en.Select(s => s.Replace(" ", string.Empty));

    public static IEnumerable<string> NormolizeEmpties(this IEnumerable<string> en) => en.Select(s => s.NormolizeEmpties());

    public static IEnumerable<string> GiveSpecialityCode(this IEnumerable<string> en) => en.Select(s => s.ParseSpecialityCode());

    public static IEnumerable<string> AnyIsNotMatchedForRegex(this IEnumerable<string> en, string regex, string regexName) => en.Select(s => Regex.IsMatch(s, regex, RegexOptions.IgnoreCase) ? s : throw new AppExceptionBase($"{s} не является {regexName}"));
}

public class AppExceptionBase : Exception
{
    public AppExceptionBase(string m) : base(m){}
    public AppExceptionBase(){}
}
