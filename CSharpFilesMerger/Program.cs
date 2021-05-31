using System;
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
            #region Variables declaration and default values

            List<string> ignoreFile = new List<string>()
            {
                @"\\Properties\\",
                @"\\obj\\"
            };

            List<string> usings = new List<string>();
            IEnumerable<string> cSharpFileNames;
            List<CSharpFile> cSharpFiles;
            string mergedFileContent = string.Empty;

            string directory = Directory.GetCurrentDirectory();
            bool recurcive = false;
            string outputFileName = "Merged.cs";

            #endregion

            #region Decode args

            #region Working directory

            if(args.Length > 0 && !args[0].StartsWith("-"))
            {
                directory = Path.GetFullPath(Path.IsPathRooted(args[0]) ? args[0] : Path.Combine(directory, args[0]));
            }

            #endregion

            #region Search files recursively ?

            recurcive = args.Contains("-r");

            #endregion

            #region listOfFiles

            int listIndex = args.ToList().IndexOf("-l");

            if (listIndex > -1 && listIndex < args.Length)
            {
                cSharpFileNames = args[listIndex + 1].Split(';')
                    .Select(fileName => Path.GetFullPath(Path.IsPathRooted(fileName) ? fileName : Path.Combine(directory, fileName)));
            }
            else
            {
                cSharpFileNames = Directory
                    .GetFiles(directory, "*.cs", recurcive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Where(fileName => !ignoreFile.Any(pattern => Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase)));
            }

            #endregion

            #region Output fileName

            int outputIndex = args.ToList().IndexOf("-o");

            if (outputIndex > -1 && outputIndex < args.Length)
            {
                outputFileName = args[outputIndex + 1];
            }

            if (!Path.IsPathRooted(outputFileName))
            {
                outputFileName = Path.GetFullPath(Path.Combine(directory, outputFileName));
            }

            #endregion

            #endregion
            
            #region Get Files to Merge

            Console.WriteLine($"Search files in \"{directory}\"{(recurcive ? " recursively" : string.Empty)}");

            cSharpFiles = cSharpFileNames
                .Select(fileName => new CSharpFile(fileName))
                .ToList();

            #endregion

            #region Parse Files to Merge

/*            cSharpFiles.ForEach(file =>
            {
                Console.WriteLine($"Parse file : \"{file.FileName}\"");
                usings = usings.Union(file.Usings).ToList();
            });*/
            
            cSharpFiles.ForEach(file =>
            {
                file.Namespaces.ForEach(ns =>
                {
                    MergedNamespace mergedNamespace = Namespaces.ContainsKey(ns.Name) ? Namespaces[ns.Name] : new MergedNamespace();


                });
            });



            #endregion

            #region Merge

            mergedFileContent = string.Join("\r\n", usings.OrderBy(u => u).Select(u => $"using {u};")) + "\r\n\r\n" + mergedFileContent;

            #endregion

            #region Write Merged File

            File.WriteAllText(outputFileName, mergedFileContent);

            #endregion

            Console.ReadLine();
        }
    }
}
