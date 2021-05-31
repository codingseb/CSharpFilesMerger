using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSharpFilesMerger
{
    /// <summary>
    /// To Parse all usings at the current level of code
    /// </summary>
    public static class UsingsParser
    {
        private static readonly Regex spacesRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex usingsRegex = new Regex(@"^(?>\s*)using(?>\s+)(?<namespace>[\p{L}_][\p{L}_0-9]*((?>\s*)\.(?>\s*)[\p{L}_][\p{L}_0-9]*)*)(?>\s*);", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Parse and return all usings at the root level of the given code
        /// </summary>
        /// <param name="code">The code to parse</param>
        /// <returns>The list of all found usings</returns>
        public static List<string> Parse(ref string code)
        {
            var result = new List<string>();

            code = usingsRegex.Replace(code, match =>
            {
                result.Add(spacesRegex.Replace(match.Groups["namespace"].Value, string.Empty));
                return string.Empty;
            });

            return result;
        }
    }
}
