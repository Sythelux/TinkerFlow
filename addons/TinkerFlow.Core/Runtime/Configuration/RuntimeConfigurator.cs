// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using System.Linq;
using Godot;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Configuration;

/// <summary>
/// Configurator to set the process runtime configuration which is used by a process during its execution.
/// There has to be one and only one process runtime configurator game object per scene.
/// </summary>
[Tool]
public partial class RuntimeConfigurator : Node
{
	#region Delegates

	// [Signal] public delegate void ModeChangedEventHandler(string tag);
	/// <summary>
	/// The event that fires when a process runtime configuration changes.
	/// </summary>
	[Signal]
	public delegate void RuntimeConfigurationChangedEventHandler();

	#endregion

	private static RuntimeConfigurator? instance;

	protected BaseRuntimeConfiguration? runtimeConfiguration;

	/// <summary>
	/// Fully qualified name of the runtime configuration used.
	/// This field is magically filled by <see cref="RuntimeConfiguratorEditor"/>
	/// </summary>
	private string RuntimeConfigurationName { get; set; } = typeof(BaseRuntimeConfiguration).AssemblyQualifiedName ?? string.Empty;

	/// <summary>
	/// Process name which is selected.
	/// This field is magically filled by <see cref="RuntimeConfiguratorEditor"/>
	/// </summary>
	private string SelectedProcess { get; set; } = "";

	/// <summary>
	/// String localization table used by the current process.
	/// </summary>
	private string ProcessStringLocalizationTable { get; set; } = "";

	/// <summary>
	/// Checks if a process runtime configurator instance exists in scene.
	/// </summary>
	public static bool Exists
	{
		get
		{
			if (instance == null || instance.Equals(null)) instance = LookUpForGameObject();

			return instance != null && instance.Equals(null) == false;
		}
	}

	/// <summary>
	/// Shortcut to get the <see cref="IRuntimeConfiguration"/> of the instance.
	/// </summary>
	public static BaseRuntimeConfiguration? Configuration
	{
		get
		{
			if (Instance.runtimeConfiguration != null) return Instance.runtimeConfiguration;

			// Type? type = ReflectionUtils.GetTypeFromAssemblyQualifiedName(Instance.RuntimeConfigurationName);
			string instanceRuntimeConfigurationName = Instance.RuntimeConfigurationName.Replace(";",",");//,->; is a temporary fix as Godot, will interpret "," as separator for field names... now we have to switch it back.
			Type? type = ReflectionUtils.GetTypeFromAssemblyQualifiedName(instanceRuntimeConfigurationName);

			if (type == null)
			{
				GD.PrintErr($"IRuntimeConfiguration type '{instanceRuntimeConfigurationName}' cannot be found. returning for now");
				return null;
			}

			var config = (IRuntimeConfiguration)ReflectionUtils.CreateInstanceOfType(type);
			if (config is BaseRuntimeConfiguration configuration)
			{
				Configuration = configuration;
			}
			else
			{
				//TODO: Debug.LogWarning("Your runtime configuration only extends the interface IRuntimeConfiguration, please consider moving to BaseRuntimeConfiguration as base class.");
				// Configuration = new RuntimeConfigWrapper(config);
			}

			return Instance.runtimeConfiguration;
		}
		set
		{
			if (value == null)
			{
				GD.PrintErr("Process runtime configuration cannot be null.");
				return;
			}

			if (Instance.runtimeConfiguration == value) return;

			if (Instance.runtimeConfiguration != null) Instance.runtimeConfiguration.Modes.ModeChanged -= RuntimeConfigurationModeChanged;

			value.Modes.ModeChanged += RuntimeConfigurationModeChanged;

			Instance.RuntimeConfigurationName = value.GetType().AssemblyQualifiedName;
			Instance.runtimeConfiguration = value;

			Instance.EmitRuntimeConfigurationChanged();
		}
	}

	/// <summary>
	/// Current instance of the RuntimeConfigurator.
	/// </summary>
	/// <exception cref="NullReferenceException">Will throw a NPE if there is no RuntimeConfigurator added to the scene.</exception>
	public static RuntimeConfigurator Instance
	{
		get
		{
			if (Exists == false) throw new NullReferenceException("Process runtime configurator is not set in the scene. Create an empty game object with the 'RuntimeConfigurator' script attached to it.");

			return instance;
		}
	}

	/// <summary>
	/// The event that fires when a process mode or runtime configuration changes.
	/// </summary>
	public static event EventHandler<ModeChangedEventArgs> ModeChanged;

	private static RuntimeConfigurator? LookUpForGameObject()
	{
		RuntimeConfigurator[] instances = NodeExtensions.FindObjectsOfType<RuntimeConfigurator>().Take(2).ToArray();

		switch (instances.Length)
		{
			case > 1:
				GD.Print("More than one process runtime configurator is found in the scene. Taking the first one. This may lead to unexpected behaviour.");
				break;
			case 0:
				return null;
		}

		return instances[0];
	}

	/// <summary>
	/// Returns the assembly qualified name of the runtime configuration.
	/// </summary>
	public string GetRuntimeConfigurationName()
	{
		return RuntimeConfigurationName;
	}

	/// <summary>
	/// Sets the runtime configuration name, expects an assembly qualified name.
	/// </summary>
	public void SetRuntimeConfigurationName(string configurationName)
	{
		RuntimeConfigurationName = configurationName;
	}

	/// <summary>
	/// Returns the path to the selected process.
	/// </summary>
	public string GetSelectedProcess()
	{
		return SelectedProcess;
	}

	/// <summary>
	/// Sets the path to the selected process.
	/// </summary>
	public void SetSelectedProcess(string path)
	{
		SelectedProcess = path;
	}

	/// <summary>
	/// Returns the string localization table for the selected process.
	/// </summary>
	public string GetProcessStringLocalizationTable()
	{
		return ProcessStringLocalizationTable;
	}

	public override void _Ready()
	{
		// TODO: Configuration.SceneObjectRegistry.RegisterAll();
		RuntimeConfigurationChanged += HandleRuntimeConfigurationChanged;
	}

	private void OnDestroy()
	{
		ModeChanged = null;
		RuntimeConfigurationChanged -= HandleRuntimeConfigurationChanged;
	}

	private static void EmitModeChanged()
	{
		ModeChanged?.Invoke(Instance, new ModeChangedEventArgs(Instance.runtimeConfiguration?.Modes.CurrentMode));
	}

	private void EmitRuntimeConfigurationChanged()
	{
		EmitSignal(SignalName.RuntimeConfigurationChanged, Array.Empty<Variant>());
	}

	private void HandleRuntimeConfigurationChanged()
	{
		EmitModeChanged();
	}

	private static void RuntimeConfigurationModeChanged(object sender, ModeChangedEventArgs modeChangedEventArgs)
	{
		EmitModeChanged();
	}
}
