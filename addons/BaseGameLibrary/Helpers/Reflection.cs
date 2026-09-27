using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ElementGodot.BaseGameLibrary.Helpers;

public class ReflectionHelper
{
    /// <summary>
    /// Instantiate a list of all the class Defined and Non-Abstract class that inherit the one given in template 
    /// </summary>
    /// <param name="p_constructorArgs">The possible argument to give in parameters</param>
    /// <typeparam name="TBaseType">The inherited type</typeparam>
    /// <returns>List of all object instantiated. (May need freeing, you're a adult do it yourself)</returns>
    public static IEnumerable<TBaseType> GetEnumerableOfTypeCreated<TBaseType>(params object[] p_constructorArgs) where TBaseType : class
    {
        List<TBaseType> objects = new List<TBaseType>();
        foreach (Type type in GetEnumerableOfType<TBaseType>())
            objects.Add(((TBaseType)Activator.CreateInstance(type, p_constructorArgs)!));
        return objects;
    }
    
    /// <summary>
    /// Get a enumerable of all the Defined and Non-Abstract class that inherit the one given in template
    /// </summary>
    /// <typeparam name="TBaseType">The inherited type</typeparam>
    /// <returns>List of Type with all the Inheritors</returns>
    public static IEnumerable<Type> GetEnumerableOfType<TBaseType>() where TBaseType : class =>
        Assembly.GetAssembly(typeof(TBaseType))!.GetTypes().Where(p_myType => p_myType.IsClass && !p_myType.IsAbstract && p_myType.IsSubclassOf(typeof(TBaseType)));
}