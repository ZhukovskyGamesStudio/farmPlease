using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public static class StaticResetter
{
    private static List<Action> _resetActions = new List<Action>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        _resetActions.Clear();

        foreach (var type in GetAllTypesInAssemblies())
        {
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.IsDefined(typeof(ResettableAttribute), false))
                {
                    object defaultValue = GetDefault(field.FieldType);

                    _resetActions.Add(() => field.SetValue(null, defaultValue));
                }
            }
        }

        ResetAll(); // reset immediately before play
    }

    public static void ResetAll()
    {
        foreach (var reset in _resetActions)
        {
            reset.Invoke();
        }
    }

    private static IEnumerable<Type> GetAllTypesInAssemblies()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try {
                types = assembly.GetTypes();
            } catch (ReflectionTypeLoadException ex) {
                types = ex.Types;
            }

            foreach (var type in types)
            {
                if (type != null)
                    yield return type;
            }
        }
    }

    private static object GetDefault(Type t)
    {
        return t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}
