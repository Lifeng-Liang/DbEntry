using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Common;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Lephone.Processor
{
    public class ModelRelationFixer
    {
        private readonly Type _type;
        private readonly TypeDefinition _model;
        private readonly ObjectInfo _info;

        public ModelRelationFixer(Type type, TypeDefinition model)
        {
            this._type = type;
            this._model = model;
            _info = ObjectInfoFactory.Instance.GetInstance(_type);
        }

        public void Process()
        {
            if(_info.RelationFields.Length > 0)
            {
                foreach(var c in _model.Methods)
                {
                    if(c.IsConstructor)
                    {
                        ProcessConstructor(c);
                    }
                }
            }
        }

        private void ProcessConstructor(MethodDefinition constructor)
        {
            var removes = new List<int>();
            var list = constructor.Body.Instructions;
            var n = list.Count - 4;
            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].OpCode == OpCodes.Nop && i < n)
                {
                    if(list[i+1].OpCode == OpCodes.Nop && 
                        list[i+2].OpCode == OpCodes.Nop && 
                        list[i+3].OpCode == OpCodes.Nop && 
                        list[i+4].OpCode == OpCodes.Ldstr)
                    {
                        removes.Insert(0, i);
                        ProcessLoadString(list[i + 4]);
                        i += 3;
                    }
                }
            }
            foreach(var i in removes)
            {
                list.RemoveAt(i);
                list.RemoveAt(i);
                list.RemoveAt(i);
                list.RemoveAt(i);
            }
        }

        private void ProcessLoadString(Instruction instruction)
        {
            var key = (string)instruction.Operand;
            foreach(var field in _info.RelationFields)
            {
                if(field.MemberInfo.Name == key)
                {
                    instruction.Operand = GetName(field);
                }
            }
        }

        private string GetName(MemberHandler f)
        {
            if(f.IsLazyLoad)
            {
                return f.Name;
            }
            if (f.IsHasOne || f.IsHasMany)
            {
                var oi1 = new ObjectInfoBase(f.FieldType.GetGenericArguments()[0]);
                var mh = oi1.GetBelongsTo(_type);
                if (mh == null)
                {
                    throw new DataException("HasOne/HasMany and BelongsTo must be paired.");
                }
                return mh.Name;
            }
            if (f.IsHasAndBelongsToMany)
            {
                var oi1 = new ObjectInfoBase(f.FieldType.GetGenericArguments()[0]);
                var mh = oi1.GetHasAndBelongsToMany(_type);
                if (mh == null)
                {
                    throw new DataException("HasOne/HasMany and BelongsTo must be paired.");
                }
                return mh.Name;
            }
            if (f.IsBelongsTo)
            {
                return f.Name;
            }
            throw new ApplicationException("Impossiable!");
        }
    }
}
