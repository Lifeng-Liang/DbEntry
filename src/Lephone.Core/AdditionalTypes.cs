namespace System
{
    [Serializable]
    public struct Date : IComparable, IFormattable, IConvertible, IComparable<Date>, IEquatable<Date>
    {
        #region operators

        public static bool operator >(Date d1, Date d2)
        {
            return d1.CompareTo(d2) > 0;
        }

        public static bool operator <(Date d1, Date d2)
        {
            return d1.CompareTo(d2) < 0;
        }

        public static bool operator >=(Date d1, Date d2)
        {
            return d1.CompareTo(d2) >= 0;
        }

        public static bool operator <=(Date d1, Date d2)
        {
            return d1.CompareTo(d2) <= 0;
        }

        public static bool operator ==(Date d1, Date d2)
        {
            return d1.CompareTo(d2) == 0;
        }

        public static bool operator !=(Date d1, Date d2)
        {
            return d1.CompareTo(d2) != 0;
        }

        public static TimeSpan operator -(Date d1, Date d2)
        {
            return d1._dt.Date - d2._dt.Date;
        }

        #endregion

        #region common

        public static Date Now
        {
            get { return new Date(DateTime.Now); }
        }

        public static Date MinValue = new Date(DateTime.MinValue);
        public static Date MaxValue = new Date(DateTime.MaxValue);

        private DateTime _dt;

        public int Year
        {
            get { return _dt.Year; }
        }
        public int Month
        {
            get { return _dt.Month; }
        }
        public int Day
        {
            get { return _dt.Day; }
        }

        public Date AddYears(int years)
        {
            return new Date(_dt.AddYears(years));
        }

        public Date AddMonths(int months)
        {
            return new Date(_dt.AddMonths(months));
        }

        public Date AddDays(double value)
        {
            return new Date(_dt.AddDays(value));
        }

        public DayOfWeek DayOfWeek
        {
            get { return _dt.DayOfWeek; }
        }

        public int DayOfYear
        {
            get { return _dt.DayOfYear; }
        }

        public Date(int year, int month, int day)
        {
            _dt = new DateTime(year, month, day);
        }

        public Date(DateTime dt)
        {
            this._dt = dt;
        }

        public static bool TryParse(string s, out Date result)
        {
            DateTime dt;
            bool could = DateTime.TryParse(s, out dt);
            if (could)
            {
                if (dt == dt.Date)
                {
                    result = new Date(dt);
                    return true;
                }
            }
            result = new Date();
            return false;
        }

        public static Date Parse(string value)
        {
            DateTime t = DateTime.Parse(value);
            if (t == t.Date)
            {
                return new Date(t);
            }
            throw new FormatException("Only date fields are supported.");
        }

        public string ToString(string format)
        {
            return _dt.ToString(format);
        }

        public override bool Equals(object obj)
        {
            return _dt.Date.Equals(((Date)obj)._dt.Date);
        }

        public override int GetHashCode()
        {
            return _dt.Date.GetHashCode();
        }

        public override string ToString()
        {
            return _dt.ToString("yyyy-MM-dd");
        }

        public static explicit operator Date(string value)
        {
            return Parse(value);
        }

        public static explicit operator Date(DateTime dt)
        {
            return new Date(dt);
        }

        public static explicit operator DateTime(Date d)
        {
            return d._dt;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            DateTime tdt = ((Date)obj)._dt;
            return _dt.Date.CompareTo(tdt.Date);
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _dt.Date.ToString(format, formatProvider);
        }

        #endregion

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToByte(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToDouble(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToInt64(provider);
        }

        [CLSCompliant(false)]
        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToSByte(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToSingle(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToType(conversionType, provider);
        }

        [CLSCompliant(false)]
        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToUInt16(provider);
        }

        [CLSCompliant(false)]
        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToUInt32(provider);
        }

        [CLSCompliant(false)]
        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)(_dt.Date)).ToUInt64(provider);
        }

        #endregion

        #region IComparable<Date> Members

        public int CompareTo(Date other)
        {
            return _dt.Date.CompareTo(other._dt.Date);
        }

        #endregion

        #region IEquatable<Date> Members

        public bool Equals(Date other)
        {
            return _dt.Date.Equals(other._dt.Date);
        }

        #endregion
    }

    [Serializable]
    public struct Time : IComparable, IFormattable, IConvertible, IComparable<Time>, IEquatable<Time>
    {
        #region operators

        public static bool operator >(Time t1, Time t2)
        {
            return t1.CompareTo(t2) > 0;
        }

        public static bool operator <(Time t1, Time t2)
        {
            return t1.CompareTo(t2) < 0;
        }

        public static bool operator >=(Time t1, Time t2)
        {
            return t1.CompareTo(t2) >= 0;
        }

        public static bool operator <=(Time t1, Time t2)
        {
            return t1.CompareTo(t2) <= 0;
        }

        public static bool operator ==(Time t1, Time t2)
        {
            return t1.CompareTo(t2) == 0;
        }

        public static bool operator !=(Time t1, Time t2)
        {
            return t1.CompareTo(t2) != 0;
        }

        public static TimeSpan operator -(Time t1, Time t2)
        {
            return t1._dt.TimeOfDay - t2._dt.TimeOfDay;
        }

        #endregion

        #region common

        public static Time Now
        {
            get { return new Time(DateTime.Now); }
        }

        private DateTime _dt;

        public Time AddHours(double value)
        {
            return new Time(_dt.AddHours(value));
        }

        public Time AddMinutes(double value)
        {
            return new Time(_dt.AddMinutes(value));
        }

        public Time AddSecond(double value)
        {
            return new Time(_dt.AddSeconds(value));
        }

        public Time AddMillisecond(double value)
        {
            return new Time(_dt.AddMilliseconds(value));
        }

        public Time Add(TimeSpan value)
        {
            return new Time(_dt.Add(value));
        }

        public Time AddTicks(long value)
        {
            return new Time(_dt.AddTicks(value));
        }

        public int Hour
        {
            get { return _dt.Hour; }
        }

        public int Minute
        {
            get { return _dt.Minute; }
        }

        public int Second
        {
            get { return _dt.Second; }
        }

        public Time(DateTime dt)
        {
            this._dt = dt;
        }

        public Time(int second)
        {
            _dt = DateTime.MinValue.AddSeconds(second);
        }

        public Time(int hour, int minute, int second)
        {
            _dt = new DateTime(1, 1, 1, hour, minute, second);
        }

        public Time(int hour, int minute, int second, int millisecond)
        {
            _dt = new DateTime(1, 1, 1, hour, minute, second, millisecond);
        }

        public static bool TryParse(string s, out Time result)
        {
            DateTime t;
            var t1 = new DateTime();
            if (DateTime.TryParse("0001-1-1 " + s, out t))
            {
                if (t.Date == t1.Date)
                {
                    result = new Time(t);
                    return true;
                }
            }
            result = new Time();
            return false;
        }

        public static Time Parse(string value)
        {
            var t = DateTime.Parse("0001-1-1 " + value);
            var t1 = new DateTime();
            if (t.Date == t1.Date)
            {
                return new Time(t);
            }
            throw new FormatException("Only time fields are supported.");
        }

        public string ToString(string format)
        {
            return _dt.ToString(format);
        }

        public override bool Equals(object obj)
        {
            return _dt.TimeOfDay.Equals(((Time)obj)._dt.TimeOfDay);
        }

        public override int GetHashCode()
        {
            return _dt.TimeOfDay.GetHashCode();
        }

        public override string ToString()
        {
            return _dt.ToString("HH:mm:ss");
        }

        public static explicit operator Time(string value)
        {
            return Parse(value);
        }

        public static explicit operator Time(DateTime dt)
        {
            return new Time(dt);
        }

        public static explicit operator DateTime(Time t)
        {
            return t._dt;
        }

        public static explicit operator TimeSpan(Time t)
        {
            return t._dt.TimeOfDay;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return _dt.TimeOfDay.CompareTo(((Time)obj)._dt.TimeOfDay);
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _dt.ToString(format, formatProvider);
        }

        #endregion

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        private IConvertible GetConvertible()
        {
            var t = _dt.TimeOfDay;
            var r = new DateTime(1900, 1, 1, t.Hours, t.Minutes, t.Seconds, t.Milliseconds); // It's a little stupid
            return r;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToByte(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToDouble(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToInt64(provider);
        }

        [CLSCompliant(false)]
        public sbyte ToSByte(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToSByte(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToSingle(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToType(conversionType, provider);
        }

        [CLSCompliant(false)]
        public ushort ToUInt16(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToUInt16(provider);
        }

        [CLSCompliant(false)]
        public uint ToUInt32(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToUInt32(provider);
        }

        [CLSCompliant(false)]
        public ulong ToUInt64(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToUInt64(provider);
        }

        #endregion

        #region IComparable<Time> Members

        public int CompareTo(Time other)
        {
            return _dt.TimeOfDay.CompareTo(other._dt.TimeOfDay);
        }

        #endregion

        #region IEquatable<Time> Members

        public bool Equals(Time other)
        {
            return _dt.TimeOfDay.Equals(other._dt.TimeOfDay);
        }

        #endregion
    }
}

namespace System.Collections.Generic
{
    using System;
    using Xml.Serialization;
    using Runtime.Serialization;

    [XmlRoot("dictionary"), Serializable]
    public class XDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region ctor

        public XDictionary()
        {
        }

        public XDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }


        public XDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }


        public XDictionary(int capacity)
            : base(capacity)
        {
        }

        public XDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        protected XDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region IXmlSerializable Members

        public Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(Xml.XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty) { return; }

            while (reader.NodeType != Xml.XmlNodeType.EndElement)
            {
                string key = reader.GetAttribute("key");
                string value = reader.GetAttribute("value");
                this.Add((TKey)Convert.ChangeType(key, typeof(TKey)),
                    (TValue)Convert.ChangeType(value, typeof(TValue)));
                reader.Read();
                reader.MoveToContent();

            }
            reader.ReadEndElement();
        }

        public void WriteXml(Xml.XmlWriter writer)
        {
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteAttributeString("key", key.ToString());
                writer.WriteAttributeString("value", this[key].ToString());
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
