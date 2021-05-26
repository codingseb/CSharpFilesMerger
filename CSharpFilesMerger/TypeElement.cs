using System.Text.RegularExpressions;

namespace CSharpFilesMerger
{
    public class TypeElement
    {
        private static readonly Regex partialRegex = new Regex(@"\spartial\s", RegexOptions.Compiled);

        public string Name { get; set; }

        public string Declaration { get; set; }

        public bool IsPartial => partialRegex.IsMatch(Declaration);

        public string Content { get; set; }
    }
}
