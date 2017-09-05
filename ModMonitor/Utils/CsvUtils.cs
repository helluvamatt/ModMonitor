using System;
using System.Linq;
using System.Reflection;

namespace ModMonitor.Utils
{
    static class CsvUtils
    {
        public static string GetCsvHeader(Type t)
        {
            return string.Join(",", t.GetProperties().Select(pi => GetCsvHeader(pi)));
        }

        public static string GetCsvHeader(PropertyInfo pi)
        {
            var attr = pi.PropertyType.GetCustomAttribute<CsvHeaderAttribute>();
            if (attr != null)
            {
                return string.Join(",", attr.Suffixes.Select(s => pi.Name + s));
            }
            return pi.Name;
        }

        public static string GetCsv(object obj)
        {
            var props = obj.GetType().GetProperties();
            return string.Join(",", props.Select(pi => GetCsvValue(pi.GetValue(obj))));
        }

        public static string GetCsvValue(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            if (obj is CsvValue)
            {
                return string.Join(",", ((CsvValue)obj).GetCsvValues());
            }
            return obj.ToString();
        }
    }

    interface CsvValue
    {
        string[] GetCsvValues();
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class CsvHeaderAttribute : Attribute
    {
        readonly string[] _suffixes;

        public CsvHeaderAttribute(params string[] suffixes)
        {
            _suffixes = suffixes;
        }

        public string[] Suffixes
        {
            get { return _suffixes; }
        }
    }
}
