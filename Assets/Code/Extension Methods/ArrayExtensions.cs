using System;

public static class ArrayExtensions
{
    public static int FindIndex<T>(this T[] array, T item)
    {
        return Array.IndexOf(array, item);
    }
}
