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
        private static readonly Regex usingsRegex = new Regex(@"^(?>\s*)using(?>\s+)(?<namespace>[\p{L}_][\p{L}_0-9]*((?>\s*)\.(?>\s*)[\p{L}_][\p{L}_0-9]*)*)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex namespaceRegex = new Regex(@"(^|\r|\n)(?>\s*)namespace(?>\s+)(?<namespace>[\p{L}_][\p{L}_0-9]*((?>\s*)\.(?>\s*)[\p{L}_][\p{L}_0-9]*)*)(?>\s*)\{(?<content>(([^}])|([}](?!\s*($|namespace))))*)[}]", RegexOptions.Compiled);

        public string FileName { get; }
        public string Content { get; private set; }
        public List<string> Usings { get; } = new List<string>();
        public List<Namespace> Namespaces { get; } = new List<Namespace>();

        public CSharpFile(string fileName)
        {
            FileName = fileName;
            Load();
        }

        public void Load()
        {
            Content = File.ReadAllText(FileName);
            string tmpContent = usingsRegex.Replace(Content, match =>
            {
                Usings.Add(spacesRegex.Replace(match.Groups["namespace"].Value, string.Empty));
                return string.Empty;
            });

            tmpContent = namespaceRegex.Replace(tmpContent, match =>
            {
                string name = spacesRegex.Replace(match.Groups["namespace"].Value, string.Empty);
                string content = match.Groups["content"].Value.Trim();
                Namespaces.Add(new Namespace(name, content));
                return string.Empty;
            });
        }
    }
}
