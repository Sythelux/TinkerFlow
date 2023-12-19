using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using VRBuilder.Core.Utils;
using FileAccess = Godot.FileAccess;

namespace VRBuilder.Core.Configuration;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
public partial class RuntimeConfigurator
{
    private static readonly List<Type> configurationTypes;
    private static readonly string[] configurationTypeNames;

    static RuntimeConfigurator()
    {
#pragma warning disable 0618
        configurationTypes = ReflectionUtils.GetConcreteImplementationsOf<IRuntimeConfiguration>().Except(new[] { typeof(RuntimeConfigWrapper) }).ToList();
#pragma warning restore 0618
        configurationTypes.Sort(((type1, type2) => string.Compare(type1.Name, type2.Name, StringComparison.Ordinal)));
        configurationTypeNames = configurationTypes.Select(t => t.Name).ToArray();

        // TODO: ProcessAssetPostprocessor.ProcessFileStructureChanged += OnProcessFileStructureChanged;
    }
    
    public override Variant _Get(StringName property)
    {
        return property.ToString() switch
        {
            nameof(RuntimeConfigurationName) => RuntimeConfigurationName,
            nameof(SelectedProcess) => SelectedProcess,
            nameof(ProcessStringLocalizationTable) => ProcessStringLocalizationTable,
            _ => base._Get(property)
        };
    }

    public override bool _Set(StringName property, Variant value)
    {
        switch (property.ToString())
        {
            case nameof(RuntimeConfigurationName):
                RuntimeConfigurationName = value.AsString();
                return true;
            case nameof(SelectedProcess):
                SelectedProcess = value.AsString();
                return true;
            case nameof(ProcessStringLocalizationTable):
                ProcessStringLocalizationTable = value.AsString();
                return true;
            default:
                return base._Set(property, value);
        }
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = new()
        {
            SerializeFieldAttribute.PropertyInfo(nameof(RuntimeConfigurationName), Variant.Type.String, PropertyHint.Enum, string.Join(',', configurationTypeNames)),
            SerializeFieldAttribute.PropertyInfo(nameof(SelectedProcess), Variant.Type.String, PropertyHint.File, "*.json"),
            SerializeFieldAttribute.PropertyInfo(nameof(ProcessStringLocalizationTable), Variant.Type.String, PropertyHint.File, "*.csv,*.po,*.translation,*.tres,*.res,*.mo"),
        };
        return properties;
    }

    public override string[] _GetConfigurationWarnings()
    {
        List<string> retVal = new();
        if (string.IsNullOrEmpty(RuntimeConfigurationName))
            retVal.Add(nameof(RuntimeConfigurationName) + " is Invalid");
        if (!FileAccess.FileExists(SelectedProcess))
            retVal.Add(nameof(SelectedProcess) + " is Invalid");
        if (!FileAccess.FileExists(ProcessStringLocalizationTable))
            retVal.Add(nameof(ProcessStringLocalizationTable) + " is Invalid");
        return retVal.ToArray();
    }
}