using System;
using System.Data;
using System.Text;
using Leafing.Data.Model.Member;

namespace Leafing.Data.Model.Creator
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
			catch (FormatException ex)
			{
				ThrowClearExceptionMsg (useIndex, ctx, dr, ex);
				throw;
			}
            catch (InvalidCastException ex)
            {
				ThrowClearExceptionMsg (useIndex, ctx, dr, ex);
				throw;
            }
        }

		private static void ThrowClearExceptionMsg(bool useIndex, ModelContext ctx, IDataReader dr, Exception ex)
		{
			if (useIndex)
			{
				FindMemberCastFailByIndex(ctx, dr, ex);
			}
			else
			{
				FindMemberCastFailByName(ctx, dr, ex);
			}
		}

        private static void FindMemberCastFailByIndex(ModelContext ctx, IDataReader dr, Exception ex)
        {
            var text = new StringBuilder();
            var ms = ctx.Info.SimpleMembers;
            for (int i = 0; i < ms.Length; i++)
            {
                CheckMemberCast(ms[i], dr[i], text);
            }
            if(text.Length > 0)
            {
                throw new DataException(text.ToString(), ex);
            }
        }

        private static void FindMemberCastFailByName(ModelContext ctx, IDataReader dr, Exception ex)
        {
            var text = new StringBuilder();
            var ms = ctx.Info.SimpleMembers;
            foreach (var member in ms)
            {
                CheckMemberCast(member, dr[member.Name], text);
            }
            if (text.Length > 0)
            {
                throw new DataException(text.ToString(), ex);
            }
        }

        private static void CheckMemberCast(MemberHandler member, object value, StringBuilder text)
        {
			if ((value == null || value == DBNull.Value) && member.Is.AllowNull)
            {
                return;
            }
            if (member.MemberType.IsEnum && value is Int32)
            {
                return;
            }
			var vt = value.GetType();
			if (member.MemberType == typeof(long) && vt == typeof(int)) {
				return;
			}
			if(member.MemberType == typeof(Date) && vt == typeof(DateTime)) {
				return;
			}
			if (member.MemberType == typeof(Date?) && vt == typeof(DateTime?)) {
				return;
			}
            if (member.MemberType != value.GetType())
            {
                text.AppendFormat("The type of member [{0}] is [{1}] but the type in Database of it is [{2}]\n",
                                  member.Name, member.MemberType, value.GetType());
            }
        }
    }
}
