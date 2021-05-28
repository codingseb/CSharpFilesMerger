using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharpFilesMerger
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            List<string> ignoreFile = new List<string>()
            {
                @"\\Properties\\",
                @"\\obj\\"
            };

            string directory = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

            List<CSharpFile> cSharpFiles = Directory
                .GetFiles(directory, "*.cs", SearchOption.AllDirectories)
                .Where(fileName => !ignoreFile.Any(pattern => Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase)))
                .Select(fileName => new CSharpFile(fileName))
                .ToList();

            List<string> usings = new List<string>();

            string mergedFile = string.Empty;

            cSharpFiles.ForEach(file =>
            {
                usings = usings.Union(file.Usings).ToList();
            });

            mergedFile = string.Join("\r\n", usings.OrderBy(u => u).Select(u => $"using {u};")) + "\r\n\r\n" + mergedFile;
        }
    }
}
