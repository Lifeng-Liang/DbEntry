using System;
using System.Text;
using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.Data.Model.Member;

namespace Leafing.CodeGen
{
    public class MvcActionGenerator
    {
        private Type _classType;
        private readonly string _actionName;

        public MvcActionGenerator(string fileName, string className, string actionName)
        {
            Helper.EnumTypes(fileName, t =>
            {
                if (t.FullName == className)
                {
                    _classType = t;
                    return false;
                }
                return true;
            });
            _actionName = actionName;
        }

        public override string ToString()
        {
            switch (_actionName)
            {
                case "New":
                    return GetActionNew();
                case "Create":
                    return GetActionCreate();
                case "List":
                    return GetActionList();
                case "Show":
                    return GetActionShow();
                case "Edit":
                    return GetActionEdit();
                case "Update":
                    return GetActionUpdate();
                case "Destroy":
                    return GetActionDestroy();
                case "All":
                    var sb = new StringBuilder();
                    sb.Append(GetActionNew());
                    sb.Append(GetActionCreate());
                    sb.Append(GetActionList());
                    sb.Append(GetActionShow());
                    sb.Append(GetActionEdit());
                    sb.Append(GetActionUpdate());
                    sb.Append(GetActionDestroy());
                    return sb.ToString();
            }
            throw new ArgsErrorException(5, "Action name invalid.");
        }

        private static string GetActionNew()
        {
            return @"
    public override void New()
    {
    }
";
        }

        private string GetActionCreate()
        {
            var sb = new StringBuilder(@"
    public override string Create()
    {
");
            sb.Append("        var").Append(" obj = ").Append(_classType.Name).Append(".New;\n\n");
            var ctx = ModelContext.GetInstance(_classType);
            foreach (MemberHandler m in ctx.Info.Members)
            {
                if (!m.Is.RelationField && !m.Is.DbGenerate && !m.Is.AutoSavedValue)
                {
                    string s = "Ctx.Request.Form[\"" + _classType.Name.ToLower() + "[" + m.Name.ToLower() + "]\"]";
                    Type t = m.Is.LazyLoad ? m.MemberType.GetGenericArguments()[0] : m.MemberType;
                    sb.Append("        obj.").Append(m.Name).Append(" = ");
                    GetFieldCode(sb, t, s);
                    sb.Append(";\n");
                }
            }
            if (_classType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                sb.Append("\n        obj.Save();");
            }
            else
            {
                sb.Append("\n        DbEntry.Save(obj);");
            }
            sb.Append(@"
        Flash.Notice = """).Append(_classType.Name).Append(@" was successfully created"";
        return UrlTo.Action(""list"");
    }
");
            return sb.ToString();
        }

        private static void GetFieldCode(StringBuilder sb, Type t, string s)
        {
            switch (t.Name)
            {
                case "String":
                    sb.Append(s);
                    break;
                case "Int32":
                    sb.Append("int.Parse(").Append(s).Append(")");
                    break;
                case "Int64":
                    sb.Append("long.Parse(").Append(s).Append(")");
                    break;
                case "Single":
                    sb.Append("float.Parse(").Append(s).Append(")");
                    break;
                case "Double":
                    sb.Append("double.Parse(").Append(s).Append(")");
                    break;
                case "Decimal":
                    sb.Append("decimal.Parse(").Append(s).Append(")");
                    break;
                case "Boolean":
                    sb.Append("(").Append(s).Append(" == \"on\") ? true : false");
                    break;
                case "DateTime":
                    sb.Append("DateTime.Parse(").Append(s).Append(")");
                    break;
                case "Date":
                    sb.Append("Date.Parse(").Append(s).Append(")");
                    break;
                case "Time":
                    sb.Append("Time.Parse(").Append(s).Append(")");
                    break;
                default:
                    if(t.IsEnum)
                    {
                        sb.Append("(").Append(t.Name).Append(")Enum.Parse(typeof(").Append(t.Name).Append("), ").Append(s).Append(")");
                    }
                    else
                    {
                        sb.Append("ControllerHelper.ChangeType(").Append(s).Append(", typeof(").Append(t.Name).Append(")");
                    }
                    break;
            }
        }

        private string GetActionList()
        {
            return @"
    public override void List(long? pageIndex, int? pageSize)
    {
        if (pageIndex < 1)
        {
            throw new DataException(""The pageIndex out of supported range."");
        }
        int psize = pageSize ?? WebSettings.DefaultPageSize;
        IPagedSelector ps = DbEntry.From<" + _classType.Name + @">().Where(Condition.Empty).OrderBy(""Id DESC"")
            .PageSize(psize).GetPagedSelector();
        this[""ItemList""] = ps.GetItemList<T>((pageIndex ?? 1) - 1);
    }
";
        }

        private string GetActionShow()
        {
            if(_classType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                return @"
    public override void Show(long n)
    {
        this[""Item""] = " + _classType.Name + @".FindById(n);
    }
";
            }

            return @"
    public override void Show(long n)
    {
        this[""Item""] = DbEntry.GetObject<" + _classType.Name + @">(n);
    }
";
        }

        private string GetActionEdit()
        {
            if(_classType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                return @"
    public override void Edit(long n)
    {
        this[""Item""] = " + _classType.Name + @".FindById(n);
    }
";
            }
            return @"
    public override void Edit(long n)
    {
        this[""Item""] = DbEntry.GetObject<" + _classType.Name + @">(n);
    }
";
        }

        private string GetActionUpdate()
        {
            var sb = new StringBuilder(@"
    public override string Update(long n)
    {
        ").Append(_classType.Name).Append(" obj = ");
            if (_classType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                sb.Append(_classType.Name).Append(".FindById(n);\n\n");
            }
            else
            {
                sb.Append("DbEntry.GetObject<").Append(_classType.Name).Append(">(n);\n\n");
            }

            var ctx = ModelContext.GetInstance(_classType);
            foreach (MemberHandler m in ctx.Info.Members)
            {
                if (m.Is.RelationField) { continue; }
                if (!m.Is.AutoSavedValue && !m.Is.DbGenerate)
                {
                    string s = "Ctx.Request.Form[\"" + _classType.Name.ToLower() + "[" + m.Name.ToLower() + "]\"]";
                    Type t = m.Is.LazyLoad ? m.MemberType.GetGenericArguments()[0] : m.MemberType;
                    sb.Append("        obj.").Append(m.Name).Append(" = ");
                    GetFieldCode(sb, t, s);
                    sb.Append(";\n");
                }
            }


            if(_classType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                sb.Append("\n        obj.Save();\n");
            }
            else
            {
                sb.Append("\n        DbEntry.Save(obj);\n");
            }
            sb.Append("        Flash.Notice = \"").Append(_classType.Name).Append(" was successfully updated\";");
            sb.Append(@"
        return UrlTo.Action(""show"").Parameters(n);
    }
");
            return sb.ToString();
        }

        private string GetActionDestroy()
        {
            if (_classType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                return @"
    public override string Destroy(long n)
    {
        " + _classType.Name + @" o = " + _classType.Name + @".FindById(n);
        if (o != null)
        {
            o.Delete();
            return UrlTo.Action(""list"");
        }
    }
";
            }
            return @"
    public override string Destroy(long n)
    {
        " + _classType.Name + @" o = DbEntry.GetObject<" + _classType.Name + @">(n);
        if (o != null)
        {
            DbEntry.Delete(o);
            return UrlTo.Action(""list"");
        }
    }
";
        }
    }
}
