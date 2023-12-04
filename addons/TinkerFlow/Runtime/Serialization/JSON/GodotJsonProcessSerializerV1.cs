
using VRBuilder.Core.IO;

namespace VRBuilder.Core.Serialization.JSON;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
/// TODO: implement
public class GodotJsonProcessSerializerV1: IProcessSerializer
{
    public string Name { get; }
    public string FileFormat { get; }
    public byte[] ProcessToByteArray(IProcess target)
    {
        throw new System.NotImplementedException();
    }

    public IProcess ProcessFromByteArray(byte[] data)
    {
        throw new System.NotImplementedException();
    }

    public byte[] ChapterToByteArray(IChapter chapter)
    {
        throw new System.NotImplementedException();
    }

    public IChapter ChapterFromByteArray(byte[] data)
    {
        throw new System.NotImplementedException();
    }

    public byte[] StepToByteArray(IStep step)
    {
        throw new System.NotImplementedException();
    }

    public IStep StepFromByteArray(byte[] data)
    {
        throw new System.NotImplementedException();
    }

    public byte[] ManifestToByteArray(IProcessAssetManifest manifest)
    {
        throw new System.NotImplementedException();
    }

    public IProcessAssetManifest ManifestFromByteArray(byte[] data)
    {
        throw new System.NotImplementedException();
    }
}