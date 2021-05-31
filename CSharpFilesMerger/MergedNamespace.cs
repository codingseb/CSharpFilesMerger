using System.Collections.Generic;

namespace CSharpFilesMerger
{
    public class MergedNamespace
    {
        public string Name { get; set; }

        public string Comment { get; set; } = string.Empty;

        public List<string> Usings { get; set; } = new List<string>();

        public Dictionary<string, MergedTypeElement> Elements { get; set; } = new Dictionary<string, MergedTypeElement>();
    }
}
