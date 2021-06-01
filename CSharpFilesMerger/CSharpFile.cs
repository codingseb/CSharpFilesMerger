using System;
using System.Collections.Generic;
using System.IO;

namespace CSharpFilesMerger
{
    /// <summary>
    /// Map the different part of a C# file
    /// </summary>
    public class CSharpFile
    {
        public string FileName { get; }
        public List<string> Usings { get; private set; }
        public List<Namespace> Namespaces { get; private set; }
        public List<TypeElement> Elements { get; private set; }

        public CSharpFile(string fileName)
        {
            FileName = fileName;
            Load();
        }

        private void Load()
        {
            string code = File.ReadAllText(FileName);

            Namespaces = Namespace.Parse(ref code);
            Elements = TypeElement.Parse(ref code);
            Usings = UsingsParser.Parse(ref code);

            if (!string.IsNullOrWhiteSpace(code))
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"!!! WARNING !!! there is always some not parsed code in file \"{FileName}\" :");
                Console.ForegroundColor = color;
                Console.WriteLine("---------------------------------------------------------------------------------------");
                Console.WriteLine(code.Trim());
                Console.WriteLine("---------------------------------------------------------------------------------------");
            }
        }
    }
}
