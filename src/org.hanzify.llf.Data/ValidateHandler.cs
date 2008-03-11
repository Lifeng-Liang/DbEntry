
#region usings

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Lephone.Util;
using Lephone.Util.Text;
using Lephone.Data.Common;
using Lephone.Data.Definition;

#endregion

namespace Lephone.Data
{
    public class ValidateHandler
    {
        private string _InvalidFieldText;
        private string _NotAllowNullText;
        private string _NotMatchedText;
        private string _LengthText;
        private string _ShouldBeUniqueText;
        private string _SeparatorText;

        private bool EmptyAsNull;
        private bool IncludeClassName;
        public bool IsValid;

        private Dictionary<string, string> _ErrorMessages;

        public Dictionary<string, string> ErrorMessages
        {
            get { return _ErrorMessages; }
        }

        public ValidateHandler()
            : this(false)
        {
        }

        public ValidateHandler(bool EmptyAsNull)
            : this(EmptyAsNull, false, "Invalid Field {0} {1}.", "Not Allow Null", "Not Matched", "The length should be {0} to {1} but was {2}", "Should be UNIQUED", ", ")
        {
        }

        public ValidateHandler(bool EmptyAsNull, bool IncludeClassName, string InvalidFieldText, string NotAllowNullText, string NotMatchedText, string LengthText, string ShouldBeUniqueText, string SeparatorText)
        {
            this.IsValid = true;
            this.EmptyAsNull = EmptyAsNull;
            this.IncludeClassName = IncludeClassName;

            this._InvalidFieldText = InvalidFieldText;
            this._NotAllowNullText = NotAllowNullText;
            this._NotMatchedText = NotMatchedText;
            this._LengthText = LengthText;
            this._ShouldBeUniqueText = ShouldBeUniqueText;
            this._SeparatorText = SeparatorText;
            
            _ErrorMessages = new Dictionary<string, string>();
        }

        public bool ValidateObject(object obj)
        {
            this.IsValid = true;
            this._ErrorMessages.Clear();

            Type t = obj.GetType();
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            string tn = oi.BaseType.Name;
            bool IsNew = false;
            if (oi.KeyFields.Length > 0)
            {
                IsNew = oi.IsNewObject(obj);
            }
            
            Type StringType = typeof(string);
            foreach (MemberHandler fh in oi.Fields)
            {
                if (fh.FieldType == StringType || (fh.IsLazyLoad && fh.FieldType.GetGenericArguments()[0] == StringType))
                {
                    string Field = fh.IsLazyLoad ? ((LazyLoadField<string>)fh.GetValue(obj)).Value : (string)fh.GetValue(obj);
                    StringBuilder ErrMsg = new StringBuilder();
                    bool isValid = ValidateField(Field, fh, ErrMsg);
                    if (ErrMsg.Length > 2) { ErrMsg.Length -= 2; }
                    if (!isValid)
                    {
                        string n = (IncludeClassName ? tn + "." + fh.Name : fh.Name);
                        _ErrorMessages[n] = string.Format(_InvalidFieldText, n, ErrMsg);
                    }
                    this.IsValid &= isValid;
                }
            }
            WhereCondition EditCondition = IsNew ? null : !ObjectInfo.GetKeyWhereClause(obj);
            foreach (List<MemberHandler> mhs in oi.UniqueIndexes.Values)
            {
                WhereCondition c = null;
                string n = "";
                foreach (MemberHandler h in mhs)
                {
                    object v = h.GetValue(obj);
                    if (v != null && v.GetType().IsGenericType)
                    {
                        v = v.GetType().GetField("m_Value", ClassHelper.AllFlag).GetValue(v);
                    }
                    c &= (CK.K[h.Name] == v);
                    n += h.Name;
                }
                if (c != null)
                {
                    if (DbEntry.Context.GetResultCount(t, c && EditCondition) != 0)
                    {
                        this.IsValid = false;
                        if (_ErrorMessages.ContainsKey(n))
                        {
                            _ErrorMessages[n] = string.Format("{0}{1}{2}", _ErrorMessages[n], _SeparatorText, _ShouldBeUniqueText);
                        }
                        else
                        {
                            _ErrorMessages[n] = string.Format(_InvalidFieldText, n, _ShouldBeUniqueText);
                        }
                    }
                }
            }
            return this.IsValid;
        }

        private bool ValidateField(string Field, MemberHandler fh, StringBuilder ErrMsg)
        {
            if (Field == null || (Field == "" && EmptyAsNull))
            {
                if (fh.AllowNull)
                {
                    return true;
                }
                else
                {
                    ErrMsg.Append(_NotAllowNullText).Append(_SeparatorText);
                    return false;
                }
            }
            else
            {
                bool isValid = true;
                Field = Field.Trim();
                if (fh.MaxLength > 0)
                {
                    isValid &= IsValidField(Field, fh.MinLength, fh.MaxLength, !fh.IsUnicode, ErrMsg);
                }
                if (!string.IsNullOrEmpty(fh.Regular))
                {
                    bool iv = Regex.IsMatch(Field, fh.Regular);
                    if (iv)
                    {
                        ErrMsg.Append(_NotMatchedText).Append(_SeparatorText);
                    }
                    isValid &= iv;
                }
                return isValid;
            }
        }

        private bool IsValidField(string Field, int Min, int Max, bool IsAnsi, StringBuilder ErrMsg)
        {
            int i = IsAnsi ? StringHelper.GetAnsiLength(Field) : Field.Length;

            if (i < Min || i > Max)
            {
                ErrMsg.Append(string.Format(_LengthText, Min, Max, i)).Append(_SeparatorText);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
