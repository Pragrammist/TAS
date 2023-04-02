using System.Text.RegularExpressions;
using static TreatyAutomateSystem.Helpers.TreateConst;

namespace TreatyAutomateSystem.Helpers;

public static class TreateHelpersMethod
    {
        public static Match MatchedForCompanyName(this string whereInsert) => 
            new Regex(PARAGRAPH_PART_FOR_COMPANY_NAME_REGEX).Match(whereInsert);
    
        public static bool IsMatchedForCompanyName(this string whereInsert) => 
            Regex.IsMatch(whereInsert, PARAGRAPH_PART_FOR_COMPANY_NAME_REGEX);


        public static Match MatchedForPracticeDirector(this string whereInsert) => 
            new Regex(PARAGRAPH_PART_FOR_PRACTICE_DIRECTOR_NAME).Match(whereInsert);
    
        public static bool IsMatchedForPracticeDirector(this string whereInsert) => 
            Regex.IsMatch(whereInsert, PARAGRAPH_PART_FOR_PRACTICE_DIRECTOR_NAME);


        public static Match MatchedForNaOsnovanii(this string whereInsert) => 
            new Regex(PARAGRAPH_PART_FOR_NA_OSNOVANII).Match(whereInsert);

        public static bool IsMatchedForNaOsnovanii(this string whereInsert) => 
            Regex.IsMatch(whereInsert, PARAGRAPH_PART_FOR_NA_OSNOVANII);
    }
