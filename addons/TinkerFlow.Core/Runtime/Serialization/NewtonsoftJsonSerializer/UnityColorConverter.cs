// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VRBuilder.Core.Serialization;

/// <summary>
/// Converts Unity color into json and back.
/// </summary>
[NewtonsoftConverter]
internal class UnityColorConverter : JsonConverter
{
    /// <inheritDoc/>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var color = (Color)value;
        var data = new JObject();

        data.Add("r", color.R);
        data.Add("g", color.G);
        data.Add("b", color.B);
        data.Add("a", color.A);

        data.WriteTo(writer);
    }

    /// <inheritDoc/>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject)
            try
            {
                var data = (JObject)JToken.ReadFrom(reader);

                var r = data["r"].Value<float>();
                var g = data["g"].Value<float>();
                var b = data["b"].Value<float>();
                var a = 1.0f;
                if (data.Count == 4) a = data["a"].Value<float>();

                return new Color(r, g, b, a);
            }
            catch (Exception ex)
            {
                GD.PrintErr("Exception occured while trying to parse a color.\n{0}", ex.Message);
                return Colors.Magenta;
            }

        GD.PushWarning("Can't read/parse color from JSON.");
        return Colors.Magenta;
    }


    /// <inheritDoc/>
    public override bool CanConvert(Type objectType)
    {
        return typeof(Color) == objectType;
    }
}