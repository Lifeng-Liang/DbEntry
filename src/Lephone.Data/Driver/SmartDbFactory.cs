using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Lephone.Util;

namespace Lephone.Data.Driver
{
    internal class SmartDbFactory : DbFactory
    {
        private readonly object[] CiParam = new object[] { };
        private ConstructorInfo CiCommand;
        private ConstructorInfo CiConnection;
        private ConstructorInfo CiDataAdapter;
        private ConstructorInfo CiParameter;
        private ConstructorInfo CiCommandBuilder;
        private MethodInfo MiCb_DeriveParameters;

        public bool DeriveParametersIsValid
        {
            get { return MiCb_DeriveParameters != null; }
        }

        public void Init(string AssemblyName)
        {
            InitWithAssemblyName(AssemblyName);
        }

        private void InitWithAssemblyName(string AssemblyName)
        {
            var EmptyParam = new Type[] { };
            Assembly asm = Assembly.Load(AssemblyName);
            Type[] ts = asm.GetTypes();
            Type CommandType = null;
            foreach (Type t in ts)
            {
                if (CiCommand == null && IsInterfaceOf(t, typeof(IDbCommand)))
                {
                    CommandType = t;
                    CiCommand = t.GetConstructor(EmptyParam);
                }
                if (CiConnection == null && IsInterfaceOf(t, typeof(IDbConnection)))
                {
                    CiConnection = t.GetConstructor(EmptyParam);
                }
                if (CiDataAdapter == null && IsInterfaceOf(t, typeof(IDbDataAdapter)))
                {
                    CiDataAdapter = t.GetConstructor(EmptyParam);
                }
                if (CiParameter == null && IsInterfaceOf(t, typeof(IDbDataParameter)))
                {
                    CiParameter = t.GetConstructor(EmptyParam);
                }
                if(CiCommandBuilder == null && IsInterfaceOf(t, typeof(DbCommandBuilder)))
                {
                    CiCommandBuilder = t.GetConstructor(EmptyParam);
                }
            }
            AssertConstructorNotNull(CiCommand, CiConnection, CiDataAdapter, CiParameter);
            TryGetDeriveParametersMethod(ts, CommandType);
        }

        private void TryGetDeriveParametersMethod(Type[] ts, Type CommandType)
        {
            CommonHelper.IfCatchException(true, delegate
            {
                foreach (Type t in ts)
                {
                    if (t.Name.EndsWith("CommandBuilder"))
                    {
                        //TODO: why left this?
                        //object[] os = new object[] { };
                        MiCb_DeriveParameters = t.GetMethod("DeriveParameters",
                                                          ClassHelper.StaticFlag,
                                                          null,
                                                          CallingConventions.Any,
                                                          new[] {CommandType},
                                                          null);
                      break;
                    }
                }
            });
        }

        private static bool IsInterfaceOf(Type t, Type it)
        {
            if (t.GetInterface(it.Name) == null)
            {
                return false;
            }
            return true;
        }

        private static void AssertConstructorNotNull(params ConstructorInfo[] ts)
        {
            foreach (ConstructorInfo t in ts)
            {
                if (t == null)
                {
                    throw new ArgumentNullException("ts", "Type is null, please check info of App.config.");
                }
            }
        }

        public override IDbCommand CreateCommand()
        {
            return (IDbCommand)CiCommand.Invoke(CiParam);
        }

        public override IDbConnection CreateConnection()
        {
            return (IDbConnection)CiConnection.Invoke(CiParam);
        }

        public override IDbDataAdapter CreateDataAdapter()
        {
            return (IDbDataAdapter)CiDataAdapter.Invoke(CiParam);
        }

        public override IDbDataParameter CreateParameter()
        {
            return (IDbDataParameter)CiParameter.Invoke(CiParam);
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            if(CiCommandBuilder != null)
            {
                return (DbCommandBuilder)CiCommandBuilder.Invoke(CiParam);
            }
            return null;
        }

        public void DeriveParameters(IDbCommand Command)
        {
            if (MiCb_DeriveParameters != null)
            {
                MiCb_DeriveParameters.Invoke(null, new object[] { Command });
            }
            else
            {
                throw new DataException("DeriveParameters not found.");
            }
        }
    }
}
