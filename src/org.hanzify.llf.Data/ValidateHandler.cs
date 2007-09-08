
#region usings

using System;
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

        public string InvalidFieldText
        {
            get { return _InvalidFieldText; }
        }

        private bool EmptyAsNull;
        private bool IncludeClassName;
        private int NotNullFieldMinSize;
        public bool IsValid;

        private Dictionary<string, string> _ErrorMessages;

        public Dictionary<string, string> ErrorMessages
        {
            get { return _ErrorMessages; }
        }

        public ValidateHandler()
            : this(false, 0)
        {
        }

        public ValidateHandler(bool EmptyAsNull, int NotNullFieldMinSize)
            : this(EmptyAsNull, NotNullFieldMinSize, false, "Invalid Field")
        {
        }

        public ValidateHandler(bool EmptyAsNull, int NotNullFieldMinSize, bool IncludeClassName, string InvalidFieldText)
        {
            this.IsValid = true;
            this.EmptyAsNull = EmptyAsNull;
            this.NotNullFieldMinSize = NotNullFieldMinSize;
            this.IncludeClassName = IncludeClassName;
            this._InvalidFieldText = InvalidFieldText;
            _ErrorMessages = new Dictionary<string, string>();
        }

        public bool ValidateObject(object obj)
        {
            this.IsValid = true;
            this._ErrorMessages.Clear();

            Type t = obj.GetType();
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(t);
            
            Type StringType = typeof(string);
            foreach (MemberHandler fh in oi.Fields)
            {
                if (fh.FieldType == StringType || (fh.IsLazyLoad && fh.FieldType.GetGenericArguments()[0] == StringType))
                {
                    string Field = fh.IsLazyLoad ? ((LazyLoadField<string>)fh.GetValue(obj)).Value : (string)fh.GetValue(obj);
                    bool isValid = ValidateField(Field, fh);
                    if (!isValid)
                    {
                        string n = (IncludeClassName ? t.Name + "." + fh.Name : fh.Name);
                        _ErrorMessages[n] = _InvalidFieldText;
                    }
                    this.IsValid &= isValid;
                }
            }
            return this.IsValid;
        }

        private bool ValidateField(string Field, MemberHandler fh)
        {
            if (Field == null)
            {
                return fh.AllowNull;
            }
            else
            {
                if (Field == "" && EmptyAsNull)
                {
                    if (NotNullFieldMinSize > 0)
                    {
                        return fh.AllowNull;
                    }
                    else
                    {
                        return true;
                    }
                }
                bool isValid = true;
                Field = Field.Trim();
                if (fh.MaxLength > 0)
                {
                    isValid &= IsValidField(Field, NotNullFieldMinSize, fh.MaxLength, !fh.IsUnicode);
                }
                if (!string.IsNullOrEmpty(fh.Regular))
                {
                    isValid &= Regex.IsMatch(Field, fh.Regular);
                }
                return isValid;
            }
        }

        private bool IsValidField(string Field, int Min, int Max, bool IsAnsi)
        {
            int i = IsAnsi ? StringHelper.GetAnsiLength(Field) : Field.Length;

            if ((i < Min) || (i > Max))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
