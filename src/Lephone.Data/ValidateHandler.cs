using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lephone.Core;
using Lephone.Core.Text;
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

        public ValidateHandler(bool emptyAsNull)
            : this(emptyAsNull, false, "Invalid Field {0} {1}.", "Not Allow Null", "Not Matched", "The length should be {0} to {1} but was {2}", "Should be UNIQUED", ", ")
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
            string tn = oi.HandleType.Name;
            bool isNew = false;
            if (oi.KeyFields.Length > 0)
            {
                isNew = oi.IsNewObject(obj);
            }

            ValidateCommon(obj, oi, tn);
            ValidateUnique(obj, t, oi, isNew);
            return IsValid;
        }

        private void ValidateCommon(object obj, ObjectInfo oi, string tableName)
        {
            var stringType = typeof(string);
            var byteArrayType = typeof(byte[]);
            foreach (MemberHandler fh in oi.Fields)
            {
                var realType = fh.IsLazyLoad ? fh.FieldType.GetGenericArguments()[0] : fh.FieldType;
                if (realType == stringType)
                {
                    var errMsg = new StringBuilder();
                    string field = fh.IsLazyLoad ? ((LazyLoadField<string>)fh.GetValue(obj)).Value : (string)fh.GetValue(obj);
                    var isValid = ValidateStringField(field, fh, errMsg);
                    SetErrorMessage(fh, errMsg, isValid, tableName);
                }
                else if (realType == byteArrayType)
                {
                    var errMsg = new StringBuilder();
                    byte[] field = fh.IsLazyLoad ? ((LazyLoadField<byte[]>)fh.GetValue(obj)).Value : (byte[])fh.GetValue(obj);
                    var isValid = ValidateByteArrayField(field, fh, errMsg);
                    SetErrorMessage(fh, errMsg, isValid, tableName);
                }
            }
        }

        private void SetErrorMessage(MemberHandler fh, StringBuilder errMsg, bool isValid, string tableName)
        {
            if (errMsg.Length > _SeparatorTextLength) { errMsg.Length -= _SeparatorTextLength; }
            if (!isValid)
            {
                string n = (IncludeClassName ? tableName + "." + fh.ShowString : fh.ShowString);
                _ErrorMessages[fh.Name] = string.Format(_InvalidFieldText, n, errMsg);
            }
            IsValid &= isValid;
        }

        private void ValidateUnique(object obj, Type t, ObjectInfo oi, bool isNew)
        {
            Dictionary<string, object> updatedColumns = null;
            if(obj is DbObjectSmartUpdate)
            {
                updatedColumns = ((DbObjectSmartUpdate)obj).m_UpdateColumns;
            }

            Condition editCondition = isNew ? null : !ObjectInfo.GetKeyWhereClause(obj);
            foreach (List<MemberHandler> mhs in oi.UniqueIndexes.Values)
            {
                Condition c = null;
                string n = "";
                var sn = new StringBuilder();
                foreach (MemberHandler h in mhs)
                {
                    if(updatedColumns == null || updatedColumns.ContainsKey(h.Name))
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
                        sn.Append(h.ShowString).Append(_SeparatorText);
                    }
                }
                if (c != null)
                {
                    if (oi.Context.GetResultCountAvoidSoftDelete(t, c && editCondition, false) != 0)
                    {
                        sn.Length -= _SeparatorTextLength;
                        IsValid = false;
                        string uniqueErrMsg = string.IsNullOrEmpty(mhs[0].UniqueErrorMessage)
                                                  ? _ShouldBeUniqueText
                                                  : mhs[0].UniqueErrorMessage;
                        _ErrorMessages[n] = _ErrorMessages.ContainsKey(n)
                                  ? string.Format("{0}{1}{2}", _ErrorMessages[n], _SeparatorText, uniqueErrMsg)
                                  : string.Format(_InvalidFieldText, sn, uniqueErrMsg);
                    }
                }
            }
        }

        private bool ValidateByteArrayField(byte[] field, MemberHandler fh, StringBuilder errMsg)
        {
            if (field == null)
            {
                if (fh.AllowNull)
                {
                    return true;
                }
                errMsg.Append(_NotAllowNullText).Append(_SeparatorText);
                return false;
            }
            bool isValid = true;
            if(fh.MaxLength > 0)
            {
                string errorText = string.IsNullOrEmpty(fh.LengthErrorMessage)
                                          ? _LengthText
                                          : fh.LengthErrorMessage;

                isValid &= IsValidField(field.Length, fh.MinLength, fh.MaxLength, errMsg, errorText);
            }
            return isValid;
        }

        private bool ValidateStringField(string field, MemberHandler fh, StringBuilder errMsg)
        {
            if (field == null || (field == "" && EmptyAsNull))
            {
                if (fh.AllowNull)
                {
                    return true;
                }
                errMsg.Append(_NotAllowNullText).Append(_SeparatorText);
                return false;
            }
            bool isValid = true;
            field = field.Trim();
            if (fh.MaxLength > 0)
            {
                isValid &= IsValidStringField(field, fh.MinLength, fh.MaxLength, !fh.IsUnicode,
                    string.IsNullOrEmpty(fh.LengthErrorMessage) ? _LengthText : fh.LengthErrorMessage, errMsg);
            }
            if (!string.IsNullOrEmpty(fh.Regular))
            {
                bool iv = Regex.IsMatch(field, fh.Regular);
                if (!iv)
                {
                    if (string.IsNullOrEmpty(fh.RegularErrorMessage))
                    {
                        errMsg.Append(_NotMatchedText).Append(_SeparatorText);
                    }
                    else
                    {
                        errMsg.Append(fh.RegularErrorMessage).Append(_SeparatorText);
                    }
                }
                isValid &= iv;
            }
            return isValid;
        }

        private bool IsValidStringField(string field, int min, int max, bool isAnsi, string errorText, StringBuilder errMsg)
        {
            int length = isAnsi ? StringHelper.GetAnsiLength(field) : field.Length;
            return IsValidField(length, min, max, errMsg, errorText);
        }

        private bool IsValidField(int length, int min, int max, StringBuilder errMsg, string errorText)
        {
            if (length < min || length > max)
            {
                errMsg.Append(string.Format(errorText, min, max, length)).Append(_SeparatorText);
                return false;
            }
            return true;
        }
    }
}
