using System;
using System.Collections.Generic;
using System.Linq;

namespace VRBuilder.Editor.Util;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
public static class TypesExtension
{
    private static readonly Dictionary<Type, List<Type>> Cache = new();

    /// <summary>
    /// Returns all types in the current AppDomain implementing the interface or inheriting the type.
    /// source: https://stackoverflow.com/questions/80247/implementations-of-interface-through-reflection
    /// </summary>
    public static IEnumerable<Type> GetTypesDerivedFrom(Type? desiredType)
    {
        if (desiredType != null && !Cache.TryGetValue(desiredType, out List<Type>? types))
        {
            types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(desiredType.IsAssignableFrom).ToList();
            Cache.Add(desiredType, types);
        }
        else
        {
            types = new List<Type>();
        }

        return types;
    }

    /// <summary>
    /// Returns all types in the current AppDomain implementing the interface or inheriting the type. 
    /// </summary>
    public static IEnumerable<Type> GetTypesDerivedFrom<T>()
    {
        Type type = typeof(T);
        if (!Cache.TryGetValue(type, out List<Type>? types))
        {
            types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type.IsAssignableFrom)
                .Where(IsRealClass)
                .ToList();
            Cache.Add(type, types);
        }

        return types;
    }

    public static bool IsRealClass(Type testType)
    {
        return testType.IsAbstract == false
               && testType.IsGenericTypeDefinition == false
               && testType.IsInterface == false;
    }
}