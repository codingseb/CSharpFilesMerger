using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharpFilesMerger
{
    /// <summary>
    /// Entry class
    /// </summary>
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
            Dictionary<string, MergedNamespace> Namespaces = new Dictionary<string, MergedNamespace>();
            Dictionary<string, MergedTypeElement> typeElements = new Dictionary<string, MergedTypeElement>();
            string mergedFileContent = string.Empty;

            string directory = Directory.GetCurrentDirectory();
            bool recurcive = false;
            string outputFileName = "Merged.cs";

            #endregion

            #region Decode args

            #region Working directory

            if (args.Length > 0 && !args[0].StartsWith("-"))
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

            #region Parse files

            Console.WriteLine($"Search files in \"{directory}\"{(recurcive ? " recursively" : string.Empty)}");

            Console.WriteLine(string.Empty);

            cSharpFiles = cSharpFileNames
                .Select(fileName =>
                {
                    Console.WriteLine($"Parse file : \"{fileName}\"");
                    return new CSharpFile(fileName);
                })
                .ToList();

            #endregion

            #region Merge Files to Merged Structure

            void MergeTypeElement(TypeElement typeElement, Dictionary<string, MergedTypeElement> mergedTypeElements, string context)
            {
                Console.WriteLine($"Merge type : \"{typeElement.Name}\" {context}");
                MergedTypeElement mergedTypeElement = mergedTypeElements.ContainsKey(typeElement.Name) ? mergedTypeElements[typeElement.Name] : new MergedTypeElement();

                if (typeElement.Comment.Length > mergedTypeElement.Comment.Length)
                    mergedTypeElement.Comment = typeElement.Comment;

                if (typeElement.Declaration.Length > mergedTypeElement.Declaration.Length)
                    mergedTypeElement.Declaration = typeElement.Declaration;

                mergedTypeElement.Content += typeElement.Content;

                mergedTypeElements[typeElement.Name] = mergedTypeElement;
            }

            cSharpFiles.ForEach(file =>
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine($"Merge file : \"{file.FileName}\"");

                usings = usings.Union(file.Usings).ToList();

                file.Namespaces.ForEach(ns =>
                {
                    Console.WriteLine($"Merge namespace : \"{ns.Name}\"");
                    MergedNamespace mergedNamespace = Namespaces.ContainsKey(ns.Name) ? Namespaces[ns.Name] : new MergedNamespace() { Name = ns.Name };

                    if (ns.Comment.Length > mergedNamespace.Comment.Length)
                        mergedNamespace.Comment = ns.Comment;

                    ns.Elements.ForEach(typeElement => MergeTypeElement(typeElement, mergedNamespace.Elements, $"of namespace \"{ns.Name}\""));

                    Namespaces[ns.Name] = mergedNamespace;
                });

                file.Elements.ForEach(typeElement => MergeTypeElement(typeElement, typeElements, "orphan element"));
            });

            #endregion

            #region Concatenate MergedStructure

            Console.WriteLine(string.Empty);
            Console.WriteLine("Concatenate");

            mergedFileContent = string.Join("\r\n", usings.OrderBy(u => u).Select(u => $"using {u};")) + "\r\n\r\n";

            Namespaces.Values.ToList().ForEach(mergedNamespace =>
            {
                string content = string.Join("\r\n\r\n",
                    mergedNamespace.Elements.Values.ToList()
                        .Select(mergedTypeElement => $"{mergedTypeElement.Comment}{mergedTypeElement.Declaration}{mergedTypeElement.Content}}}"));

                mergedFileContent += $"{mergedNamespace.Comment}\r\nnamespace {mergedNamespace.Name}\r\n{{\r\n{content}\r\n}}\r\n\r\n".TrimStart('\r','\n');
            });

            mergedFileContent += string.Join("\r\n\r\n",
                    typeElements.Values.ToList()
                        .Select(mergedTypeElement => $"{mergedTypeElement.Comment}{mergedTypeElement.Declaration}{mergedTypeElement.Content}}}"));

            mergedFileContent = mergedFileContent.Trim('\r', '\n');

            #endregion

            #region Write Merged File

            Console.WriteLine(string.Empty);
            Console.WriteLine($"Write result in \"{outputFileName}\"");
            File.WriteAllText(outputFileName, mergedFileContent);

            #endregion

            #region What to do at the end

            int startIndex = args.ToList().IndexOf("-s");

            if (startIndex > -1 && startIndex < args.Length && !args[startIndex + 1].StartsWith("-"))
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine($"start {args[startIndex + 1]} \"{outputFileName}\"");
                Process.Start(args[startIndex + 1], $"\"{outputFileName}\"");
            }
            else
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine($"start \"{outputFileName}\"");
                Process.Start(outputFileName);
            }

            if (args.Contains("-w"))
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine("Press a key to exit...");
                Console.ReadLine();
            }

            #endregion
        }
    }
}

// End
