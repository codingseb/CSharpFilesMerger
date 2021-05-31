
namespace TryThings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using TryThings.Process;

    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        static void Main()
        {
            Console.WriteLine(new Processor().Add(1, 2));
        }
    }
}

namespace TryThings.Process
{
    using System.Collections.Generic;

    public delegate void Hello();

    public class Processor
    {
        public int Add(int a, int b)
        {
            new List<int>();
            return a + b;
        }
    }
}

public class Orphan
{
    public void MyMethod()
    {
        // Content
    }
}
