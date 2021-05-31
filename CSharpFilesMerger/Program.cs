﻿using System;
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
            Dictionary<string, MergedNamespace> Namespaces = new Dictionary<string, MergedNamespace>();
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

            Console.WriteLine(string.Empty);

            cSharpFiles = cSharpFileNames
                .Select(fileName =>
                {
                    Console.WriteLine($"Parse file : \"{fileName}\"");
                    return new CSharpFile(fileName); })
                .ToList();

            #endregion

            #region Merge Files to Merged Structure

            cSharpFiles.ForEach(file =>
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine($"Merge file : \"{file.FileName}\"");

                file.Namespaces.ForEach(ns =>
                {
                    Console.WriteLine($"Merge namespace : \"{ns.Name}\"");
                    MergedNamespace mergedNamespace = Namespaces.ContainsKey(ns.Name) ? Namespaces[ns.Name] : new MergedNamespace(){ Name = ns.Name };

                    if(ns.Comment.Length > mergedNamespace.Comment.Length)
                        mergedNamespace.Comment = ns.Comment;

                    ns.Elements.ForEach(typeElement =>
                    {
                        Console.WriteLine($"Merge type : \"{typeElement.Name}\"");
                        MergedTypeElement mergedTypeElement = mergedNamespace.Elements.ContainsKey(typeElement.Name) ? mergedNamespace.Elements[typeElement.Name] : new MergedTypeElement();

                        if (typeElement.Comment.Length > mergedTypeElement.Comment.Length)
                            mergedTypeElement.Comment = typeElement.Comment;

                        if (typeElement.Declaration.Length > mergedTypeElement.Declaration.Length)
                            mergedTypeElement.Declaration = typeElement.Declaration;

                        mergedTypeElement.Content += typeElement.Content;

                        mergedNamespace.Elements[typeElement.Name] = mergedTypeElement;
                    });

                    Namespaces[ns.Name] = mergedNamespace;
                });
            });

            #endregion

            #region Concatenate MergedStructure

            Console.WriteLine(string.Empty);
            Console.WriteLine("Concatenate");

            Namespaces.Values.ToList().ForEach(mergedNamespace =>
            {
                string content = string.Join("\r\n",
                    mergedNamespace.Elements.Values.ToList()
                        .Select(mergedTypeElement => $"{mergedTypeElement.Comment}\r\n{mergedTypeElement.Declaration}{mergedTypeElement.Content}}}"));

                mergedFileContent += $"{mergedNamespace.Comment}\r\nnamespace {mergedNamespace.Name}\r\n{{{content}\r\n}}\r\n";
            });

            //mergedFileContent = string.Join("\r\n", usings.OrderBy(u => u).Select(u => $"using {u};")) + "\r\n\r\n" + mergedFileContent;

            #endregion

            #region Write Merged File

            Console.WriteLine(string.Empty);
            Console.WriteLine($"Write result in \"{outputFileName}\"");
            File.WriteAllText(outputFileName, mergedFileContent);

            #endregion

            Console.ReadLine();
        }
    }
}
