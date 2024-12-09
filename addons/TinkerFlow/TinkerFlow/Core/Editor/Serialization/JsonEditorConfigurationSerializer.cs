#if UNITY_6000_0_OR_NEWER
// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System.Collections.Generic;
using Godot;
using VRBuilder.Core.Editor.Configuration;

namespace VRBuilder.Core.Editor.Serialization
{
    public static class JsonEditorConfigurationSerializer
    {
        private const int version = 0;

        public static string Serialize(AllowedMenuItemsSettings deserialized)
        {
            //TODO: JObject jObject = JObject.FromObject(, JsonSerializer.Create(SerializerSettings));
            // jObject.Add("$serializerVersion", version);
            // return jObject.ToString();
            return Json.Stringify(deserialized);
        }

        private static int RetrieveSerializerVersion(string serialized)
        {
            // TODO: return (int)JObject.Parse(serialized)["$serializerVersion"].ToObject(typeof(int));
            return 0;
        }

        public static AllowedMenuItemsSettings Deserialize(string serialized)
        {
            return Json.ParseString(serialized).As<AllowedMenuItemsSettings>();
        }
    }
}
#endif
