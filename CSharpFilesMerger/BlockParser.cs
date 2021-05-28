using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpFilesMerger
{
    public static class BlockParser
    {
        private static readonly Regex stringBeginningRegex = new Regex("^(?<interpolated>[$])?(?<escaped>[@])?[\"]", RegexOptions.Compiled);
        private static readonly Regex internalCharRegex = new Regex(@"^['](\\[\\'0abfnrtv]|[^'])[']", RegexOptions.Compiled);
        private static readonly Regex endOfStringWithDollar = new Regex("^([^\"{\\\\]|\\\\[\\\\\"0abfnrtv])*[\"{]", RegexOptions.Compiled);
        private static readonly Regex endOfStringWithoutDollar = new Regex("^([^\"\\\\]|\\\\[\\\\\"0abfnrtv])*[\"]", RegexOptions.Compiled);
        private static readonly Regex endOfStringWithDollarWithAt = new Regex("^[^\"{]*[\"{]", RegexOptions.Compiled);
        private static readonly Regex endOfStringWithoutDollarWithAt = new Regex("^[^\"]*[\"]", RegexOptions.Compiled);
        private static readonly Regex endOfStringInterpolationRegex = new Regex("^('\"'|[^}\"])*[}\"]", RegexOptions.Compiled);
        private static readonly Regex stringBeginningForEndBlockRegex = new Regex("[$]?[@]?[\"]$", RegexOptions.Compiled);

        private static readonly Regex lineCommentRegex = new Regex(@"^//[^\r\n]*", RegexOptions.Compiled);
        private static readonly Regex blockCommentRegex = new Regex(@"^/\*(.*?)\*/", RegexOptions.Compiled);

        private static IDictionary<string, string> ImbricableBracketsPairing { get; } = new Dictionary<string, string>()
        {
            { "(", ")" },
            { "{", "}" },
            { "[", "]" },
        };

        public static string ParseBetweenImbricableBrackets(string code, ref int i, string startToken = "{", string endToken = "}")
        {
            string contentCode = string.Empty;
            int bracketCount = 1;
            for (; i < code.Length; i++)
            {
                string subExpr = code.Substring(i);
                Match match;

                if ((match = stringBeginningRegex.Match(subExpr)).Success)
                {
                    string innerString = match.Value + GetCodeUntilEndOfString(code.Substring(i + match.Length), match);
                    contentCode += innerString;
                    i += innerString.Length - 1;
                }
                else if ((match = internalCharRegex.Match(subExpr)).Success
                    || (match = lineCommentRegex.Match(subExpr)).Success
                    || (match = blockCommentRegex.Match(subExpr)).Success)
                {
                    contentCode += match.Value;
                    i += match.Length - 1;
                }
                else
                {
                    if (subExpr.StartsWith(endToken))
                    {
                        bracketCount--;
                        i += endToken.Length - 1;
                        if (bracketCount == 0)
                        {
                            break;
                        }
                    }

                    if (startToken != null && subExpr.StartsWith(startToken))
                    {
                        bracketCount++;
                        i += startToken.Length - 1;
                        contentCode += startToken;
                        continue;
                    }

                    int index = i;
                    string openingBracket = ImbricableBracketsPairing.Keys.FirstOrDefault(key => code.Substring(index).StartsWith(key));

                    if (openingBracket != null)
                    {
                        i += openingBracket.Length;
                        string closingBrackets = ImbricableBracketsPairing[openingBracket];
                        contentCode += openingBracket + ParseBetweenImbricableBrackets(code, ref i, openingBracket, closingBrackets) + closingBrackets;
                        continue;
                    }

                    subExpr = code.Substring(i);

                    contentCode += code.Substring(i, 1);
                }
            }

            if (bracketCount > 0)
            {
                string beVerb = bracketCount == 1 ? "is" : "are";
                throw new Exception($"{bracketCount} '{endToken}' character {beVerb} missing in expression : [{code}]");
            }

            return contentCode;
        }

        private static string GetCodeUntilEndOfString(string subExpr, Match stringBeginningMatch)
        {
            StringBuilder stringBuilder = new StringBuilder();

            GetCodeUntilEndOfString(subExpr, stringBeginningMatch, ref stringBuilder);

            return stringBuilder.ToString();
        }

        private static void GetCodeUntilEndOfString(string subExpr, Match stringBeginningMatch, ref StringBuilder stringBuilder)
        {
            Match codeUntilEndOfStringMatch = stringBeginningMatch.Value.Contains("$") ?
                (stringBeginningMatch.Value.Contains("@") ? endOfStringWithDollarWithAt.Match(subExpr) : endOfStringWithDollar.Match(subExpr)) :
                (stringBeginningMatch.Value.Contains("@") ? endOfStringWithoutDollarWithAt.Match(subExpr) : endOfStringWithoutDollar.Match(subExpr));

            if (codeUntilEndOfStringMatch.Success)
            {
                if (codeUntilEndOfStringMatch.Value.EndsWith("\""))
                {
                    stringBuilder.Append(codeUntilEndOfStringMatch.Value);
                }
                else if (codeUntilEndOfStringMatch.Value.EndsWith("{") && codeUntilEndOfStringMatch.Length < subExpr.Length)
                {
                    if (subExpr[codeUntilEndOfStringMatch.Length] == '{')
                    {
                        stringBuilder.Append(codeUntilEndOfStringMatch.Value);
                        stringBuilder.Append("{");
                        GetCodeUntilEndOfString(subExpr.Substring(codeUntilEndOfStringMatch.Length + 1), stringBeginningMatch, ref stringBuilder);
                    }
                    else
                    {
                        string interpolation = GetCodeUntilEndOfStringInterpolation(subExpr.Substring(codeUntilEndOfStringMatch.Length));
                        stringBuilder.Append(codeUntilEndOfStringMatch.Value);
                        stringBuilder.Append(interpolation);
                        GetCodeUntilEndOfString(subExpr.Substring(codeUntilEndOfStringMatch.Length + interpolation.Length), stringBeginningMatch, ref stringBuilder);
                    }
                }
                else
                {
                    stringBuilder.Append(subExpr);
                }
            }
            else
            {
                stringBuilder.Append(subExpr);
            }
        }

        private static string GetCodeUntilEndOfStringInterpolation(string subExpr)
        {
            Match endOfStringInterpolationMatch = endOfStringInterpolationRegex.Match(subExpr);
            string result = subExpr;

            if (endOfStringInterpolationMatch.Success)
            {
                if (endOfStringInterpolationMatch.Value.EndsWith("}"))
                {
                    result = endOfStringInterpolationMatch.Value;
                }
                else
                {
                    Match stringBeginningForEndBlockMatch = stringBeginningForEndBlockRegex.Match(endOfStringInterpolationMatch.Value);

                    string subString = GetCodeUntilEndOfString(subExpr.Substring(endOfStringInterpolationMatch.Length), stringBeginningForEndBlockMatch);

                    result = endOfStringInterpolationMatch.Value + subString
                        + GetCodeUntilEndOfStringInterpolation(subExpr.Substring(endOfStringInterpolationMatch.Length + subString.Length));
                }
            }

            return result;
        }
    }
}
