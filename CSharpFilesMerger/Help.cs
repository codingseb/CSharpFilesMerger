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
            Console.WriteLine("By default the application search for *.cs files in the working directory of the application.");
            Console.WriteLine("You can specify an other directory as first argument.");
            Console.WriteLine("It can be an absolute path or a relative path from working directory");
            Console.WriteLine("");
        }
    }
}
