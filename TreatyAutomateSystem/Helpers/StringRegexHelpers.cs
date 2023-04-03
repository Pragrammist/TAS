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
        var match = Regex.Match(speciality, SPEC_CODE_IN_START_PATERN_REGEX);
        
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
        if(group.HasAnyRegexSignature(GROUP_PATERN_SECONDARY_1_REGEX, GROUP_PATERN_SECONDARY_2_REGEX))
            return FacultativeType.Sec;
        else if(group.HasAnyRegexSignature(GROUP_PATERN_HITH_REGEX))
            return FacultativeType.Hgh;
        
        throw new AppExceptionBase($"Группа не правильного формата {group}");
    }

    public static StudyConditionType ParseStudyConditionType(this string stdCond)
    {
        if(stdCond == "ко")
            return StudyConditionType.Pd;
        else if(stdCond == "б")
            return StudyConditionType.Ste;
        
        throw new AppExceptionBase($"Текст неправильного формата {stdCond}");
    }

    public static string ParseCourseFromGroup(this string str)
    {   
        var mCourse = Regex.Match(str, COURSE_IN_GROUP_PATERN_REGEX, RegexOptions.IgnoreCase);
        if(!mCourse.Success)
            throw new AppExceptionBase($"группа не содержит в конце курс {str}");
        return Regex.Match(mCourse.Value, NUMBER_IN_COURSE_PATEREN_REGEX).Value;
    }

    public static bool HasAnyRegexSignature(this string data, params string[] regexs)
    {
        foreach(var regex in regexs)
            if(Regex.IsMatch(data, regex))
                    return true;
        return false;
    }
    public static Match GiveFirstMatchedRegex(this string data, RegexOptions opt = RegexOptions.None, params string[] regexs)
    {
        foreach(var regex in regexs)
            if(Regex.IsMatch(data, regex))
                    return Regex.Match(data, regex);
        throw new AppExceptionBase($"в {data} не совпадает с regex: {String.Join(" ", regexs)}");
    }
    public static bool HasAllRegexSignature(this string data, params string[] regexs)
    {
        foreach(var regex in regexs)
            if(!Regex.IsMatch(data, regex))
                    return false;
        return true;
    }
    
}
