using System;
using System.Text;
using Lephone.Data.Common;
using Lephone.Data.Definition;

namespace Lephone.CodeGen
{
    public class RailsActionGenerator
    {
        private Type ClassType;
        private readonly string ActionName;

        public RailsActionGenerator(string fileName, string className, string actionName)
        {
            Helper.EnumTypes(fileName, t =>
            {
                if (t.FullName == className)
                {
                    ClassType = t;
                    return false;
                }
                return true;
            });
            ActionName = actionName;
        }

        public override string ToString()
        {
            switch (ActionName)
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
    public virtual void New()
    {
    }
";
        }

        private string GetActionCreate()
        {
            var sb = new StringBuilder(@"
    public virtual void Create()
    {
");
            sb.Append("        ").Append(ClassType.Name).Append(" obj = new ").Append(ClassType.Name).Append("();\n\n");
            ObjectInfo oi = ObjectInfo.GetInstance(ClassType);
            foreach (MemberHandler m in oi.Fields)
            {
                if (!m.IsRelationField && !m.IsDbGenerate && !m.IsAutoSavedValue)
                {
                    string s = "ctx.Request.Form[\"" + ClassType.Name.ToLower() + "[" + m.Name.ToLower() + "]\"]";
                    Type t = m.IsLazyLoad ? m.FieldType.GetGenericArguments()[0] : m.FieldType;
                    sb.Append("        obj.").Append(m.Name).Append(" = ");
                    GetFieldCode(sb, t, s);
                    sb.Append(";\n");
                }
            }
            if (ClassType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                sb.Append("\n        obj.Save();");
            }
            else
            {
                sb.Append("\n        DbEntry.Save(obj);");
            }
            sb.Append(@"
        Flash[""notice""] = """).Append(ClassType.Name).Append(@" was successfully created"";
        RedirectTo(new UTArgs{Action = ""list""});
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
                    sb.Append("bool.Parse(").Append(s).Append(")");
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
                        sb.Append("Enum.Parse(typeof(").Append(t.Name).Append("), ").Append(s).Append(")");
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
    public virtual void List(int pageIndex, int? pageSize)
    {
        if (pageIndex < 0)
        {
            throw new DataException(""The pageIndex out of supported range."");
        }
        if (pageIndex != 0)
        {
            pageIndex--;
        }
        int psize = pageSize ?? WebSettings.DefaultPageSize;
        IPagedSelector ps = DbEntry.From<" + ClassType.Name + @">().Where(null).OrderBy(""Id DESC"")
            .PageSize(psize).GetPagedSelector();
        bag[""list""] = ps.GetCurrentPage(pageIndex);
        bag[""list_count""] = ps.GetResultCount();
        bag[""list_pagesize""] = WebSettings.DefaultPageSize;
    }
";
        }

        private string GetActionShow()
        {
            if(ClassType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                return @"
    public virtual void Show(int n)
    {
        bag[""item""] = " + ClassType.Name + @".FindById(n);
    }
";
            }

            return @"
    public virtual void Show(int n)
    {
        bag[""item""] = DbEntry.GetObject<" + ClassType.Name + @">(n);
    }
";
        }

        private string GetActionEdit()
        {
            if(ClassType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                return @"
    public virtual void Edit(int n)
    {
        bag[""item""] = " + ClassType.Name + @".FindById(n);
    }
";
            }
            return @"
    public virtual void Edit(int n)
    {
        bag[""item""] = DbEntry.GetObject<" + ClassType.Name + @">(n);
    }
";
        }

        private string GetActionUpdate()
        {
            var sb = new StringBuilder(@"
    public virtual void Update(int n)
    {
        ").Append(ClassType.Name).Append(" obj = ");
            if (ClassType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                sb.Append(ClassType.Name).Append(".FindById(n);\n\n");
            }
            else
            {
                sb.Append("DbEntry.GetObject<").Append(ClassType.Name).Append(">(n);\n\n");
            }

            ObjectInfo oi = ObjectInfo.GetInstance(ClassType);
            foreach (MemberHandler m in oi.Fields)
            {
                if (m.IsRelationField) { continue; }
                if (!m.IsAutoSavedValue && !m.IsDbGenerate)
                {
                    string s = "ctx.Request.Form[\"" + ClassType.Name.ToLower() + "[" + m.Name.ToLower() + "]\"]";
                    Type t = m.IsLazyLoad ? m.FieldType.GetGenericArguments()[0] : m.FieldType;
                    sb.Append("        obj.").Append(m.Name).Append(" = ");
                    GetFieldCode(sb, t, s);
                    sb.Append(";\n");
                }
            }


            if(ClassType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                sb.Append("\n        obj.Save();\n");
            }
            else
            {
                sb.Append("\n        DbEntry.Save(obj);\n");
            }
            sb.Append("        Flash[\"notice\"] = \"").Append(ClassType.Name).Append(" was successfully updated\";");
            sb.Append(@"
        RedirectTo(new UTArgs{Action = ""show""}, n);
    }
");
            return sb.ToString();
        }

        private string GetActionDestroy()
        {
            if (ClassType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                return @"
    public virtual void Destroy(int n)
    {
        " + ClassType.Name + @" o = " + ClassType.Name + @".FindById(n);
        if (o != null)
        {
            o.Delete();
            RedirectTo(new UTArgs{Action = ""list""});
        }
    }
";
            }
            return @"
    public virtual void Destroy(int n)
    {
        " + ClassType.Name + @" o = DbEntry.GetObject<" + ClassType.Name + @">(n);
        if (o != null)
        {
            DbEntry.Delete(o);
            RedirectTo(new UTArgs{Action = ""list""});
        }
    }
";
        }
    }
}
