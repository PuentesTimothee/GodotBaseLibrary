using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary.Helpers;

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
    
    
    /// Tous les champs/propriétés [ControlNeeded] de type Control (classes filles incluses)
    public static MemberInfo[] GetMemberOfGivenTypeAndAttribute<TType, TAttribute>(Type p_type)
    {
        const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public
                                                         | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        var result = new List<MemberInfo>();

        for (Type? t = p_type; t != null && t != typeof(Node); t = t.BaseType)
        {
            foreach (FieldInfo f in t.GetFields(FLAGS))
                if (IsMemberOfValidClassAndAttribute<TType, TAttribute>(f, f.FieldType)) result.Add(f);

            foreach (PropertyInfo p in t.GetProperties(FLAGS))
                if (IsMemberOfValidClassAndAttribute<TType, TAttribute>(p, p.PropertyType)) result.Add(p);
        }

        return result.ToArray();
    }

    private static bool IsMemberOfValidClassAndAttribute<TType, TAttribute>(MemberInfo p_member, Type p_memberType) =>
        p_member.IsDefined(typeof(TAttribute), true)
        && typeof(TType).IsAssignableFrom((Type?)p_memberType);

    /// Les valeurs actuelles de ces membres
    public static List<TType> GetControlNeededValues<TType, TAttribute>(object p_pObject)
    {
        List<TType> list = new List<TType>();
        foreach (MemberInfo m in GetMemberOfGivenTypeAndAttribute<TType, TAttribute>(p_pObject.GetType()))
        {
            object? value = m switch
            {
                FieldInfo f => f.GetValue(p_pObject),
                PropertyInfo p when p.CanRead => p.GetValue(p_pObject),
                _ => null
            };
            if (value is TType c) list.Add(c);
        }
        return list;
    }

    public static string[] _GetConfigurationWarnings<TNodeType, TAttribute>(Node p_pSource)
    {
        Godot.Collections.Array<string> sData = new();

        foreach (MemberInfo pMemberInfos in ReflectionHelper
                     .GetMemberOfGivenTypeAndAttribute<TNodeType, TAttribute>(p_pSource.GetType()))
        {
            object? value = pMemberInfos is FieldInfo f
                ? f.GetValue(p_pSource)
                : ((PropertyInfo)pMemberInfos).GetValue(p_pSource);
            if (value == null)
                sData.Add($"{p_pSource.Name}, Missing {pMemberInfos.Name} members");
        }
        return sData.ToArray();
    }
}
