using System;
using Lephone.Data;
using Lephone.Data.Definition;

namespace Debug
{
    public abstract class FullType : DbObjectModel<FullType>
    {
        public abstract string c1 { get; set; }
        public abstract int c2 { get; set; }
        public abstract short c3 { get; set; }
        public abstract byte c4 { get; set; }
        public abstract bool c5 { get; set; }
        public abstract DateTime c6 { get; set; }
        public abstract decimal c7 { get; set; }
        public abstract float c8 { get; set; }
        public abstract double c9 { get; set; }
        public abstract Guid c10 { get; set; }
        //public abstract sbyte c11 { get; set; }
        //public abstract byte[] c15 { get; set; }
        public abstract Date c16 { get; set; }
        public abstract Time c17 { get; set; }
    }


    class Program
    {
        static void Main()
        {
            var ft = FullType.New;
            ft.c1 = "tom";
            ft.c2 = 2;
            ft.c3 = 3;
            ft.c4 = 4;
            ft.c5 = true;
            ft.c6 = new DateTime(2000, 1, 1);
            ft.c7 = 7;
            ft.c8 = (float)8.1;
            ft.c9 = 9.1;
            ft.c10 = Guid.NewGuid();
            //ft.c11 = 11;
            //ft.c15 = new byte[] { 1, 2, 3, 4, 5 };
            ft.c16 = Date.Now;
            ft.c17 = Time.Now;
            ft.Save();
            Console.WriteLine("Save Done!!!");
            Console.WriteLine();

            var list = DbEntry.Context.ExecuteList<FullType>("Select * from full_types");
            foreach (var fullType in list)
            {
                Console.WriteLine(fullType);
            }

            Console.ReadLine();
        }
    }
}