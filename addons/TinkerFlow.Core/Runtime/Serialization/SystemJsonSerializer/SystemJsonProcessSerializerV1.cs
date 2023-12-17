using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using VRBuilder.Core.IO;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Serialization.JSON;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
/// TODO: implement
public class SystemJsonProcessSerializerV1 : SystemJsonProcessSerializer
{
    protected virtual int Version { get; } = 1;

    public override IProcess ProcessFromByteArray(byte[] data)
    {
        return JsonSerializer.Deserialize<Process>(data, Options) ?? new Process("ERROR", Array.Empty<IChapter>());
    }

    public override byte[] ProcessToByteArray(IProcess target)
    {
        throw new NotImplementedException();
    }

    public override byte[] ChapterToByteArray(IChapter chapter)
    {
        throw new NotImplementedException();
    }

    public override IChapter ChapterFromByteArray(byte[] data)
    {
        throw new NotImplementedException();
    }

    public override byte[] StepToByteArray(IStep step)
    {
        throw new NotImplementedException();
    }

    public override IStep StepFromByteArray(byte[] data)
    {
        throw new NotImplementedException();
    }

    public override byte[] ManifestToByteArray(IProcessAssetManifest manifest)
    {
        throw new NotImplementedException();
    }

    public override IProcessAssetManifest ManifestFromByteArray(byte[] data)
    {
        throw new NotImplementedException();
    }
}