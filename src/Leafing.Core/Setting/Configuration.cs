using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Leafing.Core.Setting {
    [DataContract]
    public class Configration {
        [DataMember]
        public Dictionary<string, string> AppSettings = new Dictionary<string, string>();

        [DataMember]
        public ServiceSetting Service = new ServiceSetting();

        [DataMember]
        public LogSetting Log = new LogSetting();

        [DataMember]
        public DatabaseSetting Database = new DatabaseSetting();
    }

    [DataContract]
    public class ServiceSetting {
        [DataMember]
        public int DelayToStart = 30000;

        [DataMember]
        public int MinStartTicks = 180000;
    }

    [DataContract]
    public class LogSetting {
        [DataMember]
        public string Level = "All";

        [DataMember]
        public string FileName = "{0}{1}.{2}.log";

        [DataMember]
        public Dictionary<string, List<string>> Recorders = new Dictionary<string, List<string>>();
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
    }

    [DataContract]
    public enum AutoScheme {
        None,
        CreateTable,
        AddColumns,
        RemoveColumns,
    }
}
