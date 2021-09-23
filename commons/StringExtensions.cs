using System.Collections.Generic;

namespace vault.commons;


public static class StringExtensions
{
    public static string Join<T>(this IEnumerable<T> enumerable, char d) 
        => string.Join(d, enumerable);
    public static string Join<T>(this IEnumerable<T> enumerable, string d) 
        => string.Join(d, enumerable);
}