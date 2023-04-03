using System.Text.RegularExpressions;
using static TreatyAutomateSystem.Helpers.RegexConsts;

namespace TreatyAutomateSystem.Helpers;

public static class TreatyHelpersMethod
    {
        public static string ToUpperFirstLaterFirstWord(this string str)
        {
            var firstWord = str.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(w => Regex.IsMatch(w, WORD_REGEX));

            if(firstWord is null)
                return str;

            var matchedWordWithUpperFirstLater = firstWord.ToUpperFirstLatter();

            return str.Replace(firstWord, matchedWordWithUpperFirstLater);
        }

        public static string ToUpperFirstLater(this string str)
        {
            const char SEPARATOR = ' ';
            var words = str
                .Split(SEPARATOR)
                .Select(w => 
                    Regex.IsMatch(w, WORD_REGEX) 
                        ? w.ToUpperFirstLaterFirstWord() 
                        : w
                    );
            var newStr = String.Join(SEPARATOR, words);
            return newStr;
        }

        static string ToUpperFirstLatter(this string str)
        {
            if(str.Length == 0)
                return str;

            if(str.Length == 1)
                return str.ToUpper();

            return str.First().ToString().ToUpper() + str[1..];
        }
    }
