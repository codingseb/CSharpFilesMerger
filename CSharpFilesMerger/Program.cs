using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpFilesMerger
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            List<string> ignoreFile = new List<string>()
            {
                @"\\Properties\\",
                @"\\obj\\"
            };

            List<CSharpFile> cSharpFiles = Directory
                .GetFiles(@"C:\Projets\CSharpFilesMerger\CSharpFilesMerger", "*.cs", SearchOption.AllDirectories)
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
