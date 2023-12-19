using System;
using System.Collections.Generic;
using System.Linq;
using VRBuilder.Core.Exceptions;

namespace VRBuilder.Core.SceneObjects;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
public class BaseSceneObjectRegistry : ISceneObjectRegistry
{
    protected readonly Dictionary<Guid, ISceneObject> registeredEntities = new();

    /// <inheritdoc />
    public ISceneObject this[Guid guid] => GetByGuid(guid);

    public BaseSceneObjectRegistry()
    {
        RegisterAll();
    }

    /// <inheritdoc />
    public void RegisterAll()
    {
    }

    /// <inheritdoc />
    public ISceneObject this[string name] => GetByName(name);

    /// <inheritdoc />
    public void Register(ISceneObject obj)
    {
        if (ContainsGuid(obj.Guid)) throw new AlreadyRegisteredException(obj);

        if (ContainsName(obj.UniqueName)) throw new NameNotUniqueException(obj);

        registeredEntities.Add(obj.Guid, obj);
    }

    /// <inheritdoc />
    public bool Unregister(ISceneObject entity)
    {
        return registeredEntities.Remove(entity.Guid);
    }

    /// <inheritdoc />
    public bool ContainsName(string name)
    {
        return registeredEntities.Any(entity => entity.Value.UniqueName == name);
    }

    /// <inheritdoc />
    public ISceneObject GetByName(string name)
    {
        if (ContainsName(name) == false) throw new MissingEntityException(string.Format("Could not find scene entity '{0}'", name));

        return registeredEntities.First(entity => entity.Value.UniqueName == name).Value;
    }

    /// <inheritdoc />
    public bool ContainsGuid(Guid guid)
    {
        return registeredEntities.ContainsKey(guid);
    }

    /// <inheritdoc />
    public ISceneObject GetByGuid(Guid guid)
    {
        try
        {
            return registeredEntities[guid];
        }
        catch (KeyNotFoundException)
        {
            throw new MissingEntityException(string.Format("Could not find scene entity with identifier '{0}'", guid.ToString()));
        }
    }

    /// <inheritdoc />
    public IEnumerable<ISceneObject> GetByTag(Guid tag)
    {
        return registeredEntities.Values.Where(entity => entity as ITagContainer != null && ((ITagContainer)entity).HasTag(tag));
    }

    /// <inheritdoc />
    public IEnumerable<T> GetPropertyByTag<T>(Guid tag)
    {
        return GetByTag(tag)
            .Where(sceneObject => sceneObject.Properties.Any(property => property is T))
            .Select(sceneObject => sceneObject.Properties.First(property => property is T))
            .Cast<T>();
    }
}