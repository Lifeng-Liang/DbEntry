using System;
using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.Data.Model;
using Leafing.Data.Model.Member;
using System.Reflection;
using System.Reflection.Emit;
using System.Data;
using Leafing.Data.Builder.Clause;

namespace Leafing.Data.Model.Handler.Generator
{
	public class ModelHandlerGenerator
	{
		private const MethodAttributes CtMethodAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual; //public hidebysig virtual instance
		private const MethodAttributes MethodAttr = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual; //public hidebysig virtual instance
		private const MethodAttributes CtorAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

		private const BindingFlags commFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		private const TypeAttributes DynamicObjectTypeAttr = TypeAttributes.Class | TypeAttributes.Public;

		private static readonly Type[] noArgs ;
		private static readonly Type handlerBaseType ;
		private static readonly Type objectType ;
		private static readonly Type iDataReaderType ;
		private static readonly Type dictionaryStringObjectType ;
		private static readonly Type listKeyValuePairStringStringType ;
		private static readonly Type keyOpValueListType ;

		private static readonly MethodInfo handlerBaseCreateInstance ;
		private static readonly MethodInfo handlerBaseLoadSimpleValuesByIndex ;
		private static readonly MethodInfo handlerBaseLoadSimpleValuesByName ;
		private static readonly MethodInfo handlerBaseLoadRelationValuesByIndex ;
		private static readonly MethodInfo handlerBaseLoadRelationValuesByName ;
		private static readonly MethodInfo handlerBaseLoadRelationValuesByIndexNoLazy ;
		private static readonly MethodInfo handlerBaseLoadRelationValuesByNameNoLazy ;
		private static readonly MethodInfo handlerBaseTypeGetNullable ;
		private static readonly MethodInfo handlerBaseGetKeyValueDirect ;
		private static readonly MethodInfo handlerBaseGetKeyValuesDirect ;
		private static readonly MethodInfo handlerBaseSetKeyValueDirect ;
		private static readonly MethodInfo handlerBaseSetValuesForSelectDirect;
		private static readonly MethodInfo handlerBaseSetValuesForSelectDirectNoLazy;
		private static readonly MethodInfo handlerBaseSetValuesForInsertDirect ;
		private static readonly MethodInfo handlerBaseSetValuesForUpdateDirect ;
		private static readonly MethodInfo handlerBaseTypeNewSpKeyValueDirect ;
		private static readonly MethodInfo handlerBaseTypeNewKeyValue ;
		private static readonly MethodInfo handlerBaseTypeAddKeyValue ;

		private static readonly MethodInfo dataReaderMethodInt ;
		private static readonly MethodInfo dataReaderMethodString ;
		private static readonly MethodInfo dataReaderGetOrdinal;
		private static readonly MethodInfo lazyLoadingInterfaceWrite ;
		private static readonly MethodInfo belongsToInterfaceSetForeignKey ;
		private static readonly MethodInfo dictionaryStringObjectAdd ;
		private static readonly MethodInfo convertToInt64 ;
		private static readonly MethodInfo convertToInt32 ;
		private static readonly ConstructorInfo keyValuePairStringStringCtor ;
		private static readonly MethodInfo listKeyValuePairStringStringAdd ;
		private static readonly MethodInfo keyOpValueListAdd ;

		private readonly Type _model;
		private readonly TypeBuilder _result;

		private readonly ObjectInfo _info;

		static ModelHandlerGenerator ()
		{
			noArgs = new Type[]{ };
			handlerBaseType = typeof(EmitObjectHandlerBase);
			objectType = typeof(object);
			iDataReaderType = typeof(IDataReader);
			dictionaryStringObjectType = typeof(Dictionary<string, object>);
			listKeyValuePairStringStringType = typeof(List<KeyValuePair<string, string>>);
			keyOpValueListType = typeof(List<KeyOpValue>);

			handlerBaseCreateInstance = handlerBaseType.GetMethod ("CreateInstance", commFlag);
			handlerBaseLoadSimpleValuesByIndex = handlerBaseType.GetMethod("LoadSimpleValuesByIndex", commFlag);
			handlerBaseLoadSimpleValuesByName = handlerBaseType.GetMethod("LoadSimpleValuesByName", commFlag);
			handlerBaseLoadRelationValuesByIndex = handlerBaseType.GetMethod("LoadRelationValuesByIndex", commFlag);
			handlerBaseLoadRelationValuesByName = handlerBaseType.GetMethod("LoadRelationValuesByName", commFlag);
			handlerBaseLoadRelationValuesByIndexNoLazy = handlerBaseType.GetMethod("LoadRelationValuesByIndexNoLazy", commFlag);
			handlerBaseLoadRelationValuesByNameNoLazy = handlerBaseType.GetMethod("LoadRelationValuesByNameNoLazy", commFlag);
			handlerBaseTypeGetNullable = handlerBaseType.GetMethod("GetNullable", commFlag);
			handlerBaseGetKeyValueDirect = handlerBaseType.GetMethod("GetKeyValueDirect", commFlag);
			handlerBaseGetKeyValuesDirect = handlerBaseType.GetMethod("GetKeyValuesDirect", commFlag);
			handlerBaseSetKeyValueDirect = handlerBaseType.GetMethod("SetKeyValueDirect", commFlag);
			handlerBaseSetValuesForSelectDirect = handlerBaseType.GetMethod("SetValuesForSelectDirect", commFlag);
			handlerBaseSetValuesForSelectDirectNoLazy = handlerBaseType.GetMethod("SetValuesForSelectDirectNoLazy", commFlag);
			handlerBaseSetValuesForInsertDirect = handlerBaseType.GetMethod("SetValuesForInsertDirect", commFlag);
			handlerBaseSetValuesForUpdateDirect = handlerBaseType.GetMethod("SetValuesForUpdateDirect", commFlag);
			handlerBaseTypeNewSpKeyValueDirect = handlerBaseType.GetMethod("NewSpKeyValueDirect", commFlag);
			handlerBaseTypeNewKeyValue = handlerBaseType.GetMethod("NewKeyValue", commFlag);
			handlerBaseTypeAddKeyValue = handlerBaseType.GetMethod("AddKeyValue", commFlag);

			dataReaderMethodInt = typeof(IDataRecord).GetMethod("get_Item", new Type[]{typeof(int)});
			dataReaderMethodString = typeof(IDataRecord).GetMethod("get_Item", new Type[]{typeof(string)});
			dataReaderGetOrdinal = typeof(IDataRecord).GetMethod ("GetOrdinal", commFlag);
			lazyLoadingInterfaceWrite = typeof(ILazyLoading).GetMethod("Write");
			belongsToInterfaceSetForeignKey = typeof(IBelongsTo).GetMethod("set_ForeignKey");
			dictionaryStringObjectAdd = dictionaryStringObjectType.GetMethod("Add");
			convertToInt64 = typeof(Convert).GetMethod ("ToInt64", new []{ objectType });
			convertToInt32 = typeof(Convert).GetMethod ("ToInt32", new []{ objectType });
			keyValuePairStringStringCtor = typeof(KeyValuePair<string, string>).GetConstructor(new[] { typeof(string), typeof(string) });
			listKeyValuePairStringStringAdd = listKeyValuePairStringStringType.GetMethod("Add");
			keyOpValueListAdd = typeof(List<KeyOpValue>).GetMethod("Add", new[] {typeof(KeyOpValue)});
		}

		public ModelHandlerGenerator(ObjectInfo info)
		{
			this._model = info.HandleType;
			_result = CreateType();
			_info = info;
			CheckType(_model);
		}

		private TypeBuilder CreateType()
		{
			return MemoryAssembly.Instance.DefineType(
				DynamicObjectTypeAttr, 
				handlerBaseType, 
				new[] { typeof(IDbObjectHandler) });
		}

		private void CheckType(Type type)
		{
			if (type.IsSubclassOf(typeof(DbObjectSmartUpdate)))
			{
				foreach(var field in _info.SimpleMembers)
				{
					if(!field.MemberInfo.IsProperty)
					{
						throw new ModelException(
							field.MemberInfo,
							"The subclass of DbObjectModel can not has any fields, use property instead.");
					}
				}
			}
			if(_info.From.PartOf != null)
			{
				var oi = ObjectInfoFactory.Instance.GetInstance(_info.From.PartOf);
				var dic = new Dictionary<string, int>();
				foreach(var member in oi.Members)
				{
					if(member.Is.SimpleField || member.Is.LazyLoad || member.Is.BelongsTo)
					{
						dic.Add(member.Name, 1);
					}
				}
				foreach(var member in _info.SimpleMembers)
				{
					if(!dic.ContainsKey(member.Name))
					{
						throw new ModelException(member.MemberInfo,
							"The member of PartOf-model can not find in the origin model");
					}
				}
			}
		}

		public Type Generate()
		{
			GenerateConstructor();
			GenerateCreateInstance();
			GenerateLoadSimpleValuesByIndex();
			GenerateLoadSimpleValuesByName();
			GenerateLoadRelationValues(true, false);
			GenerateLoadRelationValues(false, false);
			GenerateLoadRelationValues(true, true);
			GenerateLoadRelationValues(false, true);
			GenerateGetKeyValueDirect();
			GenerateGetKeyValuesDirect();
			GenerateSetKeyValueDirect();
			GenerateSetValuesForSelectDirect();
			GenerateSetValuesForInsertDirect();
			GenerateSetValuesForUpdateDirect();
			return _result.CreateType();
		}

		private void GenerateConstructor()
		{
			_result.DefineDefaultConstructor (MethodAttributes.Public);
		}

		private void GenerateCreateInstance()
		{
			var ctor = _model.GetConstructor(noArgs);
			var method = _result.DefineMethod("CreateInstance", CtMethodAttr, objectType, noArgs);
			_result.DefineMethodOverride(method, handlerBaseCreateInstance);
			var processor = new ILBuilder(method.GetILGenerator());
			processor.NewObj(ctor);
			processor.Return();
		}

		private void GenerateLoadSimpleValuesByIndex()
		{
			var method = _result.DefineMethod("LoadSimpleValuesByIndex", MethodAttr, 
				null, new Type[]{objectType, iDataReaderType});
			_result.DefineMethodOverride (method, handlerBaseLoadSimpleValuesByIndex);
			var processor = new ILBuilder(method.GetILGenerator());

			// User u = (User)o;
			processor.DeclareLocal(_model);
			processor.LoadArg(1).Cast(_model).SetLoc(0);
			// set values
			int n = 0;
			foreach (var f in _info.SimpleMembers)
			{
				processor.LoadLoc(0);
				if (f.Is.AllowNull) { processor.LoadArg(0); }
				processor.LoadArg(2).LoadInt(n);
				var mi1 = DataReaderEmitHelper.GetMethodInfo(f.MemberType);
				if (f.Is.AllowNull || mi1 == null)
				{
					processor.CallVirtual(dataReaderMethodInt);
					if (f.Is.AllowNull)
					{
						SetSecendArgForGetNullable(f, processor);
						processor.Call(handlerBaseTypeGetNullable);
					}
					// cast or unbox
					processor.CastOrUnbox(f.MemberType);
				}
				else
				{
					processor.CallVirtual(mi1);
				}
				processor.SetMember(f);
				n++;
			}

			processor.Return();
		}

		private static void SetSecendArgForGetNullable(MemberHandler f, ILBuilder il)
		{
			if (f.MemberType.IsValueType && f.MemberType == typeof(Guid?))
			{
				il.LoadInt(1);
			}
			else if (f.MemberType.IsValueType && f.MemberType == typeof(bool?))
			{
				il.LoadInt(2);
			}
			else if (f.MemberType.IsValueType && f.MemberType == typeof(Date?))
			{
				il.LoadInt(3);
			}
			else if (f.MemberType.IsValueType && f.MemberType == typeof(Time?))
			{
				il.LoadInt(4);
			}
			else
			{
				il.LoadInt(0);
			}
		}

		private void GenerateLoadSimpleValuesByName()
		{
			var method = _result.DefineMethod("LoadSimpleValuesByName", MethodAttr, 
				null, new Type[]{objectType, iDataReaderType});
			_result.DefineMethodOverride (method, handlerBaseLoadSimpleValuesByName);
			var processor = new ILBuilder(method.GetILGenerator());

			// User u = (User)o;
			processor.DeclareLocal(_model);
			processor.LoadArg(1).Cast(_model).SetLoc(0);
			// set values
			foreach (var f in _info.SimpleMembers)
			{
				processor.LoadLoc(0);
				if (f.Is.AllowNull) { processor.LoadArg(0); }
				processor.LoadArg(2).LoadArg(2).LoadString(f.Name).CallVirtual(dataReaderGetOrdinal);
				var mi1 = DataReaderEmitHelper.GetMethodInfo(f.MemberType);
				if (f.Is.AllowNull || mi1 == null)
				{
					processor.CallVirtual(dataReaderMethodInt);
					if (f.Is.AllowNull)
					{
						SetSecendArgForGetNullable(f, processor);
						processor.Call(handlerBaseTypeGetNullable);
					}
					// cast or unbox
					processor.CastOrUnbox(f.MemberType);
				}
				else
				{
					processor.CallVirtual(mi1);
				}
				processor.SetMember(f);
			}

			processor.Return();
		}

		private void GenerateLoadRelationValues(bool useIndex, bool noLazy)
		{
			int index = _info.SimpleMembers.Length;
			string methodName = useIndex ? "LoadRelationValuesByIndex" : "LoadRelationValuesByName";
			if (noLazy)
			{
				methodName = methodName + "NoLazy";
			}

			var method = _result.DefineMethod(methodName, MethodAttr, 
				null, new Type[]{objectType, iDataReaderType});
			_result.DefineMethodOverride (method, useIndex
				? (noLazy ? handlerBaseLoadRelationValuesByIndexNoLazy : handlerBaseLoadRelationValuesByIndex)
				: (noLazy ? handlerBaseLoadRelationValuesByNameNoLazy : handlerBaseLoadRelationValuesByName));
			var processor = new ILBuilder(method.GetILGenerator());

			if(_info.RelationMembers.Length > 0)
			{
				// User u = (User)o;
				processor.DeclareLocal(_model);
				processor.LoadArg(1).Cast(_model).SetLoc(0);
				// set values
				foreach (var f in _info.RelationMembers)
				{
					if (f.Is.LazyLoad)
					{
						if (noLazy)
						{
							processor.LoadLoc(0);
							processor.GetMember(f);
							processor.LoadArg(2);
							if (useIndex)
							{
								processor.LoadInt(index++).CallVirtual(dataReaderMethodInt);
							}
							else
							{
								processor.LoadString(f.Name).CallVirtual(dataReaderMethodString);
							}
							processor.LoadInt(0);
							processor.CallVirtual(lazyLoadingInterfaceWrite);
						}
					}
					else if (f.Is.BelongsTo)
					{
						processor.LoadLoc(0);
						processor.GetMember(f);
						processor.LoadArg(2);
						if (useIndex)
						{
							processor.LoadInt(index++).CallVirtual(dataReaderMethodInt);
						}
						else
						{
							processor.LoadString(f.Name).CallVirtual(dataReaderMethodString);
						}
						processor.CallVirtual(belongsToInterfaceSetForeignKey);
					}
				}
			}

			processor.Return();
		}

		private void GenerateGetKeyValueDirect()
		{
			var method = _result.DefineMethod("GetKeyValueDirect", MethodAttr, 
				objectType, new Type[]{objectType});
			_result.DefineMethodOverride (method, handlerBaseGetKeyValueDirect);
			var processor = new ILBuilder(method.GetILGenerator());

			if (_info.KeyMembers.Length == 1)
			{
				var h = _info.KeyMembers[0];
				processor.LoadArg(1).Cast(_model);
				processor.GetMember(h);
				processor.Box(h.MemberType);
			}
			else
			{
				processor.LoadNull();
			}

			processor.Return();
		}

		private void GenerateGetKeyValuesDirect()
		{
			var method = _result.DefineMethod("GetKeyValuesDirect", MethodAttr, 
				null, new Type[]{dictionaryStringObjectType, objectType});
			_result.DefineMethodOverride (method, handlerBaseGetKeyValuesDirect);
			var processor = new ILBuilder(method.GetILGenerator());

			// User u = (User)o;
			processor.DeclareLocal(_model);
			processor.LoadArg(2).Cast(_model).SetLoc(0);
			// set values
			foreach (var f in _info.KeyMembers)
			{
				processor.LoadArg(1).LoadString(f.Name).LoadLoc(0);
				processor.GetMember(f);
				processor.Box(f.MemberType).CallVirtual(dictionaryStringObjectAdd);
			}

			processor.Return();
		}

		private void GenerateSetKeyValueDirect()
		{
			var method = _result.DefineMethod("SetKeyValueDirect", MethodAttr, 
				null, new Type[]{objectType, objectType});
			_result.DefineMethodOverride (method, handlerBaseSetKeyValueDirect);
			var processor = new ILBuilder(method.GetILGenerator());

			if (_info.KeyMembers.Length == 1)
			{
				var h = _info.KeyMembers[0];
				processor.LoadArg(1).Cast(_model);
				processor.LoadArg(2);
				var fh = _info.KeyMembers[0];
				if (fh.MemberType == typeof(long))
				{
					processor.Call(convertToInt64);
				}
				else if (fh.MemberType == typeof(int))
				{
					processor.Call(convertToInt32);
				}
				else if (fh.MemberType == typeof(Guid))
				{
					processor.Unbox(h.MemberType);
				}
				else
				{
					processor.Cast(h.MemberType);
				}
				processor.SetMember(h);
			}

			processor.Return();
		}

		private void GenerateSetValuesForSelectDirect()
		{
			GenerateSetValuesForSelectDirectDirect("SetValuesForSelectDirect", false);
			GenerateSetValuesForSelectDirectDirect("SetValuesForSelectDirectNoLazy", true);
		}

		private void GenerateSetValuesForSelectDirectDirect(string methodName, bool noLazy)
		{
			var method = _result.DefineMethod(methodName, MethodAttr, 
				null, new Type[]{listKeyValuePairStringStringType});
			_result.DefineMethodOverride (method, noLazy 
				? handlerBaseSetValuesForSelectDirectNoLazy 
				: handlerBaseSetValuesForSelectDirect);
			var processor = new ILBuilder(method.GetILGenerator());

			foreach (var f in _info.Members)
			{
				if (!f.Is.HasOne && !f.Is.HasMany && !f.Is.HasAndBelongsToMany)
				{
					if (noLazy || !f.Is.LazyLoad)
					{
						processor.LoadArg(1);

						processor.LoadString(f.Name);
						if (f.Name != f.MemberInfo.Name)
						{
							processor.LoadString(f.MemberInfo.Name);
						}
						else
						{
							processor.LoadNull();
						}
						processor.NewObj(keyValuePairStringStringCtor);

						processor.CallVirtual(listKeyValuePairStringStringAdd);
					}
				}
			}

			processor.Return();
		}

		private void GenerateSetValuesForInsertDirect()
		{
			//TODO: implements this
			var method = _result.DefineMethod("SetValuesForInsertDirect", MethodAttr, 
				null, new Type[]{keyOpValueListType, objectType});
			_result.DefineMethodOverride (method, handlerBaseSetValuesForInsertDirect);
			var processor = new ILBuilder(method.GetILGenerator());

			GenerateSetValuesDirect(processor,
				m => m.Is.UpdatedOn,
				m => m.Is.CreatedOn || m.Is.SavedOn || m.Is.Count);

			processor.Return();
		}

		private void GenerateSetValuesForUpdateDirect()
		{
			//TODO: implements this
			var method = _result.DefineMethod("SetValuesForUpdateDirect", MethodAttr, 
				null, new Type[]{keyOpValueListType, objectType});
			_result.DefineMethodOverride (method, handlerBaseSetValuesForUpdateDirect);
			var processor = new ILBuilder(method.GetILGenerator());

			if (_model.BaseType == typeof(DbObjectSmartUpdate))
			{
				GenerateSetValuesForPartialUpdate(processor);
			}
			else
			{
				GenerateSetValuesDirect(processor,
					m => m.Is.CreatedOn || m.Is.Key,
					m => m.Is.UpdatedOn || m.Is.SavedOn || m.Is.Count);
			}

			processor.Return();
		}

		private void GenerateSetValuesDirect(ILBuilder processor, Func<MemberHandler, bool> cb1, Func<MemberHandler, bool> cb2)
		{
			// User u = (User)o;
			processor.DeclareLocal(_model);
			processor.LoadArg(2).Cast(_model).SetLoc(0);
			// set values
			int n = 0;
			foreach (var f in _info.Members)
			{
				if (!f.Is.DbGenerate && !f.Is.HasOne && !f.Is.HasMany && !f.Is.HasAndBelongsToMany)
				{
					if (!cb1(f))
					{
						processor.LoadArg(1).LoadArg(0).LoadInt(n);
						if (cb2(f))
						{
							processor.LoadInt(f.Is.Count ? 1 : 2)
								.Call(handlerBaseTypeNewSpKeyValueDirect);
						}
						else
						{
							processor.LoadLoc(0);
							processor.GetMember(f);
							if (f.Is.BelongsTo)
							{
								processor.CallVirtual(f.MemberType.GetMethod("get_ForeignKey"));
							}
							else if (f.Is.LazyLoad)
							{
								var it = f.MemberType.GetGenericArguments()[0];
								processor.CallVirtual(f.MemberType.GetMethod("get_Value"));
								processor.Box(it);
							}
							else
							{
								processor.Box(f.MemberType);
							}
							processor.Call(handlerBaseTypeNewKeyValue);
						}
						processor.CallVirtual(keyOpValueListAdd);
					}
					n++;
				}
			}
		}

		private void GenerateSetValuesForPartialUpdate(ILBuilder processor)
		{
			// User u = (User)o;
			processor.DeclareLocal(_model);
			processor.LoadArg(2).Cast(_model).SetLoc(0);
			// set values
			int n = 0;
			foreach (var f in _info.Members)
			{
				if (!f.Is.DbGenerate && !f.Is.HasOne && !f.Is.HasMany && !f.Is.HasAndBelongsToMany)
				{
					if (!f.Is.Key && (f.Is.UpdatedOn || f.Is.SavedOn || !f.Is.CreatedOn || f.Is.Count))
					{
						if (f.Is.UpdatedOn || f.Is.SavedOn || f.Is.Count)
						{
							processor.LoadArg(1).LoadArg(0).LoadInt(n)
								.LoadInt(f.Is.Count ? 1 : 2)
								.Call(handlerBaseTypeNewSpKeyValueDirect).CallVirtual(keyOpValueListAdd);
						}
						else
						{
							processor.LoadArg(0).LoadArg(1).LoadLoc(0).LoadString(f.Name).LoadInt(n).LoadLoc(0);
							processor.GetMember(f);
							if (f.Is.BelongsTo)
							{
								processor.CallVirtual(f.MemberType.GetMethod("get_ForeignKey"));
							}
							else if (f.Is.LazyLoad)
							{
								var it = f.MemberType.GetGenericArguments()[0];
								processor.CallVirtual(f.MemberType.GetMethod("get_Value"));
								processor.Box(it);
							}
							else
							{
								processor.Box(f.MemberType);
							}
							processor.Call(handlerBaseTypeAddKeyValue);
						}
					}
					n++;
				}
			}
		}
	}
}
