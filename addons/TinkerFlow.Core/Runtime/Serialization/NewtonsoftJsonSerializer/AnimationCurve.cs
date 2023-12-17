using System;
using System.Collections.Generic;
using Godot;

namespace VRBuilder.Core.Serialization;

public partial class AnimationCurve : Curve
{
    private readonly IEnumerable<Keyframe> keys = Array.Empty<Keyframe>();


    public AnimationCurve(IEnumerable<Keyframe> keys)
    {
        this.keys = keys;
    }

    public AnimationCurve()
    {
    }

    public WrapMode PreWrapMode { get; set; }
    public WrapMode PostWrapMode { get; set; }

    public IEnumerable<Keyframe> Keys => keys;
}