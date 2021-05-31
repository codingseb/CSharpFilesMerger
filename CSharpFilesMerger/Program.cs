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

            string directory = Directory.GetCurrentDirectory();

            if(args.Length > 0)
            {
                if(args[0].StartsWith("."))
                {
                    directory = Path.GetFullPath(Path.Combine(directory, args[0]));
                }
                else
                {
                    directory = args[0];
                }
            }

            List<CSharpFile> cSharpFiles = Directory
                .GetFiles(directory, "*.cs", SearchOption.AllDirectories)
                .Where(fileName => !ignoreFile.Any(pattern => Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase)))
                .Select(fileName => new CSharpFile(fileName))
                .ToList();

            List<string> usings = new List<string>();
            Dictionary<string, MergedNamespace> Namespaces = new Dictionary<string, MergedNamespace>();

            cSharpFiles.ForEach(file =>
            {
                file.Namespaces.ForEach(ns =>
                {
                    MergedNamespace mergedNamespace = Namespaces.ContainsKey(ns.Name) ? Namespaces[ns.Name] : new MergedNamespace();


                });
            });
        }
    }
}
