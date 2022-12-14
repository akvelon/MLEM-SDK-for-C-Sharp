using System.Text.RegularExpressions;

namespace MlemApi.Utils
{
    /// <summary>
    /// Converts identifier to camel case
    /// </summary>
    internal class CamelCaseConverter
    {
        Regex delimitersRegex = new Regex(@"[\W_]");
        public string ConvertToCamelCase(string str)
        {
            return string.Concat(delimitersRegex.Split(str)
                .Select(s =>
                {
                    var tailingSubstring = s.Substring(1);
                    var camelCasedTailingSubstring = isCamelCase(str) ? tailingSubstring : tailingSubstring.ToLower();
                    return char.ToUpper(s[0]) + camelCasedTailingSubstring;
                })
            );
        }

        private bool isCamelCase(string value)
        {
            int upperCaseSubstrLength = 0;
            int maxUpperCaseSubstrLength = 0;
            int curIdx = 0;

            while (curIdx < value.Length)
            {
                if (Char.IsUpper(value[curIdx]))
                {
                    ++upperCaseSubstrLength;
                }

                if (!Char.IsUpper(value[curIdx]) || curIdx == value.Length - 1)
                {
                    if(upperCaseSubstrLength > maxUpperCaseSubstrLength)
                    {
                        maxUpperCaseSubstrLength = upperCaseSubstrLength;
                    }

                    upperCaseSubstrLength = 0;
                }

                ++curIdx;
            }

            return maxUpperCaseSubstrLength <= 2;
        }
    }
}
