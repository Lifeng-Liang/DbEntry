using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Leafing.Core;
using Leafing.Core.Setting;
using Leafing.Core.Text;
using Leafing.Data.Builder.Clause;
using Leafing.Data.Definition;
using Leafing.Data.Model.Member;
using Leafing.Data.Model.Member.Adapter;

namespace Leafing.Data.Model
{
    public class ObjectInfo
    {
        public readonly bool AllowSqlLog;
        public readonly bool Cacheable;
        public readonly string ContextName;
		public readonly string ShowString;
        public readonly List<string> QueryRequiredFields;
        public readonly Dictionary<Type, CrossTable> CrossTables = new Dictionary<Type, CrossTable>();
        public readonly string DeleteToTableName;
        public readonly FromClause From;
        public readonly Type HandleType;
        public readonly bool HasRelation;
        public readonly bool HasOnePrimaryKey;
        public readonly bool HasSystemKey;
        public readonly Dictionary<string, List<ASC>> Indexes = new Dictionary<string, List<ASC>>();
        public readonly MemberHandler[] KeyMembers;
        public readonly MemberHandler LockVersion;
        public readonly MemberHandler[] Members;
        public readonly MemberHandler[] RelationMembers;
        public readonly MemberHandler[] SimpleMembers;
        public readonly string SoftDeleteColumnName;
        public readonly Dictionary<string, List<MemberHandler>> UniqueIndexes = new Dictionary<string, List<MemberHandler>>();

        internal ObjectInfo(Type t)
        {
            this.HandleType = t;
            this.CheckTypeIsModel();
            List<MemberHandler> members = this.GetMembers();
            this.KeyMembers = members.FindAll(m => m.Is.Key).ToArray();
            this.CheckKeys();
            this.SimpleMembers = this.GetSimpleMembers(members);
            this.RelationMembers = members.FindAll(m => !m.Is.SimpleField).ToArray();
            this.Members = this.GetAllMembers();
            this.From = GetObjectFromClause(t);
            this.AllowSqlLog = this.IsAllowSqlLog();
            this.HasSystemKey = this.GetHasSystemKey();
            this.HasRelation = members.Exists(delegate(MemberHandler m)
            {
                if ((!m.Is.HasOne && !m.Is.HasMany) && !m.Is.HasAndBelongsToMany)
                {
                    return m.Is.BelongsTo;
                }
                return true;
            });
            this.LockVersion = members.FirstOrDefault(m => m.Is.LockVersion);
            this.HasOnePrimaryKey = this.GetHasOnePrimaryKey();
            this.SoftDeleteColumnName = this.GetSoftDeleteColumnName();
            this.DeleteToTableName = this.GetDeleteToTableName();
            this.ContextName = this.GetContextName();
			this.ShowString = this.GetShowString();
            this.QueryRequiredFields = this.GetQueryRequiredFields();
            this.Cacheable = this.HandleType.HasAttribute<CacheableAttribute>(false);
            this.GetIndexes();
            SetManyToManyFrom(this, this.From.MainModelName, this.Members);
        }

        private List<string> GetQueryRequiredFields()
        {
            List<string> result = null;
            foreach (var member in Members)
            {
                if (member.MemberInfo.HasAttribute<QueryRequiredAttribute>(false))
                {
                    if(result == null)
                    {
                        result = new List<string>();
                    }
                    result.Add(member.Name);
                }
            }
            return result;
        }

        private static void CheckIndexAttributes(IEnumerable<IndexAttribute> ias)
        {
            var list = new List<string>();
            foreach (var attribute in ias)
            {
                foreach (string name in list)
                {
                    if (attribute.IndexName == name)
                    {
                        throw new ApplicationException("Cann't set the same name index more than ones at the same column.");
                    }
                }
                list.Add(attribute.IndexName);
            }
        }

        private void CheckKeys()
        {
            if (this.KeyMembers.Length > 1)
            {
                foreach (MemberHandler member in this.KeyMembers)
                {
                    if (member.Is.DbGenerate)
                    {
                        throw new ModelException(member.MemberInfo, "Multiple key do not allow to be SystemGeneration!");
                    }
                }
            }
        }

        private void CheckTypeIsModel()
        {
            if (this.HandleType.IsNotPublic)
            {
                throw new ModelException(this.HandleType, "The model class should be public.");
            }
            if (ClassHelper.GetArgumentlessConstructor(this.HandleType) == null)
            {
                throw new ModelException(this.HandleType, "The model need a public/protected(DbObjectModel) argumentless constructor");
            }
            var list = new List<Type>(this.HandleType.GetInterfaces());
            if (!list.Contains(typeof(IDbObject)))
            {
                throw new ModelException(this.HandleType, "The data object must implements IDbObject!");
            }
        }

        private MemberHandler[] GetAllMembers()
        {
            var list = new List<MemberHandler>(this.SimpleMembers);
            list.AddRange(this.RelationMembers);
            return list.ToArray();
        }

        internal MemberHandler GetBelongsTo(Type t)
        {
            foreach (MemberHandler member in this.RelationMembers)
            {
                if (member.Is.BelongsTo)
                {
                    var type = member.MemberType.GetGenericArguments()[0];
                    if (type == t)
                    {
                        return member;
                    }
                }
            }
            return null;
        }

        private string GetContextName()
        {
			var attr = this.HandleType.GetAttribute<DbContextAttribute>(true);
            if (attr != null)
            {
                return attr.ContextName;
            }
            return null;
        }

		private string GetShowString()
		{
			var attr = this.HandleType.GetAttribute<ShowStringAttribute>(true);
			if (attr != null)
			{
				return attr.ShowString;
			}
			return HandleType.Name;
		}

        private static string GetCrossTableName(MemberHandler f, string unmappedMainTableName, string unmappedSlaveTableName)
        {
            string crossTableName;
            if (!string.IsNullOrEmpty(f.CrossTableName))
            {
                crossTableName = f.CrossTableName;
            }
            else
            {
                crossTableName = (unmappedMainTableName.CompareTo(unmappedSlaveTableName) > 0) 
                    ? (unmappedSlaveTableName + "_" + unmappedMainTableName) 
                    : (unmappedMainTableName + "_" + unmappedSlaveTableName);
            }
            return (NameMapper.Instance.Prefix + crossTableName);
        }

        private string GetDeleteToTableName()
        {
            var attribute = this.HandleType.GetAttribute<DeleteToAttribute>(true);
            if (attribute != null)
            {
                return attribute.TableName;
            }
            return null;
        }

        internal MemberHandler GetHasAndBelongsToMany(Type t)
        {
            foreach (MemberHandler member in this.RelationMembers)
            {
                if (member.Is.HasAndBelongsToMany)
                {
                    Type type = member.MemberType.GetGenericArguments()[0];
                    if (type == t)
                    {
                        return member;
                    }
                }
            }
            return null;
        }

        private bool GetHasOnePrimaryKey()
        {
            return ((this.KeyMembers != null) && (this.KeyMembers.Length == 1));
        }

        private bool GetHasSystemKey()
        {
            if ((this.KeyMembers == null) || (this.KeyMembers.Length != 1))
            {
                return false;
            }
            if (!this.KeyMembers[0].Is.DbGenerate)
            {
                return (this.KeyMembers[0].MemberType == typeof(Guid));
            }
            return true;
        }

        private void GetIndexes()
        {
            foreach (MemberHandler member in this.Members)
            {
                IndexAttribute[] attributes = member.MemberInfo.GetAttributes<IndexAttribute>(false);
                CheckIndexAttributes(attributes);
                foreach (IndexAttribute attribute in attributes)
                {
                    ASC item = attribute.ASC ? ((ASC)member.Name) : ((DESC)member.Name);
                    string key = attribute.IndexName ?? item.Key;
                    if (!this.Indexes.ContainsKey(key))
                    {
                        this.Indexes.Add(key, new List<ASC>());
                    }
                    this.Indexes[key].Add(item);
                    if (attribute.UNIQUE)
                    {
                        if (!this.UniqueIndexes.ContainsKey(key))
                        {
                            this.UniqueIndexes.Add(key, new List<MemberHandler>());
                        }
                        this.UniqueIndexes[key].Add(member);
                    }
                }
            }
        }

        private static FromClause GetJoinedFrom(Type modelType, JoinOnAttribute[] joins)
        {
            if (joins.Length == 0)
            {
                string definedName = NameMapper.Instance.MapName(modelType.Name);
                return new FromClause(modelType.Name, GetTableNameFromConfig(definedName));
            }
            var joinClauseList = new JoinClause[joins.Length];
            for (int i = 0; i < joins.Length; i++)
            {
                int index = joins[i].Index;
                if (index < 0)
                {
                    index = i;
                }
                joinClauseList[index] = joins[i].Joinner;
            }
            foreach (var t in joinClauseList)
            {
                if (t == null)
                {
                    throw new ArgumentException(string.Format("class [{0}] JoinOnAttribute defined error.", modelType.Name));
                }
            }
            return new FromClause(joinClauseList);
        }

        private List<MemberHandler> GetMembers()
        {
            var members = new List<MemberHandler>();
            foreach (FieldInfo fi in this.HandleType.GetFields(ClassHelper.InstanceFlag))
            {
                if (!fi.IsPrivate)
                {
                    ProcessMember(MemberAdapter.NewObject(fi), members);
                }
            }
            foreach (PropertyInfo pi in this.HandleType.GetProperties(ClassHelper.InstanceFlag))
            {
				var getMethod = pi.GetGetMethod(true);
				if ((pi.CanRead && pi.CanWrite) && ((getMethod != null) && !getMethod.IsPrivate))
                {
					ProcessMember(MemberAdapter.NewObject(pi), members);
                }
            }
            return members;
        }

        internal static FromClause GetObjectFromClause(Type modelType)
        {
            var attribute = modelType.GetAttribute<DbTableAttribute>(false);
            var attributes = modelType.GetAttributes<JoinOnAttribute>(false);
            if ((attribute != null) && (attributes.Length != 0))
            {
                throw new ArgumentException(string.Format("class [{0}] defined DbTable and JoinOn. Only one allowed.", modelType.Name));
            }
            if (attribute == null)
            {
                return GetJoinedFrom(modelType, attributes);
            }
            if (attribute.TableName == null)
            {
                return new FromClause(attribute.PartOf);
            }
            return new FromClause(GetTableNameFromConfig(attribute.TableName));
        }

        private MemberHandler[] GetSimpleMembers(List<MemberHandler> members)
        {
            var list = new List<MemberHandler>(this.KeyMembers);
            list.AddRange(members.Where(member => member.Is.SimpleField && !member.Is.Key));
            return list.ToArray();
        }

        private string GetSoftDeleteColumnName()
        {
            var attribute = this.HandleType.GetAttribute<SoftDeleteAttribute>(true);
            if (attribute != null)
            {
                return attribute.ColumnName;
            }
            return null;
        }

        private static string GetTableNameFromConfig(string definedName)
        {
            return ConfigHelper.LeafingSettings.GetValue("@" + definedName, definedName);
        }

        private bool IsAllowSqlLog()
        {
            if (this.HandleType.GetCustomAttributes(typeof(DisableSqlLogAttribute), false).Length != 0)
            {
                return false;
            }
            return true;
        }

        private static void ProcessMember(MemberAdapter m, IList<MemberHandler> members)
        {
            //if (((!m.HasAttribute<ExcludeAttribute>(false) && !m.HasAttribute<HasOneAttribute>(false)) && (!m.HasAttribute<HasManyAttribute>(false) && !m.HasAttribute<HasAndBelongsToManyAttribute>(false))) && (!m.HasAttribute<BelongsToAttribute>(false) && !m.HasAttribute<LazyLoadAttribute>(false)))
			if(!IsExcludeMember(m))
            {
				var member = MemberHandler.NewObject(m);
				if (member.Is.Key)
				{
					members.Add(member);
				}
				else
				{
					members.Add(member);
				}
            }
        }

		private static bool IsExcludeMember(MemberAdapter m)
		{
			if (m.HasAttribute<ExcludeAttribute> (false)) {
				return true;
			}
//			if(m.MemberType.IsGenericType) {
//				if (m.MemberType.GetGenericTypeDefinition() == typeof(BelongsTo<,>)) {
//					return false;
//				}
//				if(m.MemberType.IsNullable()){
//					return false;
//				}
//				return true;
//			}
			return false;
		}

        private static void SetManyToManyFrom(ObjectInfo oi, string unmappedMainTableName, IEnumerable<MemberHandler> fields)
        {
            foreach (MemberHandler member in fields)
            {
                if (member.Is.HasAndBelongsToMany)
                {
                    var modelType = member.MemberType.GetGenericArguments()[0];
					if (oi.KeyMembers[0].MemberType != GetIdType(modelType)) {
						throw new DataException("HasAndBelongsToMany tables should have the same type of Id(s)");
					}
                    var fromClause = GetObjectFromClause(modelType);
                    var mainOriginTableName = fromClause.MainModelName;
                    var name = GetCrossTableName(member, unmappedMainTableName, mainOriginTableName);
                    var fkMainOriginTableName = mainOriginTableName + "_Id";
                    var from = new FromClause(new[] { new JoinClause(name, fkMainOriginTableName, fromClause.MainTableName, "Id", CompareOpration.Equal, JoinMode.Inner) });
					oi.CrossTables[modelType] = new CrossTable(modelType, from, name,
                        oi.From.MainTableName, unmappedMainTableName + "_Id", fromClause.MainTableName, fkMainOriginTableName);
                }
            }
        }

		private static Type GetIdType(Type type)
		{
			return type.GetProperty("Id").PropertyType;
		}

		public override string ToString ()
		{
			return string.Format ("Handle Type : [{0}]", HandleType);
		}
    }
}

