using System;
using Lephone.Data;
using Lephone.Data.Definition;

namespace Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = DbEntry.Context.GetDbColumnInfoList("people");

            Console.ReadLine();
        }
    }
}