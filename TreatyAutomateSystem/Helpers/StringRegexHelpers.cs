using static TreatyAutomateSystem.Helpers.RegexConsts;
using System.Text.RegularExpressions;
using TreatyAutomateSystem.Models;

namespace TreatyAutomateSystem.Helpers;

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
    
    public static FacultativeType ParseFacultativeType(this string group)
    {
        if(Regex.IsMatch(group, GROUP_PATERN_SECONDARY_REGEX))
            return FacultativeType.Sec;
        else if(Regex.IsMatch(group, GROUP_PATERN_HITH_REGEX))
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

    public static bool HasAnyRegexSignature(this string data, params string[] regexs)
    {
        foreach(var regex in regexs)
            if(new Regex(regex, RegexOptions.IgnoreCase).Match(data).Success)
                    return true;
        return false;
    }

    
}
