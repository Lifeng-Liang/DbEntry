using System;
using System.Data;
using Lephone.Data.Model.Member;

namespace Lephone.Data.Model.Creator
{
    class PureObjectModelCreator : ModelCreator
    {
        protected readonly ModelContext Ctx;

        public PureObjectModelCreator(Type dbObjectType, bool useIndex, bool noLazy)
            : base(dbObjectType, useIndex, noLazy)
        {
            Ctx = ModelContext.GetInstance(dbObjectType);
        }

        public override object CreateObject(IDataReader dr)
        {
            object obj = Ctx.NewObject();
            InitObject(obj, dr);
            return obj;
        }

        protected virtual void InitObject(object obj, IDataReader dr)
        {
            LoadValues(obj, Ctx, dr, UseIndex, NoLazy);
        }

        protected static void LoadValues(object obj, ModelContext ctx, IDataReader dr, bool useIndex, bool noLazy)
        {
            try
            {
                ctx.Handler.LoadSimpleValues(obj, useIndex, dr);
                ctx.Handler.LoadRelationValues(obj, useIndex, noLazy, dr);
            }
            catch (InvalidCastException ex)
            {
                if (useIndex)
                {
                    FindMemberCastFailByIndex(ctx, dr, ex);
                }
                else
                {
                    FindMemberCastFailByName(ctx, dr, ex);
                }
                throw;
            }
        }

        private static void FindMemberCastFailByIndex(ModelContext ctx, IDataReader dr, InvalidCastException ex)
        {
            var ms = ctx.Info.SimpleMembers;
            for (int i = 0; i < ms.Length; i++)
            {
                CheckMemberCast(ms[i], dr[i], ex);
            }
        }

        private static void FindMemberCastFailByName(ModelContext ctx, IDataReader dr, InvalidCastException ex)
        {
            var ms = ctx.Info.SimpleMembers;
            foreach (var member in ms)
            {
                CheckMemberCast(member, dr[member.Name], ex);
            }
        }

        private static void CheckMemberCast(MemberHandler member, object value, InvalidCastException ex)
        {
            if (value == DBNull.Value && member.Is.AllowNull)
            {
                return;
            }
            if (member.MemberType.IsEnum && value is Int32)
            {
                return;
            }
            if (member.MemberType != value.GetType())
            {
                throw new DataException(string.Format("The type of member [{0}] is [{1}] but sql value of it is [{2}]",
                                        member.Name, member.MemberType, value.GetType()), ex);
            }
        }
    }
}
