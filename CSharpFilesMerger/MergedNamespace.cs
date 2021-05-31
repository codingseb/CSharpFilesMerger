using System.Collections.Generic;

namespace CSharpFilesMerger
{
    public class MergedNamespace
    {
        public List<string> Usings { get; set; } = new List<string>();

        public Dictionary<string, MergedTypeElement> Elements { get; set; } = new Dictionary<string, MergedTypeElement>();
    }
}
