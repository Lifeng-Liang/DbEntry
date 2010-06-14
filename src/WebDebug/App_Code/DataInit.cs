
using System;
using System.IO;
using Lephone.Data;
using Lephone.Core;
using DebugLib.Models;

public static class DataInit
{
    private static readonly string dbpath = SystemHelper.BaseDirectory + @"App_Data\test.db";

    public static void Init()
    {
        if (File.Exists(dbpath)) { return; }

        DbEntry.NewTransaction(delegate
        {
            new SysUser { Name = "tom", Age = 19, Birthday = GetDate(), IsMale = true }.Save();
            new SysUser { Name = "jerry", Age = 26, Birthday = GetDate(), IsMale = true }.Save();
            new SysUser { Name = "mike", Age = 21, Birthday = GetDate(), IsMale = true }.Save();
            new SysUser { Name = "rose", Age = 17, Birthday = GetDate(), IsMale = false }.Save();
            new SysUser { Name = "alice", Age = 16, Birthday = GetDate(), IsMale = false }.Save();

            new SysUser { Name = "peter", Age = 41, Birthday = GetDate(), IsMale = true }.Save();
            new SysUser { Name = "vito", Age = 28, Birthday = GetDate(), IsMale = true }.Save();
            new SysUser { Name = "jeff", Age = 23, Birthday = GetDate(), IsMale = true }.Save();
            new SysUser { Name = "kate", Age = 22, Birthday = GetDate(), IsMale = false }.Save();
            new SysUser { Name = "july", Age = 25, Birthday = GetDate(), IsMale = false }.Save();

            new SysUser { Name = "lephone", Age = 31, Birthday = GetDate(), IsMale = true }.Save();
            new SysUser { Name = "juan", Age = 25, Birthday = GetDate(), IsMale = false }.Save();
        });
    }

    private static Date GetDate()
    {
        return new Date(DateTime.Now);
    }
}
