using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace VRBuilder.Core.Serialization;

/// <summary>
/// Converter that serializes and deserializes <see cref="Keyframe"/>.
/// </summary>
[NewtonsoftConverter]
public class KeyframeConverter : JsonConverter
{
    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return typeof(Keyframe) == objectType;
    }

    /// <inheritdoc/>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject)
        {
            var data = (JObject)JToken.ReadFrom(reader);
            var keyframe = new Keyframe(data["Time"].Value<float>(), data["Value"].Value<float>(), data["InTangent"].Value<float>(), data["OutTangent"].Value<float>(), data["InWeight"].Value<float>(), data["OutWeight"].Value<float>());
            keyframe.WeightedMode = (WeightedMode)data["WeightedMode"].Value<int>();
            return keyframe;
        }

        return new Keyframe();
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var keyframe = (Keyframe)value;
        var data = new JObject();

        data.Add("Time", keyframe.Time);
        data.Add("Value", keyframe.Value);
        data.Add("InTangent", keyframe.InTangent);
        data.Add("OutTangent", keyframe.OutTangent);
        data.Add("InWeight", keyframe.InWeight);
        data.Add("OutWeight", keyframe.OutWeight);
        data.Add("WeightedMode", (int)keyframe.WeightedMode);

        data.WriteTo(writer);
    }
}