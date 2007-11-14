
using System;
using System.IO;
using Lephone.Data;
using Lephone.Util;
using DebugLib.Models;

public static class DataInit
{
    private static readonly string dbpath = SystemHelper.BaseDirectory + @"App_Data\test.db";

    public static void Init()
    {
        if (File.Exists(dbpath)) { return; }

        DbEntry.UsingTransaction(delegate()
        {
            SysUser.New().Init("tom", 19, DateTime.Now, true).Save();
            SysUser.New().Init("jerry", 26, DateTime.Now, true).Save();
            SysUser.New().Init("mike", 21, DateTime.Now, true).Save();
            SysUser.New().Init("rose", 17, DateTime.Now, false).Save();
            SysUser.New().Init("alice", 16, DateTime.Now, false).Save();

            SysUser.New().Init("peter", 41, DateTime.Now, true).Save();
            SysUser.New().Init("vito", 28, DateTime.Now, true).Save();
            SysUser.New().Init("jeff", 23, DateTime.Now, true).Save();
            SysUser.New().Init("kate", 22, DateTime.Now, false).Save();
            SysUser.New().Init("july", 25, DateTime.Now, false).Save();

            SysUser.New().Init("lephone", 31, DateTime.Now, true).Save();
            SysUser.New().Init("juan", 25, DateTime.Now, false).Save();
        });
    }
}
