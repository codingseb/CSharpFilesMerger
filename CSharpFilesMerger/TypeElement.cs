using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSharpFilesMerger
{
    /// <summary>
    /// To parse type elements (class,enum, struct, interface) in one file
    /// </summary>
    public class TypeElement
    {
        private static readonly Regex elementsRegex = new Regex(@"(?<=^|\r|\n)(?<comment>(?m:(^(?>\s*)//.*$[\r\n])*))?(?<declaration>(?>\s*)((?<modifiers>private|protected|internal|public|new|abstract|sealed|static|partial)(?>\s+))*((?<elementtype>class|struct|enum|interface)(?>\s+))(?<name>[\p{L}_][\p{L}_0-9]*)(?>\s*)(?<isgeneric>[<](?>([\p{L}_](?>[\p{L}_0-9]*)|(?>\s+)|[,\.])+|(?<gentag>[<])|(?<-gentag>[>]))*(?(gentag)(?!))[>](?>\s*))?(?<inherit>:(?>\s*)[\p{L}_][\p{L}_0-9]*(?>\s*)(?<isgeneric>[<](?>([\p{L}_](?>[\p{L}_0-9]*)|(?>\s+)|[,\.])+|(?<gentag>[<])|(?<-gentag>[>]))*(?(gentag)(?!))[>](?>\s*))?(,(?>\s*)[\p{L}_][\p{L}_0-9]*(?>\s*)(?<isgeneric>[<](?>([\p{L}_](?>[\p{L}_0-9]*)|(?>\s+)|[,\.])+|(?<gentag>[<])|(?<-gentag>[>]))*(?(gentag)(?!))[>](?>\s*))?)*)?\{)", RegexOptions.Compiled);

        public string Comment { get; private set; }

        public string Name { get; private set; }

        public string Declaration { get; private set; }

        public string Content { get; private set; }

        /// <summary>
        /// Parse and return all typeElements (class, struct, enum, interface) at the root level of the given code
        /// </summary>
        /// <param name="code">The code to parse</param>
        /// <returns>The list of all found typeElements</returns>
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
