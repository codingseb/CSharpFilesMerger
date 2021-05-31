using System;

namespace CSharpFilesMerger
{
    /// <summary>
    /// To print the documentation (-h or --help) of the application in the console
    /// </summary>
    public static class Help
    {
        /// <summary>
        /// Print the help
        /// </summary>
        public static void PrintHelp()
        {
            const int left = 4;
            const int right = 20;

            Console.WriteLine("By default the application search for *.cs files in the working directory of the application.");
            Console.WriteLine("You can specify an other directory as first argument.");
            Console.WriteLine("It can be an absolute path or a relative path from working directory");
            Console.WriteLine("");
            Console.WriteLine("Syntax:");
            Console.WriteLine("CSharpFilesMerger [workingDirectory] [OPTIONS]".PadLeft(left));
            Console.WriteLine("");
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("".PadRight(left) + "-h, --help".PadRight(right) + "Show this help text (is exclusive with others options except -w)");
            Console.WriteLine("");
            Console.WriteLine("".PadRight(left) + "-r, --recursive".PadRight(right) + "Search for *.cs file recursively from working directory or specified directory.");
            Console.WriteLine("".PadRight(left + right) + "Do not work with -f, --files");
            Console.WriteLine("");
            Console.WriteLine("".PadRight(left) + "-f, --files".PadLeft(left).PadRight(right) + "To specify the list of files to merge. Only these files will be merged.");
            Console.WriteLine("".PadRight(left + right) + "Each file must be separate by a semicolon \";\".");
            Console.WriteLine("".PadRight(left + right) + "Each file can be specify as absolute path or relative to working directory");
            Console.WriteLine("".PadRight(left + right) + "or specified directory.");
            Console.WriteLine("".PadRight(left + right) + "Example : -f \"a.cs;..\\b.cs;C:\\myDirectory\\c.cs\"");
        }
    }
}
    