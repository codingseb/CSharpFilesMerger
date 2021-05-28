using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSharpFilesMerger
{
    public sealed class Namespace
    {
        private static readonly Regex spacesRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex namespaceRegex = new Regex(@"(^|\r|\n)(?<comment>(?m:(^(?>\s*)//.*$[\r\n])*))?(?>\s*)namespace(?>\s+)(?<namespace>[\p{L}_][\p{L}_0-9]*((?>\s*)\.(?>\s*)[\p{L}_][\p{L}_0-9]*)*)(?>\s*)\{", RegexOptions.Compiled);

        public string Comment { get; private set; }
        public string Name { get; private set;  }

        public List<TypeElement> Elements { get; private set; }

        private Namespace()
        {}

        public static List<Namespace> Parse(ref string code)
        {
            var result = new List<Namespace>();
            Match match;

            while((match = namespaceRegex.Match(code)).Success)
            {
                int i = match.Index + match.Length;

                string content = BlockParser.ParseBetweenImbricableBrackets(code, ref i);

                result.Add(new Namespace
                {
                    Comment = match.Groups["comment"].Value.TrimStart('\r', '\n'),
                    Name = spacesRegex.Replace(match.Groups["namespace"].Value, string.Empty),
                    Elements = TypeElement.Parse(ref content)
                });

                code = code.Remove(match.Index, i - match.Index + 1);
            }

            return result;
        }
    }
}
