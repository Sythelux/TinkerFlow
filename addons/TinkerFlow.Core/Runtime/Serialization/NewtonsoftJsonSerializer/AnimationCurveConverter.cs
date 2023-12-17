using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Godot;

namespace VRBuilder.Core.Serialization;

/// <summary>
/// Converter that serializes and deserializes <see cref="AnimationCurve"/>.
/// </summary>
[NewtonsoftConverter]
public class AnimationCurveConverter : JsonConverter
{
    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return typeof(AnimationCurve) == objectType;
    }

    /// <inheritdoc/>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject)
            try
            {
                JObject data = JObject.Load(reader);
                var keys = data["Keys"].Value<JArray>();
                JsonReader keyReader = keys.CreateReader();
                Keyframe[] keyframes = serializer.Deserialize<Keyframe[]>(keyReader);
                var curve = new AnimationCurve(keyframes);
                curve.PreWrapMode = (WrapMode)data["PreWrapMode"].Value<int>();
                curve.PostWrapMode = (WrapMode)data["PostWrapMode"].Value<int>();

                return curve;
            }
            catch (Exception ex)
            {
                GD.PrintErr("Exception occured while trying to parse an animation curve.\n{0}", ex.Message);
                return new AnimationCurve();
            }

        GD.PushWarning("Can't read/parse animation curve from JSON.");
        return new AnimationCurve();
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var curve = (AnimationCurve)value;
        var data = new JObject();

        data.Add("Keys", new JArray(curve.Keys.Select(key => JObject.FromObject(key, serializer))));
        data.Add("PreWrapMode", (int)curve.PreWrapMode);
        data.Add("PostWrapMode", (int)curve.PostWrapMode);
        data.WriteTo(writer);
    }
}

public enum WrapMode
{
}

public class Keyframe
{
    public Keyframe(float time, float value, float inTangent, float outTangent, float inWeight, float outWeight)
    {
        Time = time;
        Value = value;
        InTangent = inTangent;
        OutTangent = outTangent;
        InWeight = inWeight;
        OutWeight = outWeight;
    }

    public Keyframe()
    {
    }

    public float Time { get; set; }
    public float Value { get; set; }
    public float InTangent { get; set; }
    public float OutTangent { get; set; }
    public float InWeight { get; set; }
    public float OutWeight { get; set; }
    public WeightedMode WeightedMode { get; set; }
}

public enum WeightedMode
{
}