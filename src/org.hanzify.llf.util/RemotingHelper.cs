
#region usings

using System;
using System.Configuration;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using org.hanzify.llf.util.Setting;

#endregion

namespace org.hanzify.llf.util
{
	public static class RemotingHelper
	{
        private static readonly string RemotingUriPrefix = ConfigHelper.DefaultSettings.GetValue("RemotingUriPrefix", "");

		public static void EventModeRegister(int Port)
		{
			BinaryServerFormatterSinkProvider s = new BinaryServerFormatterSinkProvider();
			BinaryClientFormatterSinkProvider c = new BinaryClientFormatterSinkProvider();
			s.TypeFilterLevel = TypeFilterLevel.Full;

            IDictionary p = Hashtable.Synchronized(new Hashtable());
			p["port"] = Port;
			IChannel channel = new TcpChannel(p, c, s);
            ChannelServices.RegisterChannel(channel, false);
		}

		public static void EventModeServerRegister()
		{
            int Port = ConfigHelper.DefaultSettings.GetValue("RemotingPort", 9145);
			RemotingHelper.EventModeRegister(Port);
		}

		public static void EventModeClientRegister()
		{
			EventModeRegister(0);
		}

		public static ObjRef Marshal(MarshalByRefObject o, Type t)
		{
			return RemotingServices.Marshal(o, t.FullName);
		}

		public static T GetObject<T>()
		{
            Type t = typeof(T);
			object o = Activator.GetObject(
				t, RemotingUriPrefix + t.FullName);
			return (T)o;
		}
	}
}
