using System;
using DebugLib;

namespace Debug
{
    class Program
    {
        static void Main()
        {
            //Books.FindAll();
            var list = User.FindAll();

            Console.WriteLine(list.Count);

            Console.ReadLine();
        }
    }
}