
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.UnitTest
{
    public static class InitHelper
    {
        // private static readonly string TestTableSql = ResourceHelper.ReadToEnd(typeof(InitHelper), "TestTable.sql");
        private const string FileName = "UnitTest.db";
        private const string TestFilePath = @"C:\" + FileName;
        private static readonly byte[] TestFileBuffer = ResourceHelper.ReadAll(typeof(InitHelper), FileName);

        public static void Init()
        {
            // org.hanzify.llf.Data.SqlEntry.DataBase.ExecuteNonQuery(TestTableSql);
            Clear();
            using (Stream s = new FileStream(TestFilePath, FileMode.Create))
            {
                s.Write(TestFileBuffer, 0, TestFileBuffer.Length);
            }
        }

        public static void Clear()
        {
            File.Delete(TestFilePath);
        }
    }
}
