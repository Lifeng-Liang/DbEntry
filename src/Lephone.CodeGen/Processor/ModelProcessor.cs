using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Common;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class ModelProcessor
    {
        private const string MemberPrifix = "$";
        private readonly TypeDefinition _model;

        public ModelProcessor(TypeDefinition model)
        {
            this._model = model;
        }

        public void Process()
        {
            Console.WriteLine(_model.FullName);
            ProcessSerializable();
            ProcessProperties();
            ProcessConstructors();
            RemoveCompilerGeneratedFields();
        }

        private void RemoveCompilerGeneratedFields()
        {
            for (int i = _model.Fields.Count - 1; i >= 0; i--)
            {
                var field = _model.Fields[i];
                if (field.IsCompilerGenerated())
                {
                    _model.Fields.RemoveAt(i);
                }
            }
        }

        private static void ProcessSerializable()
        {
            //TypeAttributes ta = DynamicObjectTypeAttr;
            //Type[] interfaces = null;
            //if (sourceType.IsSerializable)
            //{
            //    ta |= TypeAttributes.Serializable;
            //    interfaces = new[] { typeof(ISerializable) };
            //}
            //if (sourceType.IsSerializable)
            //{
            //    MethodInfo mi = typeof(DynamicObjectReference).GetMethod("SerializeObject", ClassHelper.StaticFlag);
            //    tb.OverrideMethod(ImplFlag, "GetObjectData", typeof(ISerializable), null,
            //                      new[] { typeof(SerializationInfo), typeof(StreamingContext) },
            //                      il => il.LoadArg(0).LoadArg(1).LoadArg(2).Call(mi));
            //}
            //TODO: process Serializable
        }

        private void ProcessProperties()
        {
            var list = GetProperties();
            foreach (var pi in list)
            {
                ProcessProperty(pi);
            }
        }

        private void ProcessConstructors()
        {
            foreach (var c in _model.Methods)
            {
                if (c.IsConstructor)
                {
                    //TODO: process constructor
                }
            }
        }

        private List<PropertyDefinition> GetProperties()
        {
            var pis = _model.Properties;
            var plist = new List<PropertyDefinition>();
            foreach (PropertyDefinition pi in pis)
            {
                if (pi.SetMethod != null && pi.GetMethod != null)
                {
                    if (!pi.PropertyType.IsValueType && !pi.PropertyType.IsArray && pi.PropertyType.FullName != "System.String")
                    {
                        var ft = GetFieldType(pi);
                        if (ft == FieldType.Normal || ft == FieldType.LazyLoad)
                        {
                            throw new DataException("The property '{0}' should define as relation field and can not set lazy load attribute", pi.Name);
                        }
                    }
                    plist.Add(pi);
                }
            }
            return plist;
        }

        private static FieldType GetFieldType(PropertyDefinition pi)
        {
            foreach (CustomAttribute ca in pi.CustomAttributes)
            {
                switch (ca.Constructor.DeclaringType.FullName)
                {
                    case "Lephone.Data.Definition.HasOneAttribute":
                        return FieldType.HasOne;
                    case "Lephone.Data.Definition.BelongsToAttribute":
                        return FieldType.BelongsTo;
                    case "Lephone.Data.Definition.HasManyAttribute":
                        return FieldType.HasMany;
                    case "Lephone.Data.Definition.HasAndBelongsToManyAttribute":
                        return FieldType.HasAndBelongsToMany;
                    case "Lephone.Data.Definition.LazyLoadAttribute":
                        return FieldType.LazyLoad;
                }
            }
            return FieldType.Normal;
        }

        private void ProcessProperty(PropertyDefinition pi)
        {
            FieldType ft = GetFieldType(pi);

            var fi = DefineField(MemberPrifix + pi.Name, pi.PropertyType, ft, pi);
            _model.Fields.Add(fi);

            ProcessPropertyGet(pi, ft, fi);
            ProcessPropertySet(pi, ft, fi);

        }

        private static void ProcessPropertyGet(PropertyDefinition pi, FieldType ft, FieldDefinition fi)
        {
            var method = pi.GetMethod;
            RemovePropertyCompilerGeneratedAttribute(method);
            method.Body.Instructions.Clear();
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.LoadArg(0);
            processor.LoadField(fi);
            processor.Return();
            //OverrideMethod(OverrideFlag, "get_" + pi.Name, originType, pi.PropertyType, null, delegate(ILBuilder il)
            //{
            //    il.LoadArg(0);
            //    il.LoadField(fi);
            //    if (ft == FieldType.BelongsTo || ft == FieldType.HasOne || ft == FieldType.LazyLoad)
            //    {
            //        MethodInfo getValue = fi.FieldType.GetMethod("get_Value", ClassHelper.InstancePublic);
            //        il.CallVirtual(getValue);
            //    }
            //});
            //TODO: implements property get
        }

        private static void ProcessPropertySet(PropertyDefinition pi, FieldType ft, FieldDefinition fi)
        {
            var method = pi.SetMethod;
            RemovePropertyCompilerGeneratedAttribute(method);
            method.Body.Instructions.Clear();
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.LoadArg(0);
            processor.LoadArg(1);
            processor.SetField(fi);
            processor.Return();
            //**var mupdate = new MethodDefinition("m_ColumnUpdated", Mono.Cecil.MethodAttributes.Public, null);
            //OverrideMethod(OverrideFlag, SetPropertyName, originType, null, new[] { pi.PropertyType }, delegate(ILBuilder il)
            //{
            //    Label label = il.DefineLabel();
            //    bool hasLabel = false;
            //    if (ft == FieldType.Normal)
            //    {
            //        hasLabel = EmitCompareStatement(il, pi.PropertyType, fi, label);
            //    }
            //    else if (ft == FieldType.HasOne || ft == FieldType.BelongsTo || ft == FieldType.LazyLoad)
            //    {
            //        il.LoadArg(0);
            //        il.LoadField(fi).LoadArg(1);
            //        MethodInfo setValue = fi.FieldType.GetMethod("set_Value", ClassHelper.InstancePublic);
            //        il.CallVirtual(setValue);
            //    }
            //    else
            //    {
            //        il.LoadArg(0);
            //    }
            //    if (ft == FieldType.LazyLoad || ft == FieldType.Normal)
            //    {
            //        if (mupdate != null)
            //        {
            //            string cn = ObjectInfo.GetColumuName(new MemberAdapter.PropertyAdapter(pi));
            //            il.LoadArg(0).LoadString(cn).Call(mupdate);
            //        }
            //    }
            //    if (hasLabel)
            //    {
            //        il.MarkLabel(label);
            //    }
            //});
            //TODO: implements property set
        }

        private static void RemovePropertyCompilerGeneratedAttribute(MethodDefinition method)
        {
            foreach (var attribute in method.CustomAttributes)
            {
                if(attribute.Constructor.DeclaringType.FullName == TypeHelper.CompilerGenerated)
                {
                    method.CustomAttributes.Remove(attribute);
                    return;
                }
            }
        }

        /*
        private static bool EmitCompareStatement(ILBuilder il, Type propertyType, FieldInfo fi, Label label)
        {
            bool hasLabel = false;
            if (propertyType.IsValueType)
            {
                hasLabel = true;
                if (propertyType.IsGenericType)
                {
                    var l1 = il.DefineLabel();
                    var l2 = il.DefineLabel();
                    var getValueOrDefault = propertyType.GetMethod("GetValueOrDefault", new Type[] { });
                    var getHasValue = propertyType.GetMethod("get_HasValue", ClassHelper.AllFlag);
                    var realType = propertyType.GetGenericArguments()[0];
                    il.DeclareLocal(propertyType).DeclareLocal(propertyType).DeclareLocal(typeof(bool));
                    il.LoadArg(1).SetLoc(0).LoadArg(0).LoadField(fi).SetLoc(1).LoadLocala_S(0);
                    if (realType == typeof(decimal))
                    {
                        il.Call(getValueOrDefault).LoadLocala_S(1).Call(getValueOrDefault).Call(TypeDict[realType]);
                        il.BrTrue_S(l1).LoadLocala_S(0).Call(getHasValue).LoadLocala_S(1).Call(getHasValue);
                        il.Ceq().LoadInt(0).Ceq().Br_S(l2);
                    }
                    else if (TypeDict.ContainsKey(realType))
                    {
                        var l3 = il.DefineLabel();
                        var l4 = il.DefineLabel();
                        il.Call(getHasValue).LoadLocala_S(1).Call(getHasValue).Bne_Un_S(l1);
                        il.LoadLocala_S(0).Call(getHasValue).BrFalse_S(l4).LoadLocala_S(0);
                        il.Call(getValueOrDefault).LoadLocala_S(1).Call(getValueOrDefault).Call(TypeDict[realType]).Br_S(l3);
                        il.MarkLabel(l4).LoadInt(0);
                        il.MarkLabel(l3).Br_S(l2);
                    }
                    else
                    {
                        il.Call(getValueOrDefault);
                        il.ConvFloaty(realType);
                        il.LoadLocala_S(1).Call(getValueOrDefault);
                        il.ConvFloaty(realType);
                        il.Bne_Un_S(l1).LoadLocala_S(0).Call(getHasValue).LoadLocala_S(1).Call(getHasValue);
                        il.Ceq().LoadInt(0).Ceq().Br_S(l2);
                    }
                    il.MarkLabel(l1).LoadInt(1);
                    il.MarkLabel(l2).LoadInt(0);
                    il.Ceq().SetLoc(2).LoadLoc(2).BrTrue_S(label);
                }
                else
                {
                    il.DeclareLocal(typeof(bool));
                    if (TypeDict.ContainsKey(propertyType))
                    {
                        il.LoadArg(1).LoadArg(0).LoadField(fi);
                        MethodInfo mi = TypeDict[propertyType];
                        il.Call(mi).LoadInt(0).Ceq().SetLoc(0).LoadLoc(0).BrTrue_S(label);
                    }
                    else
                    {
                        il.LoadArg(1).LoadArg(0).LoadField(fi).Ceq();
                        il.SetLoc(0).LoadLoc(0).BrTrue_S(label);
                    }
                }
            }
            else if (propertyType == typeof(string))
            {
                hasLabel = true;
                il.DeclareLocal(typeof(bool));
                il.LoadArg(1).LoadArg(0).LoadField(fi);
                MethodInfo mi = TypeDict[propertyType];
                il.Call(mi).LoadInt(0).Ceq().SetLoc(0).LoadLoc(0).BrTrue_S(label);
            }
            il.LoadArg(0).LoadArg(1).SetField(fi);
            return hasLabel;
        }
        */

        private static FieldDefinition DefineField(string name, TypeReference propertyType, FieldType ft, PropertyDefinition pi)
        {
            if (ft == FieldType.Normal)
            {
                return new FieldDefinition(name, FieldAttributes.Private, propertyType);
            }
            var t = GetRealType(propertyType, ft);
            var fb = new FieldDefinition(name, FieldAttributes.FamORAssem, t);
            var bc = pi.GetCustomAttribute("Lephone.Data.Definition.DbColumnAttribute");
            if (bc != null)
            {
                fb.CustomAttributes.Add(bc);
            }
            else if (ft == FieldType.LazyLoad)
            {
                //fb.SetCustomAttribute(GetDbColumnBuilder(pi.Name));
                //TODO: set [DbColumn(pi.Name)] to fb
            }
            if (ft == FieldType.HasAndBelongsToMany)
            {
                var mm = pi.GetCustomAttribute("Lephone.Data.Definition.HasAndBelongsToManyAttribute");
                //if (!string.IsNullOrEmpty(mm.CrossTableName))
                //{
                //    fb.SetCustomAttribute(new CustomAttributeBuilder(CrossTableNameAttributeConstructor, new object[] { mm.CrossTableName }));
                //}
                //TODO: set [CrossTableName(mm.CrossTableName)] to fb
            }
            if (ft == FieldType.LazyLoad)
            {
                //ProcessCustomAttribute<AllowNullAttribute>(pi, o => fb.SetCustomAttribute(GetAllowNullBuilder()));
                //ProcessCustomAttribute<LengthAttribute>(pi, o => fb.SetCustomAttribute(GetLengthBuilder(o)));
                //ProcessCustomAttribute<StringColumnAttribute>(pi, o => fb.SetCustomAttribute(GetStringColumnBuilder(o)));
                //ProcessCustomAttribute<IndexAttribute>(pi, o => fb.SetCustomAttribute(GetIndexBuilder(o)));
                //ProcessCustomAttribute<SpecialNameAttribute>(pi, o => fb.SetCustomAttribute(GetSpecialNameBuilder()));
                //TODO: set all arrtibutes to LazyLoad field.
            }
            return fb;
        }

        private static TypeDefinition GetRealType(TypeReference propertyType, FieldType ft)
        {
            switch (ft)
            {
                //case FieldType.HasOne:
                //    t = typeof(HasOne<>);
                //    t = t.MakeGenericType(propertyType);
                //    break;
                //case FieldType.HasMany:
                //    t = typeof(HasMany<>);
                //    t = t.MakeGenericType(propertyType.GetGenericArguments()[0]);
                //    break;
                //case FieldType.BelongsTo:
                //    t = typeof(BelongsTo<>);
                //    t = t.MakeGenericType(propertyType);
                //    break;
                //case FieldType.HasAndBelongsToMany:
                //    t = typeof(HasAndBelongsToMany<>);
                //    t = t.MakeGenericType(propertyType.GetGenericArguments()[0]);
                //    break;
                //case FieldType.LazyLoad:
                //    t = typeof(LazyLoadField<>);
                //    t = t.MakeGenericType(propertyType);
                //    break;
                default:
                    throw new DataException("Impossible");
            }
        }
    }
}
