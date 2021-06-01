using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static int Main(string[] args)
        {
            #region Variables declaration and default values

            List<string> ignoreFile = new List<string>()
            {
                @"\\Properties\\",
                @"\\obj\\",
                @"\\Merged.cs"
            };

            List<string> usings = new List<string>();
            IEnumerable<string> cSharpFileNames;
            List<CSharpFile> cSharpFiles;
            Dictionary<string, MergedNamespace> Namespaces = new Dictionary<string, MergedNamespace>();
            Dictionary<string, MergedTypeElement> typeElements = new Dictionary<string, MergedTypeElement>();
            string mergedFileContent = string.Empty;

            string directory = Directory.GetCurrentDirectory();
            bool recurcive = false;
            bool waitAtTheEnd = false;
            string outputFileName = "Merged.cs";
            UsingsLocation usingsLocation = UsingsLocation.DoNotMove;

            #endregion

            try
            {
                #region Decode others args

                #region Wait at the end

                waitAtTheEnd = args.Contains("-w", StringComparer.OrdinalIgnoreCase) || args.Contains("--wait", StringComparer.OrdinalIgnoreCase);

                #endregion

                #region Help (Exclusive)

                if (args.Contains("-h", StringComparer.OrdinalIgnoreCase) || args.Contains("--help", StringComparer.OrdinalIgnoreCase))
                {
                    Help.PrintHelp();
                    if (waitAtTheEnd)
                        WaitAKey();
                    return 0;
                }

                #endregion

                #region Show version (Exclusive)

                if (args.Contains("-v", StringComparer.OrdinalIgnoreCase) || args.Contains("--version", StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    if (waitAtTheEnd)
                        WaitAKey();
                    return 0;
                }

                #endregion

                #region Working directory

                if (args.Length > 0 && !args[0].StartsWith("-"))
                {
                    directory = Path.GetFullPath(Path.IsPathRooted(args[0]) ? args[0] : Path.Combine(directory, args[0]));
                }

                #endregion

                #region Search files recursively ?

                recurcive = args.Contains("-r", StringComparer.OrdinalIgnoreCase) || args.Contains("--recursive", StringComparer.OrdinalIgnoreCase);

                #endregion

                #region listOfFiles

                int listIndex = args.ToList().FindIndex(a => a.Equals("-f", StringComparison.OrdinalIgnoreCase) || a.Equals("--files", StringComparison.OrdinalIgnoreCase));

                if (listIndex > -1 && listIndex < args.Length - 1)
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

                int outputIndex = args.ToList().FindIndex(a => a.Equals("-o", StringComparison.OrdinalIgnoreCase) || a.Equals("--out", StringComparison.OrdinalIgnoreCase));

                if (outputIndex > -1 && outputIndex < args.Length - 1)
                {
                    outputFileName = args[outputIndex + 1];
                }

                if (!Path.IsPathRooted(outputFileName))
                {
                    outputFileName = Path.GetFullPath(Path.Combine(directory, outputFileName));
                }

                #endregion

                #region Usings location

                int usingsIndex = args.ToList().FindIndex(a => a.Equals("-u", StringComparison.OrdinalIgnoreCase) || a.Equals("--usings", StringComparison.OrdinalIgnoreCase));

                if (usingsIndex > -1 && usingsIndex < args.Length - 1)
                {
                    usingsLocation = (UsingsLocation)Enum.Parse(typeof(UsingsLocation), args[usingsIndex + 1], true);
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
                        Stopwatch sw = Stopwatch.StartNew();
                        var file = new CSharpFile(fileName);
                        sw.Stop();
                        Console.WriteLine("File parsed in {0}", sw.Elapsed);
                        return file;
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

                    if (usingsLocation != UsingsLocation.Namespace)
                        usings = usings.Union(file.Usings).ToList();

                    file.Namespaces.ForEach(ns =>
                    {
                        Console.WriteLine($"Merge namespace : \"{ns.Name}\"");
                        MergedNamespace mergedNamespace = Namespaces.ContainsKey(ns.Name) ? Namespaces[ns.Name] : new MergedNamespace() { Name = ns.Name };

                        if (usingsLocation == UsingsLocation.Global)
                            usings = usings.Union(ns.Usings).ToList();
                        else
                            mergedNamespace.Usings = mergedNamespace.Usings.Union(ns.Usings).ToList();

                        if (usingsLocation == UsingsLocation.Namespace)
                            mergedNamespace.Usings = mergedNamespace.Usings.Union(file.Usings).ToList();

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
                    string content = mergedNamespace.Usings.Count > 0 ? string.Join("\r\n", mergedNamespace.Usings.OrderBy(u => u).Select(u => $"    using {u};")) + "\r\n\r\n" : string.Empty;

                    content += string.Join("\r\n\r\n",
                        mergedNamespace.Elements.Values.ToList()
                            .Select(mergedTypeElement => $"{mergedTypeElement.Comment}{mergedTypeElement.Declaration}{mergedTypeElement.Content}}}"));

                    mergedFileContent += $"{mergedNamespace.Comment}\r\nnamespace {mergedNamespace.Name}\r\n{{\r\n{content}\r\n}}\r\n\r\n".TrimStart('\r', '\n');
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

                int startIndex = args.ToList().FindIndex(a => a.Equals("-s", StringComparison.OrdinalIgnoreCase) || a.Equals("--start", StringComparison.OrdinalIgnoreCase));

                if (startIndex > -1)
                {
                    if (startIndex < args.Length - 1 && !args[startIndex + 1].StartsWith("-"))
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
                }

                if (waitAtTheEnd)
                    WaitAKey();

                #endregion

                return 0;
            }
            catch(Exception exception)
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("!!! ERROR !!!");
                Console.WriteLine(exception.Message);
                Console.ForegroundColor = color;
                if (waitAtTheEnd)
                    WaitAKey();
                return 1;
            }
        }

        private static void WaitAKey()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("Press a key to exit...");
            Console.ReadLine();
        }
    }
}
