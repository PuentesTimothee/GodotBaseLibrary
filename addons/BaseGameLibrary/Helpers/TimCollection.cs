using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Helpers;

public static class TimExtensions_Collections
{
    /// <summary>
    /// Find the first item with a lambda
    /// </summary>
    /// <param name="p_array">The IEnumerable (Array/List/Whatever I don't care)</param>
    /// <param name="p_predicate">The lambda</param>
    /// <typeparam name="TType">Type of the Enumerable, probably deduced don't worry</typeparam>
    /// <returns>The found Item else defaults if not found</returns>
    public static TType? _FindByPredicate<[MustBeVariant] TType>(this IEnumerable<TType> p_array, Predicate<TType> p_predicate)
    {
        foreach (TType sPars in p_array)
            if (p_predicate(sPars))
                return sPars;

        return default;
    }

    /// <summary>
    /// Call a Action on all members of a IEnumerable
    /// </summary>
    /// <param name="p_array">You can read guess what it is</param>
    /// <param name="p_action">Probable the Holy grail, or just the action to performs, I dunno</param>
    /// <typeparam name="TType">Type of the Enumerable, probably deduced don't worry</typeparam>
    public static void _ForEach<[MustBeVariant] TType>(this IEnumerable<TType> p_array, Action<TType> p_action)
    {
        foreach (TType sPars in p_array)
            p_action(sPars);
    }

    /// <summary>
    /// If you have to read this to guess what It is, seek help or a preschooler.
    /// </summary>
    public static bool _IsEmpty<[MustBeVariant] T>(this Array<T> p_array) => p_array.Count == 0;
 
    /// <summary>
    /// If you have to read this to guess what It is, seek help or a preschooler.
    /// </summary>
    public static bool _IsEmpty<[MustBeVariant] T>(this HashSet<T> p_array) => p_array.Count == 0;
}