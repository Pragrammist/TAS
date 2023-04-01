using System.Text.RegularExpressions;
using static TreatyAutomateSystem.Helpers.RegexConsts;
namespace TreatyAutomateSystem.Helpers;

public static class EnumerableHelpers
{
    public static IEnumerable<string> ToLower(this IEnumerable<string> en) => en.Select(s => s.ToLower());

    public static IEnumerable<string> TrimEndings(this IEnumerable<string> en) => en.Select(s => s.TrimEndings());

    public static IEnumerable<string> RemoveAllSpaces(this IEnumerable<string> en) => en.Select(s => s.Replace(" ", string.Empty));

    public static IEnumerable<string> Replace(this IEnumerable<string> en, string oldValue, string newValue) => en.Select(s => s.Replace(oldValue, newValue));

    public static IEnumerable<string> NormolizeEmpties(this IEnumerable<string> en) => en.Select(s => s.NormolizeEmpties());

    public static IEnumerable<string> GiveSpecialityCode(this IEnumerable<string> en) => en.Select(s => s.ParseSpecialityCode());

    public static IEnumerable<string> AnyIsNotMatchedForRegexes(
        this IEnumerable<string> en, 
        string[] regexes, 
        string regexName,
        RegexOptions regOpt = RegexOptions.IgnoreCase
    ) => 
        en.Select(s => s.HasAnyRegexSignature(regexes) 
        ? s 
        : throw new AppExceptionBase($"{s} не является {regexName}"));

    public static IEnumerable<string> AnyIsNotMatchedForRegex(
        this IEnumerable<string> en, 
        string regex, 
        string regexName,
        RegexOptions regOpt = RegexOptions.IgnoreCase
    ) => 
        en.Select(s => Regex.IsMatch(s, regex, regOpt) 
        ? s 
        : throw new AppExceptionBase($"{s} не является {regexName}"));

    public static IEnumerable<string> AnyIsNotMatchedForGroup(
        this IEnumerable<string> en) => en.Select(s => 
        {
            if(!Regex.IsMatch(s, GROUP_PATERN_HITH_REGEX) && !Regex.IsMatch(s, GROUP_PATERN_SECONDARY_REGEX))
                throw new AppExceptionBase("Группа не соответствует патерну вышки или спо");
            return s;   
        });
    
}

public class AppExceptionBase : Exception
{
    public AppExceptionBase(string m) : base(m){}
    public AppExceptionBase(){}
}
