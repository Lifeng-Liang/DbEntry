using System;

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

        #endregion

        #region common

        public static Date Now
        {
            get { return new Date(DateTime.Now); }
        }

        private DateTime dt;

        public int Year
        {
            get { return dt.Year; }
        }
        public int Month
        {
            get { return dt.Month; }
        }
        public int Day
        {
            get { return dt.Day; }
        }

        public Date AddYears(int years)
        {
            return new Date(dt.AddYears(years));
        }

        public Date AddMonths(int months)
        {
            return new Date(dt.AddMonths(months));
        }

        public Date AddDays(double value)
        {
            return new Date(dt.AddDays(value));
        }

        public DayOfWeek DayOfWeek
        {
            get { return dt.DayOfWeek; }
        }

        public int DayOfYear
        {
            get { return dt.DayOfYear; }
        }

        public Date(int Year, int Month, int Day)
        {
            dt = new DateTime(Year, Month, Day);
        }

        public Date(DateTime dt)
        {
            this.dt = dt;
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
            return dt.ToString(format);
        }

        public override bool Equals(object obj)
        {
            return dt.Date.Equals(((Date)obj).dt.Date);
        }

        public override int GetHashCode()
        {
            return dt.Date.GetHashCode();
        }

        public override string ToString()
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static explicit operator Date(string value)
        {
            return Parse(value);
        }

        public static explicit operator Date(DateTime dt)
        {
            return new Date(dt);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            DateTime tdt = ((Date)obj).dt;
            return dt.Date.CompareTo(tdt.Date);
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return dt.Date.ToString(format, formatProvider);
        }

        #endregion

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToByte(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToDouble(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToInt64(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToSByte(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToSingle(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToType(conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToUInt16(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToUInt32(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)(dt.Date)).ToUInt64(provider);
        }

        #endregion

        #region IComparable<Date> Members

        public int CompareTo(Date other)
        {
            return dt.Date.CompareTo(other.dt.Date);
        }

        #endregion

        #region IEquatable<Date> Members

        public bool Equals(Date other)
        {
            return dt.Date.Equals(other.dt.Date);
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

        #endregion

        #region common

        public static Time Now
        {
            get { return new Time(DateTime.Now); }
        }

        private DateTime dt;

        public Time AddHours(double value)
        {
            return new Time(dt.AddHours(value));
        }

        public Time AddMinutes(double value)
        {
            return new Time(dt.AddMinutes(value));
        }

        public Time AddSecond(double value)
        {
            return new Time(dt.AddSeconds(value));
        }

        public Time AddMillisecond(double value)
        {
            return new Time(dt.AddMilliseconds(value));
        }

        public Time Add(TimeSpan value)
        {
            return new Time(dt.Add(value));
        }

        public Time AddTicks(long value)
        {
            return new Time(dt.AddTicks(value));
        }

        public int Hour
        {
            get { return dt.Hour; }
        }

        public int Minute
        {
            get { return dt.Minute; }
        }

        public int Second
        {
            get { return dt.Second; }
        }

        public Time(DateTime dt)
        {
            this.dt = dt;
        }

        public Time(int Second)
        {
            dt = DateTime.MinValue.AddSeconds(Second);
        }

        public Time(int Hour, int Minute, int Second)
        {
            dt = new DateTime(1, 1, 1, Hour, Minute, Second);
        }

        public Time(int Hour, int Minute, int Second, int Millisecond)
        {
            dt = new DateTime(1, 1, 1, Hour, Minute, Second, Millisecond);
        }

        public static bool TryParse(string s, out Time result)
        {
            DateTime t;
            DateTime t1 = new DateTime();
            bool could = DateTime.TryParse("0001-1-1 " + s, out t);
            if (could)
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
            DateTime t = DateTime.Parse("0001-1-1 " + value);
            DateTime t1 = new DateTime();
            if (t.Date == t1.Date)
            {
                return new Time(t);
            }
            throw new FormatException("Only time fields are supported.");
        }

        public string ToString(string format)
        {
            return dt.ToString(format);
        }

        public override bool Equals(object obj)
        {
            return dt.TimeOfDay.Equals(((Time)obj).dt.TimeOfDay);
        }

        public override int GetHashCode()
        {
            return dt.TimeOfDay.GetHashCode();
        }

        public override string ToString()
        {
            return dt.ToString("HH:mm:ss");
        }

        public static explicit operator Time(string value)
        {
            return Parse(value);
        }

        public static explicit operator Time(DateTime dt)
        {
            return new Time(dt);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return dt.TimeOfDay.CompareTo(((Time)obj).dt.TimeOfDay);
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return dt.ToString(format, formatProvider);
        }

        #endregion

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        private IConvertible GetConvertible()
        {
            TimeSpan t = dt.TimeOfDay;
            DateTime r = new DateTime(1900, 1, 1, t.Hours, t.Minutes, t.Seconds, t.Milliseconds); // It's a little stupid
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

        public ushort ToUInt16(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToUInt16(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToUInt32(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            IConvertible o = GetConvertible();
            return o.ToUInt64(provider);
        }

        #endregion

        #region IComparable<Time> Members

        public int CompareTo(Time other)
        {
            return dt.TimeOfDay.CompareTo(other.dt.TimeOfDay);
        }

        #endregion

        #region IEquatable<Time> Members

        public bool Equals(Time other)
        {
            return dt.TimeOfDay.Equals(other.dt.TimeOfDay);
        }

        #endregion
    }
}
