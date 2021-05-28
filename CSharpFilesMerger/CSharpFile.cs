using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CSharpFilesMerger
{
    /// <summary>
    /// Map the different part of a C# file
    /// </summary>
    public class CSharpFile
    {
        private static readonly Regex spacesRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex usingsRegex = new Regex(@"^(?>\s*)using(?>\s+)(?<namespace>[\p{L}_][\p{L}_0-9]*((?>\s*)\.(?>\s*)[\p{L}_][\p{L}_0-9]*)*)(?>\s*);", RegexOptions.Compiled | RegexOptions.Multiline);

        public string FileName { get; }
        public string Content { get; private set; }
        public List<string> Usings { get; } = new List<string>();
        public List<Namespace> Namespaces { get; private set; }
        public List<TypeElement> Elements { get; private set; }

        public CSharpFile(string fileName)
        {
            FileName = fileName;
            Load();
        }

        public void Load()
        {
            Content = File.ReadAllText(FileName);
            string code = usingsRegex.Replace(Content, match =>
            {
                Usings.Add(spacesRegex.Replace(match.Groups["namespace"].Value, string.Empty));
                return string.Empty;
            });

            Namespaces = Namespace.Parse(ref code);
            Elements = TypeElement.Parse(ref code);
        }
    }
}
