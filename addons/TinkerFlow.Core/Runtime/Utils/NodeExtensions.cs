using System.Collections.Generic;
using Godot;
using VRBuilder.Core.Properties;

namespace VRBuilder.Core.Utils;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
public static class NodeExtensions
{
    public static IEnumerable<T> FindObjectsOfType<T>(this Node self) where T : Node
    {
        //TODO
        yield break;
    }

    public static IEnumerable<T> FindObjectsOfType<T>() where T : Node
    {
        //TODO
        yield break;
    }

    public static T GetComponentInChildren<T>(this Node self)
    {
        //TODO:
        throw new System.NotImplementedException();
    }

    public static T GetComponent<T>(this Node self)
    {
        //TODO:
        throw new System.NotImplementedException();
    }

    public static IEnumerable<T> GetComponents<T>(this Node self)
    {
        //TODO:
        throw new System.NotImplementedException();
    }

    public static T AddComponent<T>(this Node self)
    {
        //TODO:
        throw new System.NotImplementedException();
    }
}