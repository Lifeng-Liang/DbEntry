using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lephone.Core;
using Lephone.Core.Text;
using Lephone.Data.Definition;
using Lephone.Data.Model.Member;

namespace Lephone.Data
{
    public class ValidateHandler
    {
        private readonly string _invalidFieldText;
        private readonly string _notAllowNullText;
        private readonly string _notMatchedText;
        private readonly string _lengthText;
        private readonly string _shouldBeUniqueText;
        private readonly string _separatorText;

        private readonly int _separatorTextLength;

        private readonly bool _emptyAsNull;
        private readonly bool _includeClassName;
        public bool IsValid;

        private readonly Dictionary<string, string> _errorMessages;

        public Dictionary<string, string> ErrorMessages
        {
            get { return _errorMessages; }
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
            _emptyAsNull = emptyAsNull;
            _includeClassName = includeClassName;

            _invalidFieldText = invalidFieldText;
            _notAllowNullText = notAllowNullText;
            _notMatchedText = notMatchedText;
            _lengthText = lengthText;
            _shouldBeUniqueText = shouldBeUniqueText;
            _separatorText = separatorText;

            _separatorTextLength = _separatorText.Length;

            _errorMessages = new Dictionary<string, string>();
        }

        public bool ValidateObject(object obj)
        {
            IsValid = true;
            _errorMessages.Clear();

            var t = obj.GetType();
            var ctx = ModelContext.GetInstance(t);
            string tn = ctx.Info.HandleType.Name;
            bool isNew = false;
            if (ctx.Info.KeyMembers.Length > 0)
            {
                isNew = ctx.IsNewObject(obj);
            }

            ValidateCommon(obj, ctx, tn);
            ValidateUnique(obj, ctx, isNew);
            return IsValid;
        }

        private void ValidateCommon(object obj, ModelContext ctx, string tableName)
        {
            var stringType = typeof(string);
            var byteArrayType = typeof(byte[]);
            foreach (MemberHandler fh in ctx.Info.Members)
            {
                var realType = fh.Is.LazyLoad ? fh.FieldType.GetGenericArguments()[0] : fh.FieldType;
                if (realType == stringType)
                {
                    var errMsg = new StringBuilder();
                    string field = fh.Is.LazyLoad ? ((LazyLoadField<string>)fh.GetValue(obj)).Value : (string)fh.GetValue(obj);
                    var isValid = ValidateStringField(field, fh, errMsg);
                    SetErrorMessage(fh, errMsg, isValid, tableName);
                }
                else if (realType == byteArrayType)
                {
                    var errMsg = new StringBuilder();
                    byte[] field = fh.Is.LazyLoad ? ((LazyLoadField<byte[]>)fh.GetValue(obj)).Value : (byte[])fh.GetValue(obj);
                    var isValid = ValidateByteArrayField(field, fh, errMsg);
                    SetErrorMessage(fh, errMsg, isValid, tableName);
                }
            }
        }

        private void SetErrorMessage(MemberHandler fh, StringBuilder errMsg, bool isValid, string tableName)
        {
            if (errMsg.Length > _separatorTextLength) { errMsg.Length -= _separatorTextLength; }
            if (!isValid)
            {
                string n = (_includeClassName ? tableName + "." + fh.ShowString : fh.ShowString);
                _errorMessages[fh.Name] = string.Format(_invalidFieldText, n, errMsg);
            }
            IsValid &= isValid;
        }

        private void ValidateUnique(object obj, ModelContext ctx, bool isNew)
        {
            Dictionary<string, object> updatedColumns = null;
            if(obj is DbObjectSmartUpdate)
            {
                updatedColumns = ((DbObjectSmartUpdate)obj).m_UpdateColumns;
            }

            Condition editCondition = isNew ? null : !ModelContext.GetKeyWhereClause(obj);
            foreach (List<MemberHandler> mhs in ctx.Info.UniqueIndexes.Values)
            {
                Condition c = null;
                string n = "";
                var sn = new StringBuilder();
                foreach (MemberHandler h in mhs)
                {
                    if(updatedColumns == null || updatedColumns.ContainsKey(h.Name))
                    {
                        object v = h.GetValue(obj);
                        if (h.Is.AllowNull && v == null)
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
                        sn.Append(h.ShowString).Append(_separatorText);
                    }
                }
                if (c != null)
                {
                    if (ctx.Operator.GetResultCountAvoidSoftDelete(c && editCondition, false) != 0)
                    {
                        sn.Length -= _separatorTextLength;
                        IsValid = false;
                        string uniqueErrMsg = string.IsNullOrEmpty(mhs[0].UniqueErrorMessage)
                                                  ? _shouldBeUniqueText
                                                  : mhs[0].UniqueErrorMessage;
                        _errorMessages[n] = _errorMessages.ContainsKey(n)
                                  ? string.Format("{0}{1}{2}", _errorMessages[n], _separatorText, uniqueErrMsg)
                                  : string.Format(_invalidFieldText, sn, uniqueErrMsg);
                    }
                }
            }
        }

        private bool ValidateByteArrayField(byte[] field, MemberHandler fh, StringBuilder errMsg)
        {
            if (field == null)
            {
                if (fh.Is.AllowNull)
                {
                    return true;
                }
                errMsg.Append(_notAllowNullText).Append(_separatorText);
                return false;
            }
            bool isValid = true;
            if(fh.MaxLength > 0)
            {
                string errorText = string.IsNullOrEmpty(fh.LengthErrorMessage)
                                          ? _lengthText
                                          : fh.LengthErrorMessage;

                isValid &= IsValidField(field.Length, fh.MinLength, fh.MaxLength, errMsg, errorText);
            }
            return isValid;
        }

        private bool ValidateStringField(string field, MemberHandler fh, StringBuilder errMsg)
        {
            if (field == null || (field == "" && _emptyAsNull))
            {
                if (fh.Is.AllowNull)
                {
                    return true;
                }
                errMsg.Append(_notAllowNullText).Append(_separatorText);
                return false;
            }
            bool isValid = true;
            field = field.Trim();
            if (fh.MaxLength > 0)
            {
                isValid &= IsValidStringField(field, fh.MinLength, fh.MaxLength, !fh.Is.Unicode,
                    string.IsNullOrEmpty(fh.LengthErrorMessage) ? _lengthText : fh.LengthErrorMessage, errMsg);
            }
            if (!string.IsNullOrEmpty(fh.Regular))
            {
                bool iv = Regex.IsMatch(field, fh.Regular);
                if (!iv)
                {
                    if (string.IsNullOrEmpty(fh.RegularErrorMessage))
                    {
                        errMsg.Append(_notMatchedText).Append(_separatorText);
                    }
                    else
                    {
                        errMsg.Append(fh.RegularErrorMessage).Append(_separatorText);
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
                errMsg.Append(string.Format(errorText, min, max, length)).Append(_separatorText);
                return false;
            }
            return true;
        }
    }
}
