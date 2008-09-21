using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lephone.Util;
using Lephone.Util.Text;
using Lephone.Data.Common;
using Lephone.Data.Definition;

namespace Lephone.Data
{
    public class ValidateHandler
    {
        private readonly string _InvalidFieldText;
        private readonly string _NotAllowNullText;
        private readonly string _NotMatchedText;
        private readonly string _LengthText;
        private readonly string _ShouldBeUniqueText;
        private readonly string _SeparatorText;

        private readonly int _SeparatorTextLength;

        private readonly bool EmptyAsNull;
        private readonly bool IncludeClassName;
        public bool IsValid;

        private readonly Dictionary<string, string> _ErrorMessages;

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

        public ValidateHandler(bool emptyAsNull, bool includeClassName, string invalidFieldText, string notAllowNullText, string notMatchedText, string lengthText, string shouldBeUniqueText, string separatorText)
        {
            IsValid = true;
            EmptyAsNull = emptyAsNull;
            IncludeClassName = includeClassName;

            _InvalidFieldText = invalidFieldText;
            _NotAllowNullText = notAllowNullText;
            _NotMatchedText = notMatchedText;
            _LengthText = lengthText;
            _ShouldBeUniqueText = shouldBeUniqueText;
            _SeparatorText = separatorText;

            _SeparatorTextLength = _SeparatorText.Length;

            _ErrorMessages = new Dictionary<string, string>();
        }

        public bool ValidateObject(object obj)
        {
            IsValid = true;
            _ErrorMessages.Clear();

            var t = obj.GetType();
            var oi = ObjectInfo.GetInstance(t);
            string tn = oi.BaseType.Name;
            bool IsNew = false;
            if (oi.KeyFields.Length > 0)
            {
                IsNew = oi.IsNewObject(obj);
            }

            validateCommon(obj, oi, tn);
            validateUnique(obj, t, oi, IsNew);
            return IsValid;
        }

        private void validateCommon(object obj, ObjectInfo oi, string tableName)
        {
            var StringType = typeof(string);
            var ByteArrayType = typeof(byte[]);
            foreach (MemberHandler fh in oi.Fields)
            {
                var realType = fh.IsLazyLoad ? fh.FieldType.GetGenericArguments()[0] : fh.FieldType;
                if (realType == StringType)
                {
                    var ErrMsg = new StringBuilder();
                    string Field = fh.IsLazyLoad ? ((LazyLoadField<string>)fh.GetValue(obj)).Value : (string)fh.GetValue(obj);
                    var isValid = validateStringField(Field, fh, ErrMsg);
                    setErrorMessage(fh, ErrMsg, isValid, tableName);
                }
                else if (realType == ByteArrayType)
                {
                    var ErrMsg = new StringBuilder();
                    byte[] Field = fh.IsLazyLoad ? ((LazyLoadField<byte[]>)fh.GetValue(obj)).Value : (byte[])fh.GetValue(obj);
                    var isValid = validateByteArrayField(Field, fh, ErrMsg);
                    setErrorMessage(fh, ErrMsg, isValid, tableName);
                }
            }
        }

        private void setErrorMessage(MemberHandler fh, StringBuilder ErrMsg, bool isValid, string tableName)
        {
            if (ErrMsg.Length > _SeparatorTextLength) { ErrMsg.Length -= _SeparatorTextLength; }
            if (!isValid)
            {
                string n = (IncludeClassName ? tableName + "." + fh.Name : fh.Name);
                _ErrorMessages[n] = string.Format(_InvalidFieldText, n, ErrMsg);
            }
            IsValid &= isValid;
        }

        private void validateUnique(object obj, Type t, ObjectInfo oi, bool IsNew)
        {
            WhereCondition EditCondition = IsNew ? null : !ObjectInfo.GetKeyWhereClause(obj);
            foreach (List<MemberHandler> mhs in oi.UniqueIndexes.Values)
            {
                WhereCondition c = null;
                string n = "";
                foreach (MemberHandler h in mhs)
                {
                    object v = h.GetValue(obj);
                    if (h.AllowNull && v == null)
                    {
                        c = null;
                        break;
                    }
                    if (v != null)
                    {
                        if (v.GetType().IsGenericType)
                        {
                            if (v is IBelongsTo)
                            {
                                v = ((IBelongsTo)v).ForeignKey;
                            }
                            else
                            {
                                v = v.GetType().GetField("m_Value", ClassHelper.AllFlag).GetValue(v);
                            }
                        }
                    }
                    c &= (CK.K[h.Name] == v);
                    n += h.Name;
                }
                if (c != null)
                {
                    if (DbEntry.Context.GetResultCount(t, c && EditCondition) != 0)
                    {
                        IsValid = false;
                        string uniqueErrMsg = string.IsNullOrEmpty(mhs[0].UniqueErrorMessage)
                                                  ? _ShouldBeUniqueText
                                                  : mhs[0].UniqueErrorMessage;
                        _ErrorMessages[n] = _ErrorMessages.ContainsKey(n)
                                  ? string.Format("{0}{1}{2}", _ErrorMessages[n], _SeparatorText, uniqueErrMsg)
                                  : string.Format(_InvalidFieldText, n, uniqueErrMsg);
                    }
                }
            }
        }

        private bool validateByteArrayField(byte[] Field, MemberHandler fh, StringBuilder ErrMsg)
        {
            if (Field == null)
            {
                if (fh.AllowNull)
                {
                    return true;
                }
                ErrMsg.Append(_NotAllowNullText).Append(_SeparatorText);
                return false;
            }
            bool isValid = true;
            if(fh.MaxLength > 0)
            {
                string errorText = string.IsNullOrEmpty(fh.LengthErrorMessage)
                                          ? _LengthText
                                          : fh.LengthErrorMessage;

                isValid &= isValidField(Field.Length, fh.MinLength, fh.MaxLength, ErrMsg, errorText);
            }
            return isValid;
        }

        private bool validateStringField(string Field, MemberHandler fh, StringBuilder ErrMsg)
        {
            if (Field == null || (Field == "" && EmptyAsNull))
            {
                if (fh.AllowNull)
                {
                    return true;
                }
                ErrMsg.Append(_NotAllowNullText).Append(_SeparatorText);
                return false;
            }
            bool isValid = true;
            Field = Field.Trim();
            if (fh.MaxLength > 0)
            {
                isValid &= isValidStringField(Field, fh.MinLength, fh.MaxLength, !fh.IsUnicode,
                    string.IsNullOrEmpty(fh.LengthErrorMessage) ? _LengthText : fh.LengthErrorMessage, ErrMsg);
            }
            if (!string.IsNullOrEmpty(fh.Regular))
            {
                bool iv = Regex.IsMatch(Field, fh.Regular);
                if (!iv)
                {
                    if (string.IsNullOrEmpty(fh.RegularErrorMessage))
                    {
                        ErrMsg.Append(_NotMatchedText).Append(_SeparatorText);
                    }
                    else
                    {
                        ErrMsg.Append(fh.RegularErrorMessage).Append(_SeparatorText);
                    }
                }
                isValid &= iv;
            }
            return isValid;
        }

        private bool isValidStringField(string Field, int Min, int Max, bool IsAnsi, string errorText, StringBuilder ErrMsg)
        {
            int length = IsAnsi ? StringHelper.GetAnsiLength(Field) : Field.Length;
            return isValidField(length, Min, Max, ErrMsg, errorText);
        }

        private bool isValidField(int length, int Min, int Max, StringBuilder ErrMsg, string errorText)
        {
            if (length < Min || length > Max)
            {
                ErrMsg.Append(string.Format(errorText, Min, Max, length)).Append(_SeparatorText);
                return false;
            }
            return true;
        }
    }
}
