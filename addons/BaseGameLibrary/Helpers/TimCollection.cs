using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Helpers;

public static class TimExtensions_Collections
{
    public static T? _FindByPredicate<[MustBeVariant] T>(this Array<T> p_array, Predicate<T> p_predicate)
    {
        foreach (T sPars in p_array)
            if (p_predicate(sPars))
                return sPars;

        return default;
    }

    public static void _ForEach<[MustBeVariant] T>(this Array<T> p_array, Action<T> p_predicate)
    {
        foreach (T sPars in p_array)
            p_predicate(sPars);
    }

    public static bool _IsEmpty<[MustBeVariant] T>(this Array<T> p_array) => p_array.Count == 0;
    public static bool _IsEmpty<[MustBeVariant] T>(this HashSet<T> p_array) => p_array.Count == 0;
}