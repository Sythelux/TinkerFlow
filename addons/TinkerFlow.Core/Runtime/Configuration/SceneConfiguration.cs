using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Configuration;

/// <summary>
/// Handles configuration specific to this scene.
/// </summary>
public partial class SceneConfiguration : Node, ISceneConfiguration
{
	[SerializeField]
	// [Tooltip("Lists all assemblies whose property extensions will be used in the current scene.")]
	private List<string> extensionAssembliesWhitelist = new();

	[SerializeField]
	// [Tooltip("Default resources prefab to use for the Confetti behavior.")]
	private string defaultConfettiPrefab;

	#region ISceneConfiguration Members

	/// <inheritdoc/>
	public IEnumerable<string> ExtensionAssembliesWhitelist => extensionAssembliesWhitelist;

	/// <inheritdoc/>
	public string DefaultConfettiPrefab
	{
		get => defaultConfettiPrefab;
		set => defaultConfettiPrefab = value;
	}

	/// <inheritdoc/>
	public bool IsAllowedInAssembly(Type extensionType, string assemblyName)
	{
		if (ExtensionAssembliesWhitelist.Contains(assemblyName) == false) return false;

		PropertyExtensionExclusionList blacklist = this.GetComponents<PropertyExtensionExclusionList>().FirstOrDefault(blacklist => blacklist.AssemblyFullName == assemblyName);

		if (blacklist == null)
			return true;
		else
			return blacklist.DisallowedExtensionTypes.Any(disallowedType => disallowedType.FullName == extensionType.FullName) == false;
	}

	#endregion

	/// <summary>
	/// Adds the specified assembly names to the extension whitelist.
	/// </summary>
	public void AddWhitelistAssemblies(IEnumerable<string> assemblyNames)
	{
		foreach (string assemblyName in assemblyNames)
			if (extensionAssembliesWhitelist.Contains(assemblyName) == false)
				extensionAssembliesWhitelist.Add(assemblyName);
	}
}
