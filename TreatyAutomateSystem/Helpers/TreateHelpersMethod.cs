using System.Text.RegularExpressions;
using static TreatyAutomateSystem.Helpers.TreateConst;

namespace TreatyAutomateSystem.Helpers;

public static class TreateHelpersMethod
    {
        public static Match MatchedForCompanyName(this string whereInsert) => 
            new Regex(PARAGRAPH_PART_FOR_COMPANY_NAME_REGEX).Match(whereInsert);
    

        public static Match MatchedForPracticeDirector(this string whereInsert) => 
            new Regex(PARAGRAPH_PART_FOR_PRACTICE_DIRECTOR_NAME).Match(whereInsert);
    

        public static Match MatchedForNaOsnovanii(this string whereInsert) => 
            new Regex(PARAGRAPH_PART_FOR_NA_OSNOVANII).Match(whereInsert);
    }
