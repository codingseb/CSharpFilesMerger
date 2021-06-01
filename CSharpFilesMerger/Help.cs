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
            const int left = 2;
            const int right = 18;

            Console.WriteLine("By default the application search for *.cs files in the working directory of the application.");
            Console.WriteLine("You can specify an other directory as first argument.");
            Console.WriteLine("It can be an absolute path or a relative path from working directory.");
            Console.WriteLine("");
            Console.WriteLine("Syntax:");
            Console.WriteLine("CSharpFilesMerger [workingDirectory] [OPTIONS]".PadLeft(left));
            Console.WriteLine("");
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("".PadRight(left) + "-h, --help".PadRight(right) + "Show this help text (is exclusive with others options except -w)");
            Console.WriteLine("");
            Console.WriteLine("".PadRight(left) + "-v, --version".PadRight(right) + "Show the verison of the application (is exclusive with others options except -w)");
            Console.WriteLine("");
            Console.WriteLine("".PadRight(left) + "-r, --recursive".PadRight(right) + "Search for *.cs file recursively from working directory or specified directory.");
            Console.WriteLine("".PadRight(left + right) + "Do not work with -f, --files");
            Console.WriteLine("");
            Console.WriteLine("".PadRight(left) + "-f, --files".PadLeft(left).PadRight(right) + "To specify the list of files to merge. Only these files will be merged.");
            Console.WriteLine("".PadRight(left + right) + "Each file must be separate by a semicolon \";\".");
            Console.WriteLine("".PadRight(left + right) + "Each file can be specify as absolute path or relative to working directory");
            Console.WriteLine("".PadRight(left + right) + "or specified directory.");
            Console.WriteLine("".PadRight(left + right) + "Example : -f \"a.cs;..\\b.cs;C:\\myDirectory\\c.cs\"");
            Console.WriteLine("".PadRight(left + right) + "Remark : When this option is active files are merged in the order of the list.");
            Console.WriteLine("");
            Console.WriteLine("".PadRight(left) + "-o, --out".PadLeft(left).PadRight(right) + "To specify the fileName of the output file where *.cs files are merged.");
            Console.WriteLine("".PadRight(left + right) + "Can be specify as absolute path or relative to working directory or specified directory.");
            Console.WriteLine("".PadRight(left + right) + "Example : -o \"MyOutputFile.cs\"");
            Console.WriteLine("".PadRight(left + right) + "Example : -o \"C:\\myDirectory\\MyOutputFile.cs\"");
            Console.WriteLine("".PadRight(left + right) + "Default value is equivalent to : -o \"Merged.cs\"");
            Console.WriteLine("");
            Console.WriteLine("".PadRight(left) + "-u, --usings".PadLeft(left).PadRight(right) + "To specify where to put usings (globally vs in namespaces).");
            Console.WriteLine("".PadRight(left + right) + "Must be followed by one of these 3 options :");
            Console.WriteLine("".PadRight(left + right) + "- DoNotMove : keep the found \"using\" globally if it is defined outside of any namespace");
            Console.WriteLine("".PadRight(left + right) + "              or in the specific namespace if found in a namespace.");
            Console.WriteLine("".PadRight(left + right) + "- Global    : Force all \"using\" to be defined globally in the merged result.");
            Console.WriteLine("".PadRight(left + right) + "- Namespace : Force all \"using\" to be defined in namespaces in the merged result.");
            Console.WriteLine("".PadRight(left + right) + "Default value is equivalent to : -u DoNotMove");
            Console.WriteLine("".PadRight(left + right) + "Remark : All usings are ordered and print only one time by usings location.");
            Console.WriteLine("");
            Console.WriteLine("".PadRight(left) + "-s, --start".PadLeft(left).PadRight(right) + "To open the result file at the end.");
            Console.WriteLine("".PadRight(left + right) + "By default try to open the file with the default program for *.cs files.");
            Console.WriteLine("".PadRight(left + right) + "An other program can be optionally specify with the path of the program");
            Console.WriteLine("".PadRight(left + right) + "Example : -s \"C:\\Program Files\\MyEditor\\MyEditor.exe\"");
            Console.WriteLine("");
            Console.WriteLine("".PadRight(left) + "-w, --wait".PadLeft(left).PadRight(right) + "Ask the user to press a key to exit at the end.");
        }
    }
}
    