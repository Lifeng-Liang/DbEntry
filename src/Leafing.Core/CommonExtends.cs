using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Leafing.Core;
using Leafing.Core.Setting;
using Leafing.Core.Text;
using System.Text;

public static class CommonExtends
{
	#region FP

	public static IEnumerable<TR> Map<T, TR>(this IEnumerable<T> list, Func<T, TR> cb)
	{
		foreach (T item in list) {
			yield return cb(item);
		}
	}

	public static TR Reduce<T, TR>(this IEnumerable<T> list, Func<TR,T,TR> cb, TR baseValue)
	{
		TR prev = baseValue;
		foreach (T next in list) {
			prev = cb(prev, next);
		}
		return prev;
	}

	public static IEnumerable<T> Filter<T>(this IEnumerable<T> list, Func<T, bool> cb)
	{
		foreach(T item in list){
			if (cb(item)) {
				yield return item;
			}
		}
	}

	public static IEnumerable<T> Each<T>(this IEnumerable<T> list, Action<T> cb)
	{
		foreach (T item in list) {
			cb(item);
		}
		return list;
	}

	public static bool Every<T>(this IEnumerable<T> list, Func<T, bool> cb)
	{
		foreach (T item in list) {
			if (!cb(item)) {
				return false;
			}
		}
		return true;
	}

	public static bool Some<T>(this IEnumerable<T> list, Func<T, bool> cb)
	{
		foreach (T item in list) {
			if(cb(item)) {
				return true;
			}
		}
		return false;
	}

	#endregion

    #region String

    public static bool IsIndentityName(this string s)
    {
        return StringHelper.IsIndentityName(s);
    }

    public static string[] Split(this string s, char c, int count)
    {
        return StringHelper.Split(s, c, count);
    }

    public static string EnsureEndsWith(this string s, char c)
    {
        return StringHelper.EnsureEndsWith(s, c);
    }

    public static string EnsureEndsWith(this string s, string s1)
    {
        return StringHelper.EnsureEndsWith(s, s1);
    }

    public static string MultiLineAddPrefix(this string source)
    {
        return StringHelper.MultiLineAddPrefix(source);
    }

    public static string MultiLineAddPrefix(this string source, string prefix)
    {
        return StringHelper.MultiLineAddPrefix(source, prefix);
    }

    public static string MultiLineAddPrefix(this string source, string prefix, char splitBy)
    {
        return StringHelper.MultiLineAddPrefix(source, prefix, splitBy);
    }

    public static string GetLeft(this string s)
    {
        return StringHelper.GetStringLeft(s);
    }

    public static string GetLeft(this string s, int n)
    {
        return StringHelper.GetStringLeft(s, n);
    }

    public static string ToCString(this string s)
    {
        return StringHelper.GetCString(s);
    }

    public static int GetAnsiLength(this string s)
    {
        return StringHelper.GetAnsiLength(s);
    }

    public static string GetMultiByteSubString(this string s, int count)
    {
        return StringHelper.GetMultiByteSubString(s, count);
    }

    public static string Capitalize(this string s)
    {
        return StringHelper.Capitalize(s);
    }

    #endregion

    #region Misc

    public static byte[] Cut(this byte[] bs, int length)
    {
        return StringHelper.GetBytesByLength(bs, length);
    }

    public static string EnumToString(this Enum o)
    {
        return StringHelper.EnumToString(o);
    }

    public static string ReadToEnd(this Stream s)
    {
        return StringHelper.ReadToEnd(s);
    }

	public static string ReadToEnd(this Stream s, Encoding encoding)
	{
		return StringHelper.ReadToEnd(s, encoding);
	}

    public static string ReadToEnd(this Stream s, long position)
    {
        return StringHelper.ReadToEnd(s, position);
    }

    public static string ReadToEnd(this StreamReader s)
    {
        return StringHelper.ReadToEnd(s);
    }

    public static string ToBase32String(this Guid guid)
    {
        var bs = guid.ToByteArray();
        return Base32StringCoding.Decode(bs);
    }

    public static string FirstItem(this string[] array)
    {
        if (array == null || array.Length == 0)
        {
            return null;
        }
        return array[0];
    }

    public static string LastItem(this string[] array)
    {
        if (array == null || array.Length == 0)
        {
            return null;
        }
        return array[array.Length - 1];
    }

    public static string[] RemoveFirst(this string[] array)
    {
        if (array == null)
        {
            return null;
        }
        if (array.Length == 0)
        {
            return array;
        }
        return new List<string>(array).RemoveFirst().ToArray();
    }

    public static string[] RemoveLast(this string[] array)
    {
        if (array == null)
        {
            return null;
        }
        if (array.Length == 0)
        {
            return array;
        }
        return new List<string>(array).RemoveLast().ToArray();
    }

    public static T FirstItem<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default(T);
        }
        return list[0];
    }

    public static T LastItem<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default(T);
        }
        return list[list.Count - 1];
    }

    public static T LastItem<T>(this T[] list)
    {
        if (list == null || list.Length == 0)
        {
            return default(T);
        }
        return list[list.Length - 1];
    }

    public static List<T> RemoveFirst<T>(this List<T> list)
    {
        if (list == null)
        {
            return null;
        }
        if (list.Count == 0)
        {
            return list;
        }
        list.RemoveAt(0);
        return list;
    }

    public static List<T> RemoveLast<T>(this List<T> list)
    {
        if (list == null)
        {
            return null;
        }
        if (list.Count == 0)
        {
            return list;
        }
        list.RemoveAt(list.Count - 1);
        return list;
    }

    public static bool IsNullOrEmpty(this string s)
    {
        return s == null || s.Trim() == string.Empty;
    }

    public static bool IsNullOrEmpty(this ICollection list)
    {
        return list == null || list.Count <= 0;
    }

    public static void Initialize(this Type type)
    {
        ConfigHelper.AppSettings.InitClass(type);
    }

	public static bool IsNullable(this Type type){
		if (type.IsGenericType) {
			if (type.GetGenericTypeDefinition () == typeof(Nullable<>)) {
				return true;
			}
		}
		return false;
	}

    #endregion
}
