using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRBuilder.Core.IO;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Serialization.JSON;

public abstract class SystemJsonProcessSerializer : IProcessSerializer
{
    public string Name { get; }
    public string FileFormat => "json";
    private JsonSerializerOptions? options;

    public JsonSerializerOptions Options
    {
        get
        {
            if (options == null)
            {
                options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true,
                    AllowTrailingCommas = true,
                    UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };
                GetJsonConverters().ForEach(c => options.Converters.Add(c));
            }

            return options;
        }
    }

    /// <summary>
    /// Creates a list of JsonConverters via reflection. It adds all JsonConverters with the <seealso cref="NewtonsoftConverterAttribute"/>
    /// will be added by default.
    /// </summary>
    /// <returns>A list of all found JsonConverters.</returns>
    private static List<JsonConverter> GetJsonConverters()
    {
        return ReflectionUtils.GetConcreteImplementationsOf<JsonConverter>()
            .WhichHaveAttribute<JsonConverterAttribute>()
            // .OrderBy(type => type.GetAttribute<JsonConverterAttribute>().Priority)
            .Select(type => ReflectionUtils.CreateInstanceOfType(type) as JsonConverter)
            .OfType<JsonConverter>()
            .ToList();
    }

    public abstract byte[] ProcessToByteArray(IProcess target);
    public abstract IProcess ProcessFromByteArray(byte[] data);
    public abstract byte[] ChapterToByteArray(IChapter chapter);
    public abstract IChapter ChapterFromByteArray(byte[] data);
    public abstract byte[] StepToByteArray(IStep step);
    public abstract IStep StepFromByteArray(byte[] data);
    public abstract byte[] ManifestToByteArray(IProcessAssetManifest manifest);
    public abstract IProcessAssetManifest ManifestFromByteArray(byte[] data);
}