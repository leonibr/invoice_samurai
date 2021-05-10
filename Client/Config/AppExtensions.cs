using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceSamurai.Client.Config
{
    public static class AppExtensions
    {
        public static bool IsNull<T>(this T value) where T : class
        {
            return (value == null);
        }

        public static bool IsNotNull<T>(this T value) where T : class
        {
            return (!value.IsNull<T>());
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotNullAndNotEmpty(this string value)
        {
            return (!value.IsNullOrEmpty());
        }

        public static bool IsNotNullAndNotEmpty(this ICollection value)
        {
            return (value.IsNotNull<ICollection>() && (value.Count > 0));
        }

        public static bool IsNullOrEmpty(this ICollection value)
        {
            return (!value.IsNotNullAndNotEmpty());
        }
    }


}
