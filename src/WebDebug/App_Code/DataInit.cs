
using System;
using System.IO;
using org.hanzify.llf.Data;
using org.hanzify.llf.util;
using test;

public static class DataInit
{
    private static readonly string dbpath = SystemHelper.BaseDirectory + @"App_Data\test.db";

    public static void Init()
    {
        if (File.Exists(dbpath)) { return; }

        DbEntry.UsingTransaction(delegate()
        {
            User.New("tom", 19, DateTime.Now, true).Save();
            User.New("jerry", 26, DateTime.Now, true).Save();
            User.New("mike", 21, DateTime.Now, true).Save();
            User.New("rose", 17, DateTime.Now, false).Save();
            User.New("alice", 16, DateTime.Now, false).Save();

            User.New("peter", 41, DateTime.Now, true).Save();
            User.New("vito", 28, DateTime.Now, true).Save();
            User.New("jeff", 23, DateTime.Now, true).Save();
            User.New("kate", 22, DateTime.Now, false).Save();
            User.New("july", 25, DateTime.Now, false).Save();

            User.New("lephone", 31, DateTime.Now, true).Save();
            User.New("juan", 25, DateTime.Now, false).Save();
        });
    }
}
