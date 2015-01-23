using System;
using System.Collections.Generic;
using Leafing.Data.Builder.Clause;
using Leafing.Data.Dialect;
using Leafing.Data.Model;
using Leafing.Data.Model.Member;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Common
{
    public class AutoSchemeFixer
    {
        protected static object LockRoot = new object();

        public static AutoSchemeFixer CreateInstance(DataProvider provider, ObjectInfo info)
        {
            switch(provider.Driver.AutoScheme)
            {
                case AutoScheme.None:
                    return new AutoSchemeFixer();
                case AutoScheme.CreateTable:
                    return new AutoSchemeFixerCreateTable(provider, info);
                case AutoScheme.AddColumns:
                    return new AutoSchemeFixerAddColumns(provider, info);
                case AutoScheme.RemoveColumns:
                    return new AutoSchemeFixerRemoveColumns(provider, info);
                default:
                    throw new DataException("Application Error");
            }
        }

        protected bool Processed = false;

        protected AutoSchemeFixer()
        {
        }

        public virtual void TryFix()
        {
        }

        public void SetAsProcessed()
        {
            Processed = true;
        }
    }

    public class AutoSchemeFixerCreateTable : AutoSchemeFixer
    {
        private readonly List<Type> _createTables;
        protected readonly DataProvider Provider;

        public AutoSchemeFixerCreateTable(DataProvider provider, ObjectInfo info)
        {
            this.Provider = provider;
            _createTables = GetCreateTables(info);
        }

        private List<Type> GetCreateTables(ObjectInfo info)
        {
            if (info.From.PartOf != null)
            {
                return new List<Type> { info.From.PartOf };
            }
            if (info.From.JoinClauseList != null)
            {
                var jar = new Dictionary<Type, int>();
                foreach (JoinClause jc in info.From.JoinClauseList)
                {
                    if (jc.Type1 != null)
                    {
                        jar[jc.Type1] = 1;
                    }
                    if (jc.Type2 != null)
                    {
                        jar[jc.Type2] = 1;
                    }
                }
                if (jar.Count > 0)
                {
                    return new List<Type>(jar.Keys);
                }
            }
            return new List<Type> { info.HandleType };
        }

        public override void TryFix()
        {
            lock (LockRoot)
            {
                if(!Processed)
                {
                    foreach (var type in _createTables)
                    {
                        var ctx = ModelContext.GetInstance(type);
                        InnerTryFix(ctx);
                        ctx.Operator.Fixer.SetAsProcessed();
                    }
                }
            }
        }

        protected virtual void InnerTryFix(ModelContext ctx)
        {
            string name = ctx.Info.From.MainTableName;
            if (name != null)
            {
                if (!ctx.Provider.Driver.TableNames.Contains(name.ToLower()))
                {
                    CreateTable(ctx);
                }
                else
                {
                    FixColumns(ctx);
                }
            }
        }

        protected virtual void CreateTable(ModelContext ctx)
        {
            ctx.Operator.CreateTableAndRelations(ctx);
        }

        protected virtual void FixColumns(ModelContext ctx)
        {
        }
    }

    public class AutoSchemeFixerAddColumns : AutoSchemeFixerCreateTable
    {
        public AutoSchemeFixerAddColumns(DataProvider provider, ObjectInfo info) : base(provider, info)
        {
        }

        protected override void FixColumns(ModelContext ctx)
        {
            string name = ctx.Info.From.MainTableName;
            var cs = ctx.Provider.GetDbColumnInfoList(name);
            foreach(var member in ctx.Info.Members)
            {
                if(!member.Is.RelationField && !member.Name.StartsWith("$") 
                    && !cs.Exists(p => p.ColumnName.ToUpper() == member.Name.ToUpper()))
                {
                    ctx.AddColumn(member.Name, GetDefaultValue(member, ctx.Provider.Dialect));
                }
            }
        }

        private object GetDefaultValue(MemberHandler member, DbDialect dialect)
        {
            if(member.Is.AllowNull)
            {
                return null;
            }
            var t = member.MemberType;
            if(t == typeof(string) || t == typeof(byte[]))
            {
                return dialect.EmptyString;
            }
            if(t == typeof(DateTime) || t == typeof(Date) || t == typeof(Time))
            {
                return dialect.QuoteDateTimeValue(dialect.DefaultDateTimeString());
            }
            if(t == typeof(Guid))
            {
                return "'" + Guid.NewGuid().ToString() + "'";
            }
            return 0;
        }
    }

    public class AutoSchemeFixerRemoveColumns : AutoSchemeFixerAddColumns
    {
        public AutoSchemeFixerRemoveColumns(DataProvider provider, ObjectInfo info) : base(provider, info)
        {
        }

        protected override void FixColumns(ModelContext ctx)
        {
            var sd = ctx.Info.SoftDeleteColumnName;
            if(sd != null)
            {
                sd = sd.ToUpper();
            }
            base.FixColumns(ctx);
            string name = ctx.Info.From.MainTableName;
            var cs = ctx.Provider.GetDbColumnInfoList(name);
            var list = new List<MemberHandler>(ctx.Info.Members);
            var rcs = new List<string>();
            foreach(var c in cs)
            {
                if(!list.Exists(p => p.Name.ToUpper() == c.ColumnName.ToUpper()))
                {
                    if(c.ColumnName.ToUpper() != sd)
                    {
                        rcs.Add(c.ColumnName);
                    }
                }
            }
            if(rcs.Count > 0)
            {
                ctx.DropColumn(rcs.ToArray());
            }
        }
    }
}
