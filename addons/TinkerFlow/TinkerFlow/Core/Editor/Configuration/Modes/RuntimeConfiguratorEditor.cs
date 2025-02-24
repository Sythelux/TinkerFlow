#if GODOT
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using VRBuilder.Core.Utils;
using FileAccess = Godot.FileAccess;

namespace VRBuilder.Core.Configuration
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    [Icon("res://addons/TinkerFlow/TinkerFlow/Core/Resources/TinkerFlow.svg")]
    public partial class RuntimeConfigurator
    {
        private static readonly List<Type> configurationTypes;
        private static readonly string[] configurationTypeNames;

        static RuntimeConfigurator()
        {
#pragma warning disable 0618
            configurationTypes = ReflectionUtils.GetConcreteImplementationsOf<BaseRuntimeConfiguration>().ToList();
#pragma warning restore 0618
            configurationTypes.Sort(((type1, type2) => string.Compare(type1.Name, type2.Name, StringComparison.Ordinal)));
            configurationTypeNames = configurationTypes.Select(t => t.AssemblyQualifiedName).OfType<string>().Select(n => n.Replace(",", ";")).ToArray(); //,->; is a temporary fix as Godot, will interpret "," as separator for field names

            // TODO: ProcessAssetPostprocessor.ProcessFileStructureChanged += OnProcessFileStructureChanged;
        }

        public override Variant _Get(StringName property)
        {
            // GD.Print(property);
            return property.ToString() switch
            {
                nameof(runtimeConfigurationName) => configurationTypes.FirstOrDefault(t => t.AssemblyQualifiedName == runtimeConfigurationName)?.Name ?? string.Empty,
                nameof(selectedProcessStreamingAssetsPath) => selectedProcessStreamingAssetsPath,
                nameof(processStringLocalizationTable) => processStringLocalizationTable,
                _ => base._Get(property)
            };
        }

        public override bool _Set(StringName property, Variant value)
        {
            GD.Print(property);
            switch (property.ToString())
            {
                case nameof(runtimeConfigurationName):
                    runtimeConfigurationName = configurationTypes[configurationTypeNames.ToList().IndexOf(value.AsString())].AssemblyQualifiedName ?? string.Empty;
                    return true;
                case nameof(selectedProcessStreamingAssetsPath):
                    selectedProcessStreamingAssetsPath = value.AsString();
                    return true;
                case nameof(processStringLocalizationTable):
                    processStringLocalizationTable = value.AsString();
                    return true;
                default:
                    return base._Set(property, value);
            }
        }

        public override Array<Dictionary> _GetPropertyList()
        {
            Array<Dictionary> properties =
            [
                SerializeFieldAttribute.PropertyInfo(nameof(runtimeConfigurationName), Variant.Type.String, PropertyHint.Enum, string.Join(',', configurationTypeNames)),
                SerializeFieldAttribute.PropertyInfo(nameof(selectedProcessStreamingAssetsPath), Variant.Type.String, PropertyHint.File, "*.json"),
                SerializeFieldAttribute.PropertyInfo(nameof(processStringLocalizationTable), Variant.Type.String, PropertyHint.File, "*.csv,*.po,*.translation,*.tres,*.res,*.mo")
            ];
            return properties;
        }

        public override string[] _GetConfigurationWarnings()
        {
            List<string> retVal = [];
            if (string.IsNullOrEmpty(runtimeConfigurationName))
                retVal.Add(nameof(runtimeConfigurationName) + " is Invalid");
            if (!FileAccess.FileExists(selectedProcessStreamingAssetsPath))
                retVal.Add(nameof(selectedProcessStreamingAssetsPath) + " is Invalid");
            if (!FileAccess.FileExists(processStringLocalizationTable))
                retVal.Add(nameof(processStringLocalizationTable) + " is Invalid");
            return retVal.ToArray();
        }
    }
}
#endif