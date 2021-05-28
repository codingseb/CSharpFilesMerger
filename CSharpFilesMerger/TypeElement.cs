using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSharpFilesMerger
{
    public class TypeElement
    {
        private static readonly Regex elementsRegex = new Regex(@"(?<=^|\r|\n)(?<comment>(?m:(^(?>\s*)//.*$[\r\n])*))?(?<declaration>(?>\s*)((?<modifiers>private|protected|internal|public|new|abstract|sealed|static|partial)(?>\s+))*((?<elementtype>class|struct|enum|interface)(?>\s+))(?<name>[\p{L}_][\p{L}_0-9]*)(?>\s*)(?<isgeneric>[<](?>([\p{L}_](?>[\p{L}_0-9]*)|(?>\s+)|[,\.])+|(?<gentag>[<])|(?<-gentag>[>]))*(?(gentag)(?!))[>](?>\s*))?(?<inherit>:(?>\s*)[\p{L}_][\p{L}_0-9]*(?>\s*)(,(?>\s*)[\p{L}_][\p{L}_0-9]*(?>\s*))*)?\{)", RegexOptions.Compiled);
        private static readonly Regex partialRegex = new Regex(@"\spartial\s", RegexOptions.Compiled);

        public string Comment { get; private set; }

        public string Name { get; private set; }

        public string Declaration { get; private set; }

        public bool IsPartial => partialRegex.IsMatch(Declaration);

        public string Content { get; private set; }

        public static List<TypeElement> Parse(ref string code)
        {
            var result = new List<TypeElement>();
            Match match;

            while ((match = elementsRegex.Match(code)).Success)
            {
                int i = match.Index + match.Length;

                result.Add(new TypeElement
                {
                    Comment = match.Groups["comment"].Value.TrimStart('\r', '\n'),
                    Name = match.Groups["name"].Value,
                    Declaration = match.Groups["declaration"].Value.TrimStart('\r', '\n'),
                    Content = BlockParser.ParseBetweenImbricableBrackets(code, ref i)
                });

                code = code.Remove(match.Index, i - match.Index + 1);
            }

            return result;
        }
    }
}
