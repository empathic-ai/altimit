using System.Collections.Generic;

public static class ListExtensions
{
    public static bool TryAdd<T>(this List<T> list, T element)
    {
        if (list.Contains(element))
            return false;
        list.Add(element);
        return true;
    }

    public static bool TryRemove<T>(this List<T> list, T element)
    {
        if (!list.Contains(element))
            return false;
        list.Remove(element);
        return true;
    }
}
