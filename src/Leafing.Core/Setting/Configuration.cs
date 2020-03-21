using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Leafing.Core.Setting {
    [DataContract]
    public class Configuration {
        [DataMember]
        public Dictionary<string, string> AppSettings = new Dictionary<string, string>();

        [DataMember]
        public ServiceSetting Service = new ServiceSetting();

        [DataMember]
        public LogSetting Log = new LogSetting();

        [DataMember]
        public DatabaseSetting Database = new DatabaseSetting();

        public void Merge(Configuration setting) {
            if(setting.AppSettings != null && setting.AppSettings.Count > 0) {
                AppSettings = setting.AppSettings;
            }
            Service.Merge(setting.Service);
            Log.Merge(setting.Log);
            Database.Merge(setting.Database);
        }
    }

    [DataContract]
    public class ServiceSetting {
        [DataMember]
        public int DelayToStart = 30000;

        [DataMember]
        public int MinStartTicks = 180000;

        public void Merge(ServiceSetting setting) {
            if(setting.DelayToStart != DelayToStart) { DelayToStart = setting.DelayToStart; }
            if(setting.MinStartTicks != MinStartTicks) { MinStartTicks = setting.MinStartTicks; }
        }
    }

    [DataContract]
    public class LogSetting {
        [DataMember]
        public string Level = "All";

        [DataMember]
        public string FileName = "{0}{1}.{2}.log";

        [DataMember]
        public Dictionary<string, List<string>> Recorders = new Dictionary<string, List<string>>();

        public void Merge(LogSetting setting) {
            if(setting.Level != Level) { Level = setting.Level; }
            if(setting.FileName != FileName) { FileName = setting.FileName; }
            if(setting.Recorders != null && setting.Recorders.Count > 0) {
                Recorders = setting.Recorders;
            }
        }
    }

    [DataContract]
    public class DatabaseSetting {
        [DataMember]
        public string Default;

        [DataMember]
        public string NameMapper = "@Default";

        [DataMember]
        public int SqlTimeOut = 30;

        [DataMember]
        public int TimeConsumingSqlTimeOut = 60;

        [DataMember]
        public int MaxRecords = 0;

        [DataMember]
        public bool UseParameter = true;

        [DataMember]
        public CacheSetting Cache = new CacheSetting();

        [DataMember]
        public int DbTimeCheckMinutes = 10;

        [DataMember]
        public bool UseForeignKey = true;

        [DataMember]
        public bool PartialUpdate = true;

        [DataMember]
        public Dictionary<string, DbConfig> Context;

        public void Merge(DatabaseSetting setting) {
            if (Default != setting.Default) { Default = setting.Default; }
            if (NameMapper != setting.NameMapper) { NameMapper = setting.NameMapper; }
            if (SqlTimeOut != setting.SqlTimeOut) { SqlTimeOut = setting.SqlTimeOut; }
            if (TimeConsumingSqlTimeOut != setting.TimeConsumingSqlTimeOut) { TimeConsumingSqlTimeOut = setting.TimeConsumingSqlTimeOut; }
            if (MaxRecords != setting.MaxRecords) { MaxRecords = setting.MaxRecords; }
            if (UseParameter != setting.UseParameter) { UseParameter = setting.UseParameter; }
            if (Cache != setting.Cache) { Cache = setting.Cache; }
            if (DbTimeCheckMinutes != setting.DbTimeCheckMinutes) { DbTimeCheckMinutes = setting.DbTimeCheckMinutes; }
            if (UseForeignKey != setting.UseForeignKey) { UseForeignKey = setting.UseForeignKey; }
            if (PartialUpdate != setting.PartialUpdate) { PartialUpdate = setting.PartialUpdate; }
            if(setting.Context != null && setting.Context.Count > 0) {
                Context = setting.Context;
            }
        }
    }

    [DataContract]
    public class CacheSetting {
        [DataMember]
        public bool Enabled = false;

        [DataMember]
        public int Size = 1000;

        [DataMember]
        public int KeepSecends = 300;

        [DataMember]
        public bool AllSelectedItem = false;

        public void Merge(CacheSetting setting) {
            if (Enabled != setting.Enabled) { Enabled = setting.Enabled; }
            if (Size != setting.Size) { Size = setting.Size; }
            if (KeepSecends != setting.KeepSecends) { KeepSecends = setting.KeepSecends; }
            if (AllSelectedItem != setting.AllSelectedItem) { AllSelectedItem = setting.AllSelectedItem; }
        }
    }

    [DataContract]
    public class DbConfig {
        [DataMember]
        public string DataBase;

        [DataMember]
        public string ProviderFactory;

        [DataMember]
        public string Driver;

        [DataMember]
        public string AutoScheme = "None";

        public void Merge(DbConfig setting) {
            if (DataBase != setting.DataBase) { DataBase = setting.DataBase; }
            if (ProviderFactory != setting.ProviderFactory) { ProviderFactory = setting.ProviderFactory; }
            if (Driver != setting.Driver) { Driver = setting.Driver; }
            if (AutoScheme != setting.AutoScheme) { AutoScheme = setting.AutoScheme; }
        }
    }

    [DataContract]
    public enum AutoScheme {
        None,
        CreateTable,
        AddColumns,
        RemoveColumns,
    }
}
