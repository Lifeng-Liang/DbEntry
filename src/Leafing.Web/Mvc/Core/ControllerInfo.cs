using System;
using System.Collections.Generic;
using System.Reflection;
using Leafing.Core;

namespace Leafing.Web.Mvc.Core
{
    public class ControllerInfo
    {
        public readonly string Name;
        public readonly string LowerName;
        public readonly Type Type;
        public readonly string DefaultAction;
        public readonly bool IsScaffolding;
        public readonly ListStyle ListStyle;
        public readonly Dictionary<string, ActionInfo> Actions;
        public readonly Func<object> Constructor;

        public ControllerInfo(Type type)
        {
            this.Type = type;
            var ms = GetMethods(Type);
            DefaultAction = GetDefaultAction(ms);
            Actions = GetActions(ms);
            IsScaffolding = Type.HasAttribute<ScaffoldingAttribute>(true);
            ListStyle = GetListStyle();
            Name = GetControllerName();
            LowerName = Name.ToLower();
            Constructor = ClassHelper.GetConstructorDelegate(Type);
        }

        private List<MethodInfo> GetMethods(Type type)
        {
            var list = new List<MethodInfo>();
            GetMethods(list, type);
            return list;
        }

        private void GetMethods(List<MethodInfo> infos, Type type)
        {
            Console.WriteLine(">>>" + type.Name);
            var list = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            infos.AddRange(list);
            if (type.BaseType != null && type.BaseType != typeof(ControllerBase))
            {
                GetMethods(infos, type.BaseType);
            }
        }

        public ControllerBase CreateInstance()
        {
            return (ControllerBase)Constructor();
        }

        private string GetDefaultAction(List<MethodInfo> ms)
        {
            string defaultAction = null;
            foreach (var mi in ms)
            {
                if (mi.HasAttribute<DefaultActionAttribute>(false))
                {
                    if (defaultAction == null)
                    {
                        defaultAction = mi.Name.ToLower();
                    }
                    else
                    {
                        throw new WebException("Controller only support one 'DefaultAction', the class is : " + Type.Name);
                    }
                }
            }
            return defaultAction ?? "list";
        }

        private Dictionary<string, ActionInfo> GetActions(List<MethodInfo> ms)
        {
            var dic = new Dictionary<string, ActionInfo>();
            foreach (var mi in ms)
            {
                if (!mi.HasAttribute<ExcludeAttribute>(false))
                {
                    var key = mi.Name.ToLower();
                    if(!dic.ContainsKey(key))
                    {
                        dic.Add(key, new ActionInfo(mi));
                    }
                }
            }
            return dic;
        }

        private ListStyle GetListStyle()
        {
            var lsa = Type.GetAttribute<ListStyleAttribute>(false);
            return lsa != null ? lsa.Style : ListStyle.Default;
        }

        private string GetControllerName()
        {
            var name = Type.Name;
            if (name.EndsWith("Controller"))
            {
                return name.Substring(0, name.Length - 10);
            }
            return name;
        }
    }
}
