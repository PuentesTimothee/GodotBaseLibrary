using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ElementGodot.BaseGameLibrary.Helpers;

public class ReflectionHelper
{
    public static IEnumerable<T> GetEnumerableOfType<T>(params object[] p_constructorArgs) where T : class
    {
        List<T> objects = new List<T>();
        foreach (Type type in
                 Assembly.GetAssembly(typeof(T))!.GetTypes()
                     .Where(p_myType => p_myType.IsClass && !p_myType.IsAbstract && p_myType.IsSubclassOf(typeof(T))))
            objects.Add(((T)Activator.CreateInstance(type, p_constructorArgs)!));
        return objects;
    }
}