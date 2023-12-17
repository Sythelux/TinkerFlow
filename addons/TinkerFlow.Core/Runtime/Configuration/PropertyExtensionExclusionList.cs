using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace VRBuilder.Core.Configuration;

public partial class PropertyExtensionExclusionList : Node
{
    [SerializeField]
    // [Tooltip("Full name of the assembly we want to exclude the types from.")]
    private string assemblyFullName = string.Empty;

    [SerializeField]
    // [Tooltip("List of excluded extension type names, including namespaces.")]
    private readonly List<string> disallowedExtensionTypeNames = new();

    /// <summary>
    /// Full name of the assembly we want to exclude the types from.
    /// </summary>
    public string AssemblyFullName => assemblyFullName;

    /// <summary>
    /// List of excluded extension types.
    /// </summary>
    public IEnumerable<Type> DisallowedExtensionTypes
    {
        get
        {
            IEnumerable<string> assemblyQualifiedNames = disallowedExtensionTypeNames.Select(typeName => $"{typeName}, {assemblyFullName}");
            List<Type> excludedTypes = new List<Type>();

            foreach (string typeName in assemblyQualifiedNames)
            {
                var excludedType = Type.GetType(typeName);

                if (excludedType == null)
                {
                    //TODO: Debug.LogWarning($"Property extension exclusion list for assembly '{assemblyFullName}' contains invalid extension type: '{typeName}'.");
                }
                else
                {
                    excludedTypes.Add(excludedType);
                }
            }

            return excludedTypes;
        }
    }
}